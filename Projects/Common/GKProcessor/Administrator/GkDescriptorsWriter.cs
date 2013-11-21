using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Common;
using FiresecClient;
using GKProcessor;
using Infrastructure.Common.Windows;
using XFiresecAPI;

namespace GKProcessor
{
	public static class GkDescriptorsWriter
	{
		public static void WriteConfig(XDevice gkDevice)
		{
			try
			{
				DescriptorsManager.Create();

				var gkDatabase = DescriptorsManager.GkDatabases.FirstOrDefault(x => x.RootDevice.UID == gkDevice.UID);
				if (gkDatabase != null)
				{
					var result = Ping(gkDatabase);
					if (!result)
						return;

					string error = null;

					for (int i = 0; i < 3; i++)
					{
						var summaryDescriptorsCount = 4 + gkDatabase.Descriptors.Count;
						gkDatabase.KauDatabases.ForEach(x => { summaryDescriptorsCount += 3 + x.Descriptors.Count; });

						var title = "Запись конфигурации в " + gkDatabase.RootDevice.PresentationDriverAndAddress + (i > 0 ? " Попытка " + (i + 1) : "");
						LoadingService.Show(title, title, summaryDescriptorsCount, true);

						result = GoToTechnologicalRegime(gkDatabase.RootDevice);
						if (!result)
						{
							error = "Не удалось перевести ГК в технологический режим";
							continue;
						}
						if (LoadingService.IsCanceled)
							return;

						if(!EraseDatabase(gkDatabase.RootDevice))
						{
							error = "Не удалось стереть базу данных ГК";
							continue;
						}

						foreach (var kauDatabase in gkDatabase.KauDatabases)
						{
							result = GoToTechnologicalRegime(kauDatabase.RootDevice);
							if (!result)
							{
								error = "Не удалось перевести КАУ в технологический режим";
								continue;
							}

							if (!EraseDatabase(kauDatabase.RootDevice))
							{
								error = "Не удалось стереть базу данных КАУ";
								continue;
							}

							if (LoadingService.IsCanceled)
								return;

							var writeResult = WriteConfigToDevice(kauDatabase);
							if (LoadingService.IsCanceled)
								return;
							if (!writeResult)
							{
								error = "Не удалось записать дескриптор КАУ";
								continue;
							}
						}
						var writeResult2 = WriteConfigToDevice(gkDatabase);
						if (LoadingService.IsCanceled)
							return;
						if (!writeResult2)
						{
							error = "Не удалось записать дескриптор ГК";
							continue;
						}

						foreach (var kauDatabase in gkDatabase.KauDatabases)
						{
							if (!GoToWorkingRegime(kauDatabase.RootDevice))
							{
								error = "Не удалось перевести КАУ в рабочий режим";
								break;
							}
						}
						if (!GoToWorkingRegime(gkDatabase.RootDevice))
						{
							error = "Не удалось перевести ГК в рабочий режим";
							break;
						}
						FiresecManager.FiresecService.NotifyClientsOnConfigurationChanged();
						return;
					}
					if (error != null)
					{
						result = MessageBoxService.ShowQuestion("Во время записи конфигурации возникла ошибка" + Environment.NewLine + error + Environment.NewLine + "Перевести устройства в рабочий режим") == System.Windows.MessageBoxResult.Yes;
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "GKDescriptorsWriter.WriteConfig");
			}
			finally
			{
				LoadingService.Close();
			}
		}

		static bool Ping(GkDatabase gkDatabase)
		{
			var sendResult = SendManager.Send(gkDatabase.RootDevice, 0, 1, 1);
			if (sendResult.HasError)
			{
				MessageBoxService.ShowError("Устройство " + gkDatabase.RootDevice.PresentationDriverAndAddress + " недоступно");
				return false;
			}

			foreach (var kauDatabase in gkDatabase.KauDatabases)
			{
				sendResult = SendManager.Send(kauDatabase.RootDevice, 0, 1, 1);
				if (sendResult.HasError)
				{
					MessageBoxService.ShowError("Устройство " + kauDatabase.RootDevice.PresentationDriverAndAddress + " недоступно");
					return false;
				}
			}
			return true;
		}
	
		static bool WriteConfigToDevice(CommonDatabase commonDatabase)
		{
			foreach (var descriptor in commonDatabase.Descriptors)
			{
				if (LoadingService.IsCanceled)
					return false;

				if (commonDatabase is GkDatabase && descriptor.Device != null && descriptor.Device.DriverType == XDriverType.Shuv)
				{
					;
				}

				var progressStage = commonDatabase.RootDevice.PresentationDriverAndAddress + ": запись " +
					descriptor.XBase.GetDescriptorName() + " " +
					"(" + descriptor.GetDescriptorNo().ToString() + ")" +
					" из " + commonDatabase.Descriptors.Count.ToString();
				LoadingService.DoStep(progressStage);
				var packs = GkDescriptorsWriter.CreateDescriptors(descriptor);
				foreach (var pack in packs)
				{
					var packBytesCount = pack.Count;
					var sendResult = SendManager.Send(commonDatabase.RootDevice, (ushort)(packBytesCount), 17, 0, pack, true);
					if (sendResult.HasError)
					{
						LoadingService.Close();
						Trace.WriteLine(progressStage);
						return false;
					}
				}
			}
			WriteEndDescriptor(commonDatabase);
			return true;
		}

		static List<List<byte>> CreateDescriptors(BaseDescriptor descriptor)
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

		static List<byte> CreateEndDescriptor(ushort descriptorNo)
		{
			var resultBytes = new List<byte>();
			resultBytes.AddRange(BytesHelper.ShortToBytes(descriptorNo));
			resultBytes.Add(1);
			resultBytes.Add(255);
			resultBytes.Add(255);
			return resultBytes;
		}

		public static bool GoToTechnologicalRegime(XDevice device)
		{
			if (IsInTechnologicalRegime(device))
				return true;

			LoadingService.DoStep(device.PresentationDriverAndAddress + " Переход в технологический режим");
			var sendResult = SendManager.Send(device, 0, 14, 0, null, device.DriverType == XDriverType.GK);
			if (sendResult.HasError)
			{
				return false;
			}

			for (int i = 0; i < 10; i++)
			{
				if (IsInTechnologicalRegime(device))
					return true;
				Thread.Sleep(TimeSpan.FromSeconds(1));
			}

			return false;
		}

		static bool IsInTechnologicalRegime(XDevice device)
		{
			var sendResult = SendManager.Send(device, 0, 1, 1, null, true, false, 2000);
			if (!sendResult.HasError)
			{
				if (sendResult.Bytes.Count > 0)
				{
					var version = sendResult.Bytes[0];
					if (version > 127)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool GoToWorkingRegime(XDevice device)
		{
			LoadingService.DoStep(device.PresentationDriverAndAddress + " Переход в рабочий режим");
			if (LoadingService.IsCanceled)
				return true;
			SendManager.Send(device, 0, 11, 0, null, device.DriverType == XDriverType.GK);

			for (int i = 0; i < 100; i++)
			{
				if (LoadingService.IsCanceled)
					return true;
				var sendResult = SendManager.Send(device, 0, 1, 1);
				if (!sendResult.HasError)
				{
					if (sendResult.Bytes.Count > 0)
					{
						var version = sendResult.Bytes[0];
						if (version <= 127)
						{
							return true;
						}
					}
				}
				Thread.Sleep(TimeSpan.FromSeconds(1));
			}

			return false;
		}

		static bool EraseDatabase(XDevice device)
		{
			LoadingService.DoStep(device.PresentationDriverAndAddress + " Стирание базы данных");
			for (int i = 0; i < 3; i++)
			{
				var sendResult = SendManager.Send(device, 0, 15, 0, null, true, false, 10000);
				if (!sendResult.HasError)
				{
					return true;
				}
				else
				{
					Thread.Sleep(TimeSpan.FromSeconds(1));
				}
			}
			return false;
		}

		static void WriteEndDescriptor(CommonDatabase commonDatabase)
		{
			LoadingService.DoStep(commonDatabase.RootDevice.PresentationDriverAndAddress + " Запись завершающего дескриптора");
			var endBytes = GkDescriptorsWriter.CreateEndDescriptor((ushort)(commonDatabase.Descriptors.Count + 1));
			SendManager.Send(commonDatabase.RootDevice, 5, 17, 0, endBytes, true);
		}
	}
}