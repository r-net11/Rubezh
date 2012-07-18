using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GKModule.Database;
using Infrastructure.Common.Windows;

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
			SendManager.Send(commonDatabase.RootDevice, 0, 14, 0);

			LoadingService.DoStep("Стирание базы данных");
			SendManager.Send(commonDatabase.RootDevice, 0, 15, 0);

			foreach (var binaryObject in commonDatabase.BinaryObjects)
			{
				LoadingService.DoStep("Запись дескриптора " + binaryObject.GetNo().ToString());
				var bytes = BinConfigurationCreator.CreateDescriptor(binaryObject);
				SendManager.Send(commonDatabase.RootDevice, (short)(3 + bytes.Count()), 17, 0, bytes);
			}
			LoadingService.DoStep("Запись завершающего дескриптора");
			var endBytes = BinConfigurationCreator.CreateEndDescriptor((short)(commonDatabase.BinaryObjects.Count + 1));
			SendManager.Send(commonDatabase.RootDevice, 3 + 2, 17, 0, endBytes);

			LoadingService.DoStep("Запуск программы");
			SendManager.Send(commonDatabase.RootDevice, 0, 11, 0);

			LoadingService.Close();
		}

		static List<byte> CreateDescriptor(BinaryObjectBase binaryObject)
		{
			var resultBytes = new List<byte>();
			resultBytes.AddRange(BytesHelper.ShortToBytes((short)(binaryObject.GetNo())));
			resultBytes.Add(1);
			resultBytes.AddRange(binaryObject.AllBytes);
			return resultBytes;
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