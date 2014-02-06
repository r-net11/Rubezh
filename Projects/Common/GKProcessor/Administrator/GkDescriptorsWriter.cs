using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI;
using XFiresecAPI;

namespace GKProcessor
{
	public class GkDescriptorsWriter
	{
		public string Error { get; private set; }

		public void WriteConfig(XDevice gkDevice)
		{
			var progressCallback = GKProcessorManager.StartProgress("Запись конфигурации", "Проверка связи", 1, true, GKProgressClientType.Administrator);
			try
			{
				DescriptorsManager.Create();
				var gkDatabase = DescriptorsManager.GkDatabases.FirstOrDefault(x => x.RootDevice.UID == gkDevice.UID);
				if (gkDatabase != null)
				{
					var result = DeviceBytesHelper.Ping(gkDatabase.RootDevice);
					if (!result)
					{ Error = "Устройство " + gkDatabase.RootDevice.PresentationName + " недоступно"; return; }
					foreach (var kauDatabase in gkDatabase.KauDatabases)
					{
						result = DeviceBytesHelper.Ping(kauDatabase.RootDevice);
						if (!result)
						{ Error = "Устройство " + kauDatabase.RootDevice.PresentationName + " недоступно"; return; }
					}
					for (int i = 0; i < 3; i++)
					{
						var summaryDescriptorsCount = 4 + gkDatabase.Descriptors.Count;
						gkDatabase.KauDatabases.ForEach(x => { summaryDescriptorsCount += 3 + x.Descriptors.Count; });
						var title = "Запись конфигурации в " + gkDatabase.RootDevice.PresentationName + (i > 0 ? " Попытка " + (i + 1) : "");
						progressCallback = GKProcessorManager.StartProgress(title, "", summaryDescriptorsCount, true, GKProgressClientType.Administrator);
						result = DeviceBytesHelper.GoToTechnologicalRegime(gkDatabase.RootDevice, progressCallback);
						if (progressCallback.IsCanceled)
						{
							DeviceBytesHelper.GoToWorkingRegime(gkDatabase.RootDevice, progressCallback);
							return;
						}
						if (!result)
						{
							Error = "Не удалось перевести " + gkDevice.PresentationName + " в технологический режим\n" +
								   "Устройство не доступно, либо вашего " +
								   "IP адреса нет в списке разрешенного адреса ГК"; continue;
						}
						result = DeviceBytesHelper.EraseDatabase(gkDatabase.RootDevice, progressCallback);
						if (progressCallback.IsCanceled)
						{
							gkDatabase.KauDatabases.Any(x => !DeviceBytesHelper.GoToWorkingRegime(x.RootDevice, progressCallback));
							DeviceBytesHelper.GoToWorkingRegime(gkDatabase.RootDevice, progressCallback);
							return;
						}
						if (!result)
						{ Error = "Не удалось стереть базу данных ГК"; continue; }
						foreach (var kauDatabase in gkDatabase.KauDatabases)
						{
							if (progressCallback.IsCanceled)
							{
								gkDatabase.KauDatabases.Any(x => !DeviceBytesHelper.GoToWorkingRegime(x.RootDevice, progressCallback));
								DeviceBytesHelper.GoToWorkingRegime(gkDatabase.RootDevice, progressCallback);
								return;
							}
							result = DeviceBytesHelper.GoToTechnologicalRegime(kauDatabase.RootDevice, progressCallback);
							if (!result)
							{ Error = "Не удалось перевести КАУ в технологический режим"; continue; }
							if (!DeviceBytesHelper.EraseDatabase(kauDatabase.RootDevice, progressCallback))
							{ Error = "Не удалось стереть базу данных КАУ"; continue; }
							if (!WriteConfigToDevice(kauDatabase, progressCallback))
							{ Error = "Не удалось записать дескриптор КАУ"; }
						}
						result = WriteConfigToDevice(gkDatabase, progressCallback);
						if (progressCallback.IsCanceled)
						{
							gkDatabase.KauDatabases.Any(x => !DeviceBytesHelper.GoToWorkingRegime(x.RootDevice, progressCallback));
							DeviceBytesHelper.GoToWorkingRegime(gkDatabase.RootDevice, progressCallback);
							return;
						}
						if (!result)
						{ Error = "Не удалось записать дескриптор ГК"; continue; }
						var gkFileReaderWriter = new GKFileReaderWriter();
						gkFileReaderWriter.WriteFileToGK(gkDevice);
						if (gkFileReaderWriter.Error != null)
						{ Error = gkFileReaderWriter.Error; break; }
						if (gkDatabase.KauDatabases.Any(x => !DeviceBytesHelper.GoToWorkingRegime(x.RootDevice, progressCallback)))
						{ Error = "Не удалось перевести КАУ в рабочий режим"; }
						if (!DeviceBytesHelper.GoToWorkingRegime(gkDatabase.RootDevice, progressCallback))
						{ Error = "Не удалось перевести ГК в рабочий режим"; }
						break;
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "GKDescriptorsWriter.WriteConfig");
				Error = e.Message;
			}
			finally
			{
				if (progressCallback != null)
					GKProcessorManager.StopProgress(progressCallback);
			}
		}

		bool WriteConfigToDevice(CommonDatabase commonDatabase, GKProgressCallback progressCallback)
		{
			foreach (var descriptor in commonDatabase.Descriptors)
			{
				if (progressCallback.IsCanceled)
					return false;
				var progressStage = commonDatabase.RootDevice.PresentationName + ": запись " +
					descriptor.XBase.PresentationName + " " + "(" + descriptor.GetDescriptorNo() + ")" +
					" из " + commonDatabase.Descriptors.Count;
				GKProcessorManager.DoProgress(progressStage, progressCallback);
				var packs = CreateDescriptors(descriptor);
				foreach (var pack in packs)
				{
					var packBytesCount = pack.Count;
					var sendResult = SendManager.Send(commonDatabase.RootDevice, (ushort)(packBytesCount), 17, 0, pack);
					if (sendResult.HasError)
					{
						GKProcessorManager.StopProgress(progressCallback);
						return false;
					}
				}
			}
			GKProcessorManager.DoProgress(commonDatabase.RootDevice.PresentationName + " Запись завершающего дескриптора", progressCallback);
			WriteEndDescriptor(commonDatabase);
			return true;
		}

		List<List<byte>> CreateDescriptors(BaseDescriptor descriptor)
		{
			var objectNo = (ushort)(descriptor.GetDescriptorNo());

			var packs = new List<List<byte>>();
			for (int packNo = 0; packNo <= descriptor.AllBytes.Count / 256; packNo++)
			{
				int packLenght = Math.Min(256, descriptor.AllBytes.Count - packNo * 256);
				var packBytes = descriptor.AllBytes.Skip(packNo * 256).Take(packLenght).ToList();

				if (packBytes.Count > 0)// || (descriptor.Device != null && descriptor.Device.DriverType == XDriverType.Shuv))
				{
					var resultBytes = new List<byte>();
					ushort binaryObjectNo = (ushort)(descriptor.GetDescriptorNo());
					resultBytes.AddRange(BytesHelper.ShortToBytes(binaryObjectNo));
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