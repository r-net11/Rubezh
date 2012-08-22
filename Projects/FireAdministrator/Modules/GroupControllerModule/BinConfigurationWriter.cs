using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common.GK;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using System.Diagnostics;

namespace GKModule
{
	public static class BinConfigurationWriter
	{
		public static void WriteConfig()
		{
			DatabaseManager.Convert();

			SendManager.StrartLog("D:/GkLog.txt");
			foreach (var gkDatabase in DatabaseManager.GkDatabases)
			{
				var summaryObjectsCount = 3 + gkDatabase.BinaryObjects.Count;
				gkDatabase.KauDatabases.ForEach(x => { summaryObjectsCount += 3 + x.BinaryObjects.Count; });
				LoadingService.ShowProgress("", gkDatabase.RootDevice.PresentationDriverAndAddress + "\nЗапись конфигурации в ГК", summaryObjectsCount);

				GoToTechnologicalRegime(gkDatabase.RootDevice);
				EraseDatabase(gkDatabase.RootDevice);

				foreach (var kauDatabase in gkDatabase.KauDatabases)
				{
					GoToTechnologicalRegime(kauDatabase.RootDevice);
					EraseDatabase(kauDatabase.RootDevice);
					WriteConfigToDevice(kauDatabase);
					GoToWorkingRegime(kauDatabase.RootDevice);
				}
				WriteConfigToDevice(gkDatabase);
				GoToWorkingRegime(gkDatabase.RootDevice);

				LoadingService.Close();
			}
			SendManager.StopLog();
		}

		static void WriteConfigToDevice(CommonDatabase commonDatabase)
		{
			foreach (var binaryObject in commonDatabase.BinaryObjects)
			{
				var progressStage = commonDatabase.RootDevice.PresentationDriverAndAddress + ": запись дескриптора " +
					binaryObject.GetNo().ToString() + " из " + commonDatabase.BinaryObjects.Count.ToString();
				LoadingService.DoStep(progressStage);
				var packs = BinConfigurationWriter.CreateDescriptors(binaryObject);
				foreach (var pack in packs)
				{
					var packBytesCount = pack.Count;
					var sendResult = SendManager.Send(commonDatabase.RootDevice, (ushort)(packBytesCount), 17, 0, pack, true);
					if (sendResult.HasError)
					{
						MessageBoxService.Show(sendResult.Error);
						//LoadingService.Close();
						//break;
					}
				}
			}
			WriteEndDescriptor(commonDatabase);
		}

		static List<List<byte>> CreateDescriptors(BinaryObjectBase binaryObject)
		{
			var objectNo = (ushort)(binaryObject.GetNo());
			if (objectNo == 236)
			{
				;
			}

			var packs = new List<List<byte>>();
			for (int packNo = 0; packNo <= binaryObject.AllBytes.Count / 256; packNo++)
			{
				int packLenght = Math.Min(256, binaryObject.AllBytes.Count - packNo * 256);
				var packBytes = binaryObject.AllBytes.Skip(packNo * 256).Take(packLenght).ToList();

				if (packBytes.Count > 0)
				{
					var resultBytes = new List<byte>();
					ushort binaryObjectNo = (ushort)(binaryObject.GetNo());
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

		static void GoToTechnologicalRegime(XDevice device)
		{
			LoadingService.DoStep(device.PresentationDriverAndAddress + " Переход в технологический режим");
			SendManager.Send(device, 0, 14, 0, null, device.Driver.DriverType == XDriverType.GK);

			for (int i = 0; i < 10; i++)
			{
				var sendResult = SendManager.Send(device, 0, 1, 1);
				if (!sendResult.HasError)
				{
					if (sendResult.Bytes.Count > 0)
					{
						var version = sendResult.Bytes[0];
						if (version >= 80)
						{
							break;
						}
					}
				}
				Thread.Sleep(1000);
			}
		}

		static void GoToWorkingRegime(XDevice device)
		{
			LoadingService.DoStep(device.PresentationDriverAndAddress + " Переход в рабочий режим");
			SendManager.Send(device, 0, 11, 0, null, device.Driver.DriverType == XDriverType.GK);

			for (int i = 0; i < 10; i++)
			{
				var sendResult = SendManager.Send(device, 0, 1, 1);
				if (!sendResult.HasError)
				{
					if (sendResult.Bytes.Count > 0)
					{
						var version = sendResult.Bytes[0];
						if (version < 80)
						{
							break;
						}
					}
				}
				Thread.Sleep(1000);
			}
		}

		static void EraseDatabase(XDevice device)
		{
			LoadingService.DoStep(device.ShortPresentationAddressAndDriver + " Стирание базы данных");
			SendManager.Send(device, 0, 15, 0);
		}

		static void WriteEndDescriptor(CommonDatabase commonDatabase)
		{
			LoadingService.DoStep(commonDatabase.RootDevice.PresentationDriverAndAddress + " Запись завершающего дескриптора");
			var endBytes = BinConfigurationWriter.CreateEndDescriptor((ushort)(commonDatabase.BinaryObjects.Count + 1));
			SendManager.Send(commonDatabase.RootDevice, 5, 17, 0, endBytes, true);
		}
	}
}