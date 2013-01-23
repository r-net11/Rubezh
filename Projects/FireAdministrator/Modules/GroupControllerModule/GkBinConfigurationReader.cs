using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using Infrastructure.Common.Windows;
using Common.GK;
using System.Diagnostics;
using FiresecClient;

namespace GKModule
{
	public static class GkBinConfigurationReader
	{
		public static bool ReadConfiguration(XDevice gkDevice)
		{
			var devices = new List<XDevice>();

			LoadingService.SaveShowProgress("Перевод ГК в технологический режим", 1);
			BinConfigurationWriter.GoToTechnologicalRegime(gkDevice);

			LoadingService.SaveShowProgress("Чтение базы данных объектов ГК", ushort.MaxValue + 1);

			ushort descriptorNo = 0;
			while (true)
			{
				descriptorNo++;
				LoadingService.SaveDoStep("Чтение базы данных объектов ГК " + descriptorNo);

				byte packNo = 1;
				var descriptorNoBytes = new List<byte>(BitConverter.GetBytes(descriptorNo));
				var data = new List<byte>(descriptorNoBytes);
				data.Add(packNo);
				var sendResult = SendManager.Send(gkDevice, 3, 19, ushort.MaxValue, data);
				var bytes = sendResult.Bytes;
				if (sendResult.HasError || bytes.Count == 0)
					break;

				if (bytes.Count < 5)
					break;

				var inputDescriptorNo = BytesHelper.SubstructShort(bytes, 0);
				var inputPackNo = bytes[2];
				if (bytes[3] == 0xff && bytes[4] == 0xff)
					break;

				var device = Parce(bytes.Skip(3).ToList());
				devices.Add(device);
			}

			LoadingService.SaveDoStep("Перевод ГК в рабочий режим");
			BinConfigurationWriter.GoToWorkingRegime(gkDevice);
			LoadingService.SaveClose();

			return true;
		}

		static XDevice Parce(List<byte> bytes)
		{
			//Trace.WriteLine(BytesHelper.BytesToString(bytes));

			var internalType = BytesHelper.SubstructShort(bytes, 0);
			var controllerAdress = BytesHelper.SubstructShort(bytes, 2);
			var adressOnController = BytesHelper.SubstructShort(bytes, 4);
			var physicalAdress = BytesHelper.SubstructShort(bytes, 6);

			string typeName = "Неизвестный тип";
			
			var driver = XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverTypeNo == internalType);
			if (driver != null)
			{
				typeName = driver.ShortName;
			}
			if (internalType == 0x100)
				typeName = "Зона";
			if (internalType == 0x106)
				typeName = "Направление";

			Trace.WriteLine(typeName + " " + controllerAdress + " " + adressOnController + " " + physicalAdress);

			return new XDevice();
		}
	}
}