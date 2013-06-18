using System;
using System.Collections.Generic;
using System.Linq;
using FS2Api;
using ServerFS2.Helpers;
using ServerFS2.Service;
using Device = FiresecAPI.Models.Device;

namespace ServerFS2
{
    public static partial class ServerHelper
    {
        public static event Action<int, int, string> Progress;

        public static void SynchronizeTime(Device device)
        {
			USBManager.SendCodeToPanel(device, 0x02, 0x11, DateConverter.ConvertToBytes(DateTime.Now));
        }

		public static int RomDBFirstIndex;
		public static int FlashDBLastIndex;

		public static List<byte> GetRomDBBytes(Device device)
		{
			var packetLenght = USBManager.IsUsbDevice(device) ? 0x33 : 0xFF;
			var response = USBManager.SendCodeToPanel(device, 0x38, BitConverter.GetBytes(RomDBFirstIndex).Reverse(), packetLenght);
			var result = response.Bytes;
			var romDBLastIndex = BytesHelper.ExtractTriple(response.Bytes, 9);

			var numberOfPackets = romDBLastIndex - RomDBFirstIndex / packetLenght;

			for (var i = RomDBFirstIndex + packetLenght + 1; i < romDBLastIndex; i += packetLenght + 1)
			{
				var packetNo = (i - RomDBFirstIndex) / packetLenght;
				CallbackManager.AddProgress(new FS2ProgressInfo("Чтение базы ROM " + packetNo + " из " + numberOfPackets));
				var length = Math.Min(packetLenght, romDBLastIndex - i);
				response = USBManager.SendCodeToPanel(device, 0x38, BitConverter.GetBytes(i).Reverse(), length);
				result.AddRange(response.Bytes);
			}
			return result;
		}

		public static List<byte> GetFlashDBBytes(Device device)
		{
			var packetLenght = USBManager.IsUsbDevice(device) ? 0x33 : 0xFF;
			var result = new List<byte>();

			var numberOfPackets = FlashDBLastIndex - 0x100 / packetLenght;

			for (var i = 0x100; i < FlashDBLastIndex; i += packetLenght + 1)
			{
				var packetNo = (i - FlashDBLastIndex) / packetLenght;
				CallbackManager.AddProgress(new FS2ProgressInfo("Чтение базы FLASH " + packetNo + " из " + numberOfPackets));
				var length = Math.Min(packetLenght, FlashDBLastIndex - i);
				var response = USBManager.SendCodeToPanel(device, 0x01, 0x52, BitConverter.GetBytes(i).Reverse(), length);
				result.AddRange(response.Bytes);
			}
			var nullbytes = new List<byte>();
			for (var i = 0; i < 0x100; i++)
				nullbytes.Add(0);
			result.InsertRange(0, nullbytes);
			return result;
		}

		public static int GetRomFirstIndex(Device device)
		{
			CallbackManager.AddProgress(new FS2ProgressInfo("Запрос размера ROM части базы"));
			var response = USBManager.SendCodeToPanel(device, 0x01, 0x57);
			return BytesHelper.ExtractTriple(response.Bytes, 1);
		}

		public static int GetFlashLastIndex(Device device)
		{
			CallbackManager.AddProgress(new FS2ProgressInfo("Запрос размера FLASH части базы"));
			var response = USBManager.SendCodeToPanel(device, 0x38, BitConverter.GetBytes(RomDBFirstIndex).Reverse(), 0x0B);
			return BytesHelper.ExtractTriple(response.Bytes, 6);
		}

		public static List<byte> GetBytesFromFlashDB(Device device, int pointer, int count)
		{
			return USBManager.SendCodeToPanel(device, 0x01, 0x52, BitConverter.GetBytes(pointer).Reverse(), count - 1).Bytes;
		}
    }
}