using System.Collections.Generic;
using System.Linq;
using Common.GK;
using Infrastructure.Common.Windows;
using System;
using System.Threading;
using XFiresecAPI;

namespace GKModule
{
	public static class BinConfigurationWriter
	{
		public static void WriteConfig()
		{
			DatabaseManager.Convert();

			foreach (var gkDatabase in DatabaseManager.GkDatabases)
			{
				LoadingService.Show("Запись конфигурации в ГК", 2 + gkDatabase.BinaryObjects.Count);
				WriteConfigToDevice(gkDatabase);
			}
			Thread.Sleep(10000);
			foreach (var kauDatabase in DatabaseManager.KauDatabases)
			{
				LoadingService.Show("Запись конфигурации в КАУ", 2 + kauDatabase.BinaryObjects.Count);
				WriteConfigToDevice(kauDatabase);
			}
		}

		static void WriteConfigToDevice(CommonDatabase commonDatabase)
		{
			var recieveAnswer = commonDatabase.DatabaseType == DatabaseType.Gk;
			recieveAnswer = true;

			LoadingService.DoStep("Переход в технологический режим");
			SendManager.Send(commonDatabase.RootDevice, 0, 14, 0, null, false);
			Thread.Sleep(10000);

			LoadingService.DoStep("Стирание базы данных");
			SendManager.Send(commonDatabase.RootDevice, 0, 15, 0, null, false);
			Thread.Sleep(10000);

			foreach (var binaryObject in commonDatabase.BinaryObjects)
			{
				LoadingService.DoStep("Запись дескриптора " + binaryObject.GetNo().ToString());
				var packs = BinConfigurationWriter.CreateDescriptors(binaryObject);
				if (packs.Count > 1)
				{
					//MessageBoxService.Show("Отправка нескольких пакетов в конфигурации дескриптора");
				}
				foreach (var pack in packs)
				{
					var packBytesCount = pack.Count;
					for (int i = 0; i < 256 - packBytesCount; i++)
					{
						pack.Add(0);
					}

					var sendResult = SendManager.Send(commonDatabase.RootDevice, (ushort)(packBytesCount), 17, 0, pack, recieveAnswer);
					if (sendResult.HasError)
					{
						MessageBoxService.Show(sendResult.Error);
						LoadingService.Close();
						return;
					}
				}
			}
			LoadingService.DoStep("Запись завершающего дескриптора");
			var endBytes = BinConfigurationWriter.CreateEndDescriptor((ushort)(commonDatabase.BinaryObjects.Count + 1));
			SendManager.Send(commonDatabase.RootDevice, 5, 17, 0, endBytes, recieveAnswer);

			LoadingService.DoStep("Запуск программы");
			SendManager.Send(commonDatabase.RootDevice, 0, 11, 0, null, false);

			LoadingService.Close();
		}

		static List<List<byte>> CreateDescriptors(BinaryObjectBase binaryObject)
		{
			var packs = new List<List<byte>>();
			for (int packNo = 0; packNo <= binaryObject.AllBytes.Count / 256; packNo++)
			{
				int packLenght = Math.Min(256, binaryObject.AllBytes.Count - packNo * 256);
				var packBytes = binaryObject.AllBytes.Skip(packNo * 256).Take(packLenght).ToList();

				var resultBytes = new List<byte>();
				resultBytes.AddRange(BytesHelper.ShortToBytes((ushort)(binaryObject.GetNo())));
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
	}
}