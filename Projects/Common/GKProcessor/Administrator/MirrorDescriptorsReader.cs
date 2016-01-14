using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI;
using RubezhAPI.GK;

namespace GKProcessor
{
	public class MirrorDescriptorsReader : DescriptorReaderBase
	{
		GKDevice MirrorDevice;

		override public bool ReadConfiguration(GKDevice mirrorDevice, Guid clientUID)
		{
			MirrorDevice = (GKDevice)mirrorDevice.Clone();
			MirrorDevice.Children = new List<GKDevice>();

			var progressCallback = GKProcessorManager.StartProgress("Чтение конфигурации " + mirrorDevice.PresentationName, "Проверка связи", 2, true, GKProgressClientType.Administrator);
			var result = DeviceBytesHelper.Ping(mirrorDevice);
			if (result.HasError)
			{
				Error = "Устройство " + mirrorDevice.PresentationName + " недоступно";
				return false;
			}

			DeviceConfiguration = new GKDeviceConfiguration { RootDevice = MirrorDevice };
			GKProcessorManager.DoProgress("Перевод ПМФ в технологический режим", progressCallback);
			if (!DeviceBytesHelper.GoToTechnologicalRegime(mirrorDevice, progressCallback, clientUID))
			{
				Error = "Не удалось перевести " + mirrorDevice.PresentationName + " в технологический режим";
				GKProcessorManager.StopProgress(progressCallback);
				return false;
			}

			ReadConfiguration(mirrorDevice);

			GKProcessorManager.DoProgress("Перевод ПМФ в рабочий режим", progressCallback);
			if (!DeviceBytesHelper.GoToWorkingRegime(mirrorDevice, progressCallback, clientUID))
			{
				Error = "Не удалось перевести  " + mirrorDevice.PresentationName + " в рабочий режим в заданное время";
			}
			GKProcessorManager.StopProgress(progressCallback);
			return Error == null;
		}

		void ReadConfiguration(GKDevice mirrorDevice)
		{
			var progressCallback = GKProcessorManager.StartProgress("Чтение конфигурации " + mirrorDevice.PresentationName, "", 1, true, GKProgressClientType.Administrator);
			ushort descriptorNo = 0;
			while (true)
			{
				if (progressCallback.IsCanceled)
				{
					Error = "Операция отменена";
					break;
				}
				descriptorNo++;
				GKProcessorManager.DoProgress("Чтение базы данных объектов ПМФ " + descriptorNo, progressCallback);
				const byte packNo = 1;
				var data = new List<byte>(BitConverter.GetBytes(descriptorNo)) { packNo };

				for (int i = 0; i < 3; i++)
				{
					var sendResult = SendManager.Send(mirrorDevice, 3, 19, ushort.MaxValue, data);
					var bytes = sendResult.Bytes;

					if (!sendResult.HasError && bytes.Count >= 5)
					{
						if (bytes[3] == 0xff && bytes[4] == 0xff)
							return;
						if (!Parse(bytes.Skip(3).ToList(), descriptorNo))
							return;
						break;
					}

					if (i == 2)
					{
						Error = "Возникла ошибка при чтении объекта " + descriptorNo;
						return;
					}
				}
			}
		}

		bool Parse(List<byte> bytes, int descriptorNo)
		{
			var internalType = BytesHelper.SubstructShort(bytes, 0);
			var controllerAdress = BytesHelper.SubstructShort(bytes, 2);
			var adressOnController = BytesHelper.SubstructShort(bytes, 4);
			var physicalAdress = BytesHelper.SubstructShort(bytes, 6);
			if (internalType == 0)
				return true;
			var description = BytesHelper.BytesToStringDescription(bytes);
			var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverTypeNo == internalType);
			if (driver != null && driver.DriverType != GKDriverType.GK)
			{
				var device = new GKDevice
				{
					Driver = driver,
					DriverUID = driver.UID,
					IntAddress = (byte)(physicalAdress % 256),
				};
				MirrorDevice.Children.Add(device);
			}

			return true;
		}
	}
}