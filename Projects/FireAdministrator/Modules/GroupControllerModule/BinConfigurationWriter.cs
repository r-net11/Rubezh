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

			//SendManager.StrartLog("D:/xxx/Log.txt");
			//foreach (var kauDatabase in DatabaseManager.KauDatabases)
			//{
			//    LoadingService.ShowProgress("Запись конфигурации в устройство", "Запись конфигурации в КАУ", 2 + kauDatabase.BinaryObjects.Count);
			//    WriteConfigToDevice(kauDatabase);
			//}
			//Thread.Sleep(1000);
			foreach (var gkDatabase in DatabaseManager.GkDatabases)
			{
				LoadingService.ShowProgress("Запись конфигурации в устройство", "Запись конфигурации в ГК", 2 + gkDatabase.BinaryObjects.Count);

				GoToTechnologicalRegime(gkDatabase.RootDevice);
				EraseDatabase(gkDatabase.RootDevice);

				foreach (var kauDatabase in gkDatabase.KauDatabases)
				{
					LoadingService.ShowProgress("Запись конфигурации в устройство", "Запись конфигурации в КАУ", 1 + gkDatabase.BinaryObjects.Count);
					GoToTechnologicalRegime(kauDatabase.RootDevice);
					EraseDatabase(kauDatabase.RootDevice);
					WriteConfigToDevice(kauDatabase);
					GoToWorkingRegime(kauDatabase.RootDevice);
				}
				LoadingService.ShowProgress("Запись конфигурации в устройство", "Запись конфигурации в ГК", 1 + gkDatabase.BinaryObjects.Count);
				WriteConfigToDevice(gkDatabase);
				GoToWorkingRegime(gkDatabase.RootDevice);
				
				LoadingService.Close();
			}
			//SendManager.StopLog();
		}

		static void WriteConfigToDevice(CommonDatabase commonDatabase)
		{
			foreach (var binaryObject in commonDatabase.BinaryObjects)
			{
				LoadingService.DoStep("Запись дескриптора " + binaryObject.GetNo().ToString());
				var packs = BinConfigurationWriter.CreateDescriptors(binaryObject);
				foreach (var pack in packs)
				{
					var packBytesCount = pack.Count;
					var sendResult = SendManager.Send(commonDatabase.RootDevice, (ushort)(packBytesCount), 17, 0, pack, true);
					if (sendResult.HasError)
					{
						MessageBoxService.Show(sendResult.Error);
						LoadingService.Close();
						break;
					}
				}
			}
			WriteEndDescriptor(commonDatabase);
		}

		static List<List<byte>> CreateDescriptors(BinaryObjectBase binaryObject)
		{
			var packs = new List<List<byte>>();
			for (int packNo = 0; packNo <= binaryObject.AllBytes.Count / 256; packNo++)
			{
				int packLenght = Math.Min(256, binaryObject.AllBytes.Count - packNo * 256);
				var packBytes = binaryObject.AllBytes.Skip(packNo * 256).Take(packLenght).ToList();

				var resultBytes = new List<byte>();
				ushort binaryObjectNo = (ushort)(binaryObject.GetNo());
				resultBytes.AddRange(BytesHelper.ShortToBytes(binaryObjectNo));
				resultBytes.Add((byte)(packNo + 1));
				resultBytes.AddRange(packBytes);
				packs.Add(resultBytes);
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
			var sendResult0 = SendManager.Send(device, 0, 1, 1);
			Trace.WriteLine("Версия ПО до перехода в технологический режим = " + BytesHelper.BytesToString(sendResult0.Bytes));

			LoadingService.DoStep("Переход в технологический режим");
			var sendResult1 = SendManager.Send(device, 0, 14, 0, null, device.Driver.DriverType == XDriverType.GK);
			if (sendResult1.HasError)
			{
				Trace.WriteLine("ошибка при переходе в технолошический режим");
			}
			else
			{
				Trace.WriteLine(BytesHelper.BytesToString(sendResult1.Bytes));
			}

			for (int i = 0; i < 10; i++)
			{
				var sendResult = SendManager.Send(device, 0, 1, 1);
				if (sendResult.HasError)
				{
					Trace.WriteLine("Ошибка. Версия ПО после перехода в технологический режим");
				}
				else
				{
					if (sendResult.Bytes.Count > 0)
					{
						var version = sendResult.Bytes[0];
						if (version > 80)
						{
							Trace.WriteLine("перейден в технологический режим");
							Trace.WriteLine("Версия ПО после перехода в технологический режим" + BytesHelper.BytesToString(sendResult.Bytes));
							break;
						}
					}
				}
				Thread.Sleep(1000);
			}
		}

		static void EraseDatabase(XDevice device)
		{
			LoadingService.DoStep("Стирание базы данных");
			SendManager.Send(device, 0, 15, 0);
		}

		static void WriteEndDescriptor(CommonDatabase commonDatabase)
		{
			LoadingService.DoStep("Запись завершающего дескриптора");
			var endBytes = BinConfigurationWriter.CreateEndDescriptor((ushort)(commonDatabase.BinaryObjects.Count + 1));
			SendManager.Send(commonDatabase.RootDevice, 5, 17, 0, endBytes, true);
		}

		static void GoToWorkingRegime(XDevice device)
		{
			var sendResult1 = SendManager.Send(device, 0, 1, 1);
			if (sendResult1.HasError)
			{
				Trace.WriteLine("Ошибка. Версия ПО до запуска");
			}
			else
			{
				Trace.WriteLine("Версия ПО до запуска " + BytesHelper.BytesToString(sendResult1.Bytes));
			}

			LoadingService.DoStep("Запуск программы");
			SendManager.Send(device, 0, 11, 0, null, device.Driver.DriverType == XDriverType.GK);

			var sendResult2 = SendManager.Send(device, 0, 1, 1);
			if (sendResult2.HasError)
			{
				Trace.WriteLine("Ошибка. Версия ПО после запуска");
			}
			else
			{
				Trace.WriteLine("Версия ПО после запуска " + BytesHelper.BytesToString(sendResult2.Bytes));
			}
		}
	}
}