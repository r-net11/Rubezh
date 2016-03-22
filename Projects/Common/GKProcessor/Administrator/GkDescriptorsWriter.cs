using Common;
using RubezhAPI;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GKProcessor
{
	public class GkDescriptorsWriter
	{
		public List<string> Errors { get; private set; }

		public GkDescriptorsWriter()
		{
			Errors = new List<string>();
		}

		public void WriteConfig(GKDevice gkControllerDevice, GKProgressCallback progressCallback, Guid clientUID)
		{
			Errors = new List<string>();

			progressCallback = GKProcessorManager.StartProgress("Запись конфигурации", "Проверка связи", 1, false, GKProgressClientType.Administrator, clientUID);
			try
			{
				var gkDatabase = DescriptorsManager.GkDatabases.FirstOrDefault(x => x.RootDevice.UID == gkControllerDevice.UID);
				if (gkDatabase != null)
				{
					var kauDatabases = new List<KauDatabase>();
					var lostKauDatabases = new List<KauDatabase>();
					using (var gkLifecycleManager = new GKLifecycleManager(gkDatabase.RootDevice, "Проверка связи"))
					{
						gkLifecycleManager.AddItem("ГК");
						var pingResult = DeviceBytesHelper.Ping(gkDatabase.RootDevice);
						if (pingResult.HasError)
						{
							Errors.Add("Устройство " + gkDatabase.RootDevice.PresentationName + " недоступно");
							return;
						}

						foreach (var kauDatabase in gkDatabase.KauDatabases)
						{
							gkLifecycleManager.AddItem(kauDatabase.RootDevice.PresentationName);
							pingResult = DeviceBytesHelper.Ping(kauDatabase.RootDevice);
							if (!pingResult.HasError)
							{
								kauDatabases.Add(kauDatabase);
							}
							else
							{
								lostKauDatabases.Add(kauDatabase);
							}
						}
					}

					for (int i = 0; i < 3; i++)
					{
						Errors = new List<string>();

						var summaryDescriptorsCount = 4 + gkDatabase.Descriptors.Count;
						kauDatabases.ForEach(x => { summaryDescriptorsCount += 3 + x.Descriptors.Count; });

						var title = "Запись конфигурации в " + gkDatabase.RootDevice.PresentationName + (i > 0 ? " Попытка " + (i + 1) : "");
						progressCallback = GKProcessorManager.StartProgress(title, "", summaryDescriptorsCount, false, GKProgressClientType.Administrator, clientUID);
						var result = DeviceBytesHelper.GoToTechnologicalRegime(gkDatabase.RootDevice, progressCallback, clientUID);
						if (!result)
						{
							Errors.Add("Не удалось перевести " + gkControllerDevice.PresentationName + " в технологический режим\nУстройство не доступно, либо вашего IP адреса нет в списке разрешенного адреса ГК");
							continue;
						}

						result = DeviceBytesHelper.EraseDatabase(gkDatabase.RootDevice, progressCallback, clientUID);
						if (!result)
						{
							Errors.Add("Не удалось стереть базу данных ГК");
							continue;
						}

						foreach (var kauDatabase in kauDatabases)
						{
							result = DeviceBytesHelper.GoToTechnologicalRegime(kauDatabase.RootDevice, progressCallback, clientUID);
							if (!result)
							{
								Errors.Add("Не удалось перевести КАУ в технологический режим");
								continue;
							}

							if (!DeviceBytesHelper.EraseDatabase(kauDatabase.RootDevice, progressCallback, clientUID))
							{
								Errors.Add("Не удалось стереть базу данных КАУ");
								continue;
							}

							if (!WriteConfigToDevice(kauDatabase, progressCallback, clientUID))
							{
								Errors.Add("Не удалось записать дескриптор КАУ");
							}
						}

						result = WriteConfigToDevice(gkDatabase, progressCallback, clientUID);
						if (!result)
						{
							Errors.Add("Не удалось записать дескриптор ГК");
							continue;
						}


						var gkFileReaderWriter = new GKFileReaderWriter();
						gkFileReaderWriter.WriteFileToGK(gkControllerDevice, clientUID);
						if (gkFileReaderWriter.Error != null)
						{
							Errors.Add(gkFileReaderWriter.Error);
							break;
						}

						foreach (var kauDatabase in kauDatabases)
						{
							if (!DeviceBytesHelper.GoToWorkingRegime(kauDatabase.RootDevice, progressCallback, clientUID, kauDatabase.RootDevice.Driver.IsKau))
							{
								Errors.Add("Не удалось перевести " + kauDatabase.RootDevice.PresentationName + " в рабочий режим");
							}
						}

						if (!DeviceBytesHelper.GoToWorkingRegime(gkDatabase.RootDevice, progressCallback, clientUID, false))
						{
							Errors.Add("Не удалось перевести " + gkDatabase.RootDevice + " в рабочий режим");
						}
						break;
					}

					foreach (var kauDatabase in lostKauDatabases)
					{
						Errors.Add("Устройство " + kauDatabase.RootDevice.PresentationName + " недоступно");
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "GKDescriptorsWriter.WriteConfig");
				Errors.Add(e.Message);
			}
			finally
			{
				if (progressCallback != null)
					GKProcessorManager.StopProgress(progressCallback, clientUID);
			}
		}

		bool WriteConfigToDevice(CommonDatabase commonDatabase, GKProgressCallback progressCallback, Guid clientUID)
		{
			using (var gkLifecycleManager = new GKLifecycleManager(commonDatabase.RootDevice, "Запись дескрипторов"))
			{
				for (int descriptorNo = 0; descriptorNo < commonDatabase.Descriptors.Count; descriptorNo++)
				{
					var descriptor = commonDatabase.Descriptors[descriptorNo];
					gkLifecycleManager.Progress(descriptorNo + 1, commonDatabase.Descriptors.Count);
					var progressStage = commonDatabase.RootDevice.PresentationName + ": запись " + descriptor.GKBase.PresentationName + " " + "(" + descriptor.GetDescriptorNo() + ")" + " из " + commonDatabase.Descriptors.Count;
					GKProcessorManager.DoProgress(progressStage, progressCallback, clientUID);

					var packs = CreateDescriptors(descriptor);
					foreach (var pack in packs)
					{
						for (int i = 0; i < 10; i++)
						{
							var sendResult = SendManager.Send(commonDatabase.RootDevice, (ushort)(pack.Count), 17, 0, pack);
							if (sendResult.HasError)
							{
								if (i >= 9)
								{
									GKProcessorManager.StopProgress(progressCallback, clientUID);
									gkLifecycleManager.AddItem("Ошибка");
									return false;
								}
							}
							else
								break;
						}
					}
				}
				GKProcessorManager.DoProgress(commonDatabase.RootDevice.PresentationName + " Запись завершающего дескриптора", progressCallback, clientUID);
				WriteEndDescriptor(commonDatabase);
				return true;
			}
		}

		List<List<byte>> CreateDescriptors(BaseDescriptor descriptor)
		{
			var objectNo = (ushort)(descriptor.GetDescriptorNo());

			var packs = new List<List<byte>>();
			for (int packNo = 0; packNo <= descriptor.AllBytes.Count / 256; packNo++)
			{
				int packLenght = Math.Min(256, descriptor.AllBytes.Count - packNo * 256);
				var packBytes = descriptor.AllBytes.Skip(packNo * 256).Take(packLenght).ToList();

				if (packBytes.Count > 0)
				{
					var resultBytes = new List<byte>();
					ushort descriptorNo = (ushort)(descriptor.GetDescriptorNo());
					resultBytes.AddRange(BytesHelper.ShortToBytes(descriptorNo));
					resultBytes.Add((byte)(packNo + 1));
					resultBytes.AddRange(packBytes);
					packs.Add(resultBytes);
				}
			}
			return packs;
		}

		List<byte> CreateEndDescriptor(ushort descriptorNo)
		{
			var resultBytes = new List<byte>();
			resultBytes.AddRange(BytesHelper.ShortToBytes(descriptorNo));
			resultBytes.Add(1);
			resultBytes.Add(255);
			resultBytes.Add(255);
			return resultBytes;
		}

		void WriteEndDescriptor(CommonDatabase commonDatabase)
		{
			var endBytes = CreateEndDescriptor((ushort)(commonDatabase.Descriptors.Count + 1));
			SendManager.Send(commonDatabase.RootDevice, 5, 17, 0, endBytes, true);
		}
	}
}