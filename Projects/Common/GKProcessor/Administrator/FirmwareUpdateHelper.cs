using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;

namespace GKProcessor
{
	public class FirmwareUpdateHelper
	{
		public List<string> ErrorList = new List<string>();
		string Error;
		public GKProgressCallback ProgressCallback { get; private set; }

		public void Update(GKDevice device, string fileName, string userName)
		{
			var firmWareBytes = HexFileToBytesList(fileName);
			//Update(device, firmWareBytes, userName);
			GKProcessorManager.StopProgress(ProgressCallback);
			if (Error != null)
				ErrorList.Add(Error);
		}

		public void UpdateFSCS(HexFileCollectionInfo hxcFileInfo, List<GKDevice> devices, string userName)
		{
			foreach (var device in devices)
			{
				var fileInfo = hxcFileInfo.HexFileInfos.FirstOrDefault(x => x.DriverType == device.DriverType);
				if (fileInfo == null)
				{
					Error = "Не найден файл прошивки для устройства типа " + device.DriverType;
					return;
				}
				var bytes = StringsToBytes(fileInfo.Lines);
			//	Update(device, bytes, userName);
				GKProcessorManager.StopProgress(ProgressCallback);
				if (Error != null)
					ErrorList.Add(Error);
				Error = null;
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

		bool Clear(GKDevice device)
		{
			var sendResult = SendManager.Send(device, 0, 16, 0, null, true, false, 4000);
			if (sendResult.HasError)
				return false;
			return true;
		}
	}
}