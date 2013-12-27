using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using System.IO;
using HexManager;
namespace GKProcessor
{
	public class FirmwareUpdateHelper
	{
		public List<string> ErrorList = new List<string>();
	    string error;
		public void Update(XDevice device, string fileName)
		{
			var firmWareBytes = HexFileToBytesList(fileName);
			Update(device, firmWareBytes);
            if (!String.IsNullOrEmpty(error))
                ErrorList.Add(error);
		}

		public void Update(XDevice device, List<byte> firmWareBytes)
		{
			LoadingService.Show("Обновление прошивки " + device.PresentationName, "", firmWareBytes.Count / 256, true);
            LoadingService.DoStep("Опрос устройства " + device.PresentationName);
            if (!DeviceBytesHelper.Ping(device))
            {
                error = "Устройство " + device.PresentationName + " недоступно";
                LoadingService.Close();
                return;
            }
			if (!DeviceBytesHelper.GoToTechnologicalRegime(device))
			{
				error = "Не удалось перевести " + device.PresentationName + " в технологический режим\n" +
						"Устройство не доступно, либо вашего " +
						"IP адреса нет в списке разрешенного адреса ГК";
				GKProcessorManager.OnStopProgress();
				return;
			}
			var softVersion = DeviceBytesHelper.GetDeviceInfo(device);
			LoadingService.DoStep("Удаление программы " + device.PresentationName);
			DeviceBytesHelper.Clear(device);
			var data = new List<byte>();
			var offset = 0;
			if (device.Driver.IsKauOrRSR2Kau)
				offset = 0x10000;
			for (int i = 0; i < firmWareBytes.Count; i = i + 0x100)
			{
				if (LoadingService.IsCanceled)
				{ error = "Операция обновления прошивки отменена"; LoadingService.Close(); return; }
				LoadingService.DoStep("Запись блока данных " + i / 0x100 + 1);
				data = new List<byte>(BitConverter.GetBytes(i + offset));
				data.AddRange(firmWareBytes.GetRange(i, 0x100));
				var result = SendManager.Send(device, 260, 0x12, 0, data, true, false, 10000);
				if (result.HasError)
				{ error = "В заданное времени не пришел ответ от устройства"; LoadingService.Close(); return; }
			}
			DeviceBytesHelper.GoToWorkingRegime(device);
			LoadingService.Close();
		}

		public void UpdateFSCS(HexFileCollectionInfo hxcFileInfo, string userName, List<XDevice> devices)
		{
		    foreach (var device in devices)
		    {
		        var fileInfo = new HEXFileInfo();
                if (device.DriverType == XDriverType.GK)
                    fileInfo = hxcFileInfo.FileInfos.FirstOrDefault(x => x.FileName == "GK_V1.hcs");
                if (device.DriverType == XDriverType.KAU)
                    fileInfo = hxcFileInfo.FileInfos.FirstOrDefault(x => x.FileName == "KAU_RSR1_V1.hcs");
                if (device.DriverType == XDriverType.RSR2_KAU)
                    fileInfo = hxcFileInfo.FileInfos.FirstOrDefault(x => x.FileName == "KAU_RSR2_V1.hcs");
                if (fileInfo == null)
                    return;
		        var bytes = StringsToBytes(fileInfo.Lines);
		        Update(device, bytes);
                if (!String.IsNullOrEmpty(error))
                    ErrorList.Add(error);
                GKProcessorManager.AddGKMessage("Обновление ПО прибора", "", XStateClass.Info, device, userName, true);
		    }
		}

		List<byte> HexFileToBytesList(string filePath)
		{
			
			var strings = File.ReadAllLines(filePath).ToList();
			strings.RemoveAt(0);
			strings.RemoveRange(strings.Count - 1, 1);
		    return StringsToBytes(strings);
		}

	    List<byte> StringsToBytes(List<string> strings)
	    {
            var bytes = new List<byte>();
            foreach (var str in strings)
            {
                var count = Convert.ToInt32(str.Substring(1, 2), 16);
                if (count != 0x10)
                    continue;
                for (var i = 9; i < count * 2 + 9; i += 2)
                {
                    bytes.Add(Convert.ToByte(str.Substring(i, 2), 16));
                }
            }
            return bytes;
	    }
	}
}