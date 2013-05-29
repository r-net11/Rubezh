using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Device = FiresecAPI.Models.Device;

namespace ServerFS2
{
	public static partial class ServerHelper
	{
	    private static List<byte> _crc;
        private static void SetRomConfig(Device device, List<byte> deviceRom)
		{
            deviceRom.RemoveRange(0, 0x100);
		    _crc = new List<byte>(){deviceRom[0], deviceRom[1]};
		    deviceRom[0] = 0;
		    deviceRom[1] = 0;
		    var bytes = CreateBytesArray(device.Parent.IntAddress + 2,
			device.AddressOnShleif, 0x02, 0x52, BitConverter.GetBytes(0x100).Reverse(), 0x96, deviceRom);
			SendCode(bytes);
            bytes = CreateBytesArray(device.Parent.IntAddress + 2,
            device.AddressOnShleif, 0x02, 0x52, BitConverter.GetBytes(0x100).Reverse(), 0x01, _crc[0], _crc[1]);
			SendCode(bytes);
        }

        private static void SetFlashConfig(Device device, List<byte> deviceFlash)
		{
			var begin = RomDBFirstIndex / 0x100;
            List<byte> bytes;
            for (int i = 0; i < deviceFlash.Count / 256; i++)
            {
                bytes = CreateBytesArray(device.Parent.IntAddress + 2,
                device.AddressOnShleif, 0x3E, BitConverter.GetBytes((begin + i) * 0x100).Reverse(), deviceFlash.GetRange(i*256, 256));
				SendCode(bytes);
            }
			SendCode(CreateBytesArray(0x1C, 0x82, 0xA9, 0x7A, 0x04, 0x01, 0x3D, 0x1E, 0x00, 0x00, 0x36, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x36, 0x74, 0x00, 0x00, 0x01, 0x00, 0x01, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x36, 0x77, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x01 ,0x3B ,0x01 ,0x00 ,0x20 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00
            ,0x00 ,0x00 ,0x01 ,0x02 ,0x00 ,0x01 ,0x5B ,0x00 ,0x01 ,0x67));
		}

        public static void SetDeviceConfig(Device device, List<byte> Rom, List<byte> Flash)
        {
            var str = File.ReadAllText(@"C:\Documents and Settings\KazakovRB\Мои документы\TraceManual\Запись конфигурации\SetConfigTrace(2device BD).txt");
            str = str.Replace("\r", "");
            str = str.Replace("\n", "");
            var bytes = str.Split().Select(t => byte.Parse(t, NumberStyles.AllowHexSpecifier)).ToList();
            var localbytes = new List<byte>();
            var begin = false;
            foreach (var b in bytes)
            {
                if (b == 0x7E)
                {
                    begin = true;
                    localbytes = new List<byte>();
                }

                if (begin)
                {
                    if (b == 0x3E)
                    {
                        localbytes.Add(b);
                        begin = false;
                        if ((localbytes[1] == 0x61) && (localbytes[2] == 0x53))
                        {

                        }
                        localbytes = CreateInputBytes(localbytes);
                        Trace.WriteLine(String.Join(" ", localbytes.Select(p => p.ToString("X2")).ToArray()));
                        SendCode(localbytes);
                        continue;
                    }
                    localbytes.Add(b);
                }

            }

            //GetDeviceFlashFirstAndRomLastIndex(device, false);
            //SendCode(CreateBytesArray(0xDD, 0x81, 0xA9, 0x7A, 0x01, 0x02, 0x34, 0x01));
            //SendCode(CreateBytesArray(0xDE, 0x81, 0xA9, 0x7A, 0x01, 0x02, 0x37));
            //SendCode(CreateBytesArray(0xE9, 0x81, 0xA9, 0x7A, 0x04, 0x01, 0x02, 0x55, 0x01));
            //SetRomConfig(device, Rom);
            ////SendCode(CreateBytesArray(0xF2, 0x81, 0xA9, 0x7A, 0x04, 0x01, 0x39, 0x01));
            //ClearSector(device);
            //ConfirmationLongTermOperation(device);
            //SetFlashConfig(device, Flash);
            //SendCode(CreateBytesArray(0x1D, 0x82, 0xA9, 0x7A, 0x04, 0x01, 0x3A));
            //SendCode(CreateBytesArray(0x1E, 0x82, 0xA9, 0x7A, 0x04, 0x01, 0x3C));
        }

        static List<byte> CreateInputBytes(List<byte> messageBytes)
        {
            var bytes = new List<byte>();
            var previousByte = new byte();
            messageBytes.RemoveRange(0, messageBytes.IndexOf(0x7E) + 1);
            messageBytes.RemoveRange(messageBytes.IndexOf(0x3E), messageBytes.Count - messageBytes.IndexOf(0x3E));
            foreach (var b in messageBytes)
            {
                if ((b == 0x7D) || (b == 0x3D))
                { previousByte = b; continue; }
                if (previousByte == 0x7D)
                {
                    previousByte = new byte();
                    if (b == 0x5E)
                    { bytes.Add(0x7E); continue; }
                    if (b == 0x5D)
                    { bytes.Add(0x7D); continue; }
                }
                if (previousByte == 0x3D)
                {
                    previousByte = new byte();
                    if (b == 0x1E)
                    { bytes.Add(0x3E); continue; }
                    if (b == 0x1D)
                    { bytes.Add(0x3D); continue; }
                }
                bytes.Add(b);
            }
            return bytes;
        }

        // Очистка сектора памяти bSectorStart, bSectorEnd
        private static void ClearSector(Device device)
        {
            var bytes = CreateBytesArray(device.Parent.IntAddress + 2,
            device.AddressOnShleif, 0x3B, 0x03, 0x04);
			SendCode(bytes);
        }

        // Подтверждение / завершение долговременной операции
        private static void ConfirmationLongTermOperation(Device device)
        {
            var bytes = CreateBytesArray(device.Parent.IntAddress + 2,
            device.AddressOnShleif, 0x3C);
			SendCode(bytes);
        }
	}
}