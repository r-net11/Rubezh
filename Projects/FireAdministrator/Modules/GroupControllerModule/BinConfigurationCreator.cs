using System.Collections.Generic;
using System.Linq;
using Commom.GK;
using Infrastructure.Common.Windows;
using System;

namespace GKModule
{
	public static class BinConfigurationCreator
	{
		public static void WriteConfig()
		{
			DatabaseProcessor.Convert();

			foreach (var kauDatabase in DatabaseProcessor.DatabaseCollection.KauDatabases)
			{
				LoadingService.Show("Запись конфигурации в КАУ", 2 + kauDatabase.BinaryObjects.Count);
				WriteOneConfig(kauDatabase);
			}

			foreach (var gkDatabase in DatabaseProcessor.DatabaseCollection.GkDatabases)
			{
				LoadingService.Show("Запись конфигурации в ГК", 2 + gkDatabase.BinaryObjects.Count);
				WriteOneConfig(gkDatabase);
			}
		}

		static void WriteOneConfig(CommonDatabase commonDatabase)
		{
			LoadingService.DoStep("Переход в технологический режим");
			SendManager.Send(commonDatabase.RootDevice, 0, 14, 0, null, false);

			LoadingService.DoStep("Стирание базы данных");
			SendManager.Send(commonDatabase.RootDevice, 0, 15, 0);

			foreach (var binaryObject in commonDatabase.BinaryObjects)
			{
				LoadingService.DoStep("Запись дескриптора " + binaryObject.GetNo().ToString());
				var packs = BinConfigurationCreator.CreateDescriptors(binaryObject);
				if (packs.Count > 1)
				{
					MessageBoxService.Show("Отправка нескольких пакетов в конфигурации дескриптора");
				}
				foreach (var pack in packs)
				{
					var packBytesCount = pack.Count;
					for (int i = 0; i < 256 - packBytesCount; i++)
					{
						pack.Add(0);
					}

					SendManager.Send(commonDatabase.RootDevice, (short)(packBytesCount), 17, 0, pack, true);
				}
			}
			LoadingService.DoStep("Запись завершающего дескриптора");
			var endBytes = BinConfigurationCreator.CreateEndDescriptor((short)(commonDatabase.BinaryObjects.Count + 1));
			SendManager.Send(commonDatabase.RootDevice, 5, 17, 0, endBytes);

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
				resultBytes.AddRange(BytesHelper.ShortToBytes((short)(binaryObject.GetNo())));
				resultBytes.Add((byte)(packNo + 1));
				resultBytes.AddRange(packBytes);
				packs.Add(resultBytes);
			}
			return packs;
		}

		static List<byte> CreateEndDescriptor(short descriptorNo)
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