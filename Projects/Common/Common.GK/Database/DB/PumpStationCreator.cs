using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using FiresecClient;

namespace Common.GK
{
	public static class PumpStationCreator
	{
		public static void Create(GkDatabase gkDatabase, XDevice pumpStatioDevice)
		{
			if (pumpStatioDevice.PumpStationProperty == null)
				return;

			int delayTime = 2;
			var delayProperty = pumpStatioDevice.Properties.FirstOrDefault(x => x.Name == "Delay");
			if (delayProperty != null)
			{
				delayTime = delayProperty.Value;
			}

			int pumpCount = 1;
			var pumpCountProperty = pumpStatioDevice.Properties.FirstOrDefault(x => x.Name == "PumpCount");
			if (pumpCountProperty != null)
			{
				pumpCount = pumpCountProperty.Value;
			}

			var delays = new List<XDelay>();

			var pumpDevices = new List<XDevice>();
			foreach (var pumpDeviceUID in pumpStatioDevice.PumpStationProperty.DeviceUIDs)
			{
				var pumpDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == pumpDeviceUID);
				if (pumpDevice != null)
				{
					var addressOnShleif = pumpDevice.IntAddress % 256;
					if (addressOnShleif <= 8)
					{
						pumpDevices.Add(pumpDevice);
					}
				}
			}

			int pumpIndex = 1;
			foreach (var pumpDevice in pumpDevices)
			{
				var delay = new XDelay()
				{
					Name = "Задержка пуска ШУН " + pumpIndex.ToString(),
					DelayTime = (ushort)(pumpIndex * delayTime),
					SetTime = 1,
					InitialState = false,
				};
				delay.InputObjects.Add(pumpDevice);
				pumpDevice.OutputObjects.Add(delay);
				delays.Add(delay);
			}
			pumpIndex++;

			foreach (var pumpDevice in pumpDevices)
			{
				var pumpBinaryObject = gkDatabase.BinaryObjects.FirstOrDefault(x => x.Device != null && x.Device.UID == pumpDevice.UID);
				if (pumpBinaryObject != null)
				{
					;
				}
			}

			foreach (var pumpDevice in pumpDevices)
			{
				foreach (var delay in delays)
				{
					pumpDevice.InputObjects.Add(delay);
					delay.OutputObjects.Add(pumpDevice);
				}
			}

			foreach (var delay in delays)
			{
				gkDatabase.AddDelay(delay);
				var deviceBinaryObject = new DelayBinaryObject(delay);
				gkDatabase.BinaryObjects.Add(deviceBinaryObject);
			}
		}
	}
}