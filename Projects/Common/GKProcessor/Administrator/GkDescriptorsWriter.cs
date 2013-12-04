using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common;
using Infrastructure.Common.Windows;
using XFiresecAPI;

namespace GKProcessor
{
	public static class GkDescriptorsWriter
	{
		public static string Error { get; private set; }

		public static void WriteConfig(XDevice gkDevice, bool writeFileToGK = false)
		{
			try
			{
				DescriptorsManager.Create();
				var gkDatabase = DescriptorsManager.GkDatabases.FirstOrDefault(x => x.RootDevice.UID == gkDevice.UID);
				if (gkDatabase != null)
				{
					var result = DeviceBytesHelper.Ping(gkDatabase.RootDevice);
					if (!result)
						{Error = "Устройство " + gkDatabase.RootDevice.PresentationName + " недоступно"; return;}
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
						var title = "Запись конфигурации в " + gkDatabase.RootDevice.PresentationDriverAndAddress + (i > 0 ? " Попытка " + (i + 1) : "");
						LoadingService.Show(title, title, summaryDescriptorsCount, true);
						result = DeviceBytesHelper.GoToTechnologicalRegime(gkDatabase.RootDevice);
						if (!result)
							{ Error = "Не удалось перевести ГК в технологический режим"; continue; }
						if (LoadingService.IsCanceled)
							return;
						if(!DeviceBytesHelper.EraseDatabase(gkDatabase.RootDevice))
							{ Error = "Не удалось стереть базу данных ГК"; continue; }
						foreach (var kauDatabase in gkDatabase.KauDatabases)
						{
							result = DeviceBytesHelper.GoToTechnologicalRegime(kauDatabase.RootDevice);
							if (!result)
								{ Error = "Не удалось перевести КАУ в технологический режим"; continue; }
							if (!DeviceBytesHelper.EraseDatabase(kauDatabase.RootDevice))
								{ Error = "Не удалось стереть базу данных КАУ"; continue;}
							if (!WriteConfigToDevice(kauDatabase))
								{ Error = "Не удалось записать дескриптор КАУ"; }
						}
						if (!WriteConfigToDevice(gkDatabase))
							{ Error = "Не удалось записать дескриптор ГК"; continue; }

						GKFileReaderWriter.WriteFileToGK(gkDevice, writeFileToGK);
						if (!String.IsNullOrEmpty(GKFileReaderWriter.Error))
							{ Error = GKFileReaderWriter.Error; break; }

						if (gkDatabase.KauDatabases.Any(kauDatabase => !DeviceBytesHelper.GoToWorkingRegime(kauDatabase.RootDevice)))
							{ Error = "Не удалось перевести КАУ в рабочий режим"; }
						if (!DeviceBytesHelper.GoToWorkingRegime(gkDatabase.RootDevice))
							{ Error = "Не удалось перевести ГК в рабочий режим"; }
						break;
					}
				}
			}
			catch (Exception e)
				{ Logger.Error(e, "GKDescriptorsWriter.WriteConfig"); }
			finally
				{ LoadingService.Close(); }
		}
	
		static bool WriteConfigToDevice(CommonDatabase commonDatabase)
		{
			foreach (var descriptor in commonDatabase.Descriptors)
			{
				if (LoadingService.IsCanceled)
					return false;
				var progressStage = commonDatabase.RootDevice.PresentationDriverAndAddress + ": запись " +
					descriptor.XBase.GetDescriptorName() + " " + "(" + descriptor.GetDescriptorNo() + ")" +
					" из " + commonDatabase.Descriptors.Count;
				LoadingService.DoStep(progressStage);
				var packs = CreateDescriptors(descriptor);
				foreach (var pack in packs)
				{
					var packBytesCount = pack.Count;
					var sendResult = SendManager.Send(commonDatabase.RootDevice, (ushort)(packBytesCount), 17, 0, pack);
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

		static void WriteEndDescriptor(CommonDatabase commonDatabase)
		{
			LoadingService.DoStep(commonDatabase.RootDevice.PresentationDriverAndAddress + " Запись завершающего дескриптора");
			var endBytes = CreateEndDescriptor((ushort)(commonDatabase.Descriptors.Count + 1));
			SendManager.Send(commonDatabase.RootDevice, 5, 17, 0, endBytes, true);
		}
	}
}