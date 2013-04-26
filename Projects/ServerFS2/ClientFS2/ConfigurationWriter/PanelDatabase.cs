using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using System.Diagnostics;
using System.IO;

namespace ClientFS2.ConfigurationWriter
{
    public class PanelDatabase
    {
        public PanelDatabase1 PanelDatabase1 { get; set; }
        public PanelDatabase2 PanelDatabase2 { get; set; }
        public Device ParentPanel { get; set; }

        static double Total_Miliseconds;

        public PanelDatabase(Device parentDevice)
        {
            var startDateTime = DateTime.Now;

            ParentPanel = parentDevice;
            PanelDatabase2 = new PanelDatabase2(parentDevice);
            //PanelDatabase1 = new PanelDatabase1(PanelDatabase2, 12288);
			PanelDatabase1 = new PanelDatabase1(PanelDatabase2, 0x2000);
            Trace.WriteLine("PanelDatabase Done");

            var deltaMiliseconds = (DateTime.Now - startDateTime).TotalMilliseconds;
            Total_Miliseconds += deltaMiliseconds;
            Trace.WriteLine("PanelDatabase Total_Miliseconds=" + Total_Miliseconds.ToString());

            MergeDatabase1();
            MergeDatabase2();
        }

        void MergeDatabase2()
        {
            foreach (var byteDescription in PanelDatabase2.BytesDatabase.ByteDescriptions)
            {
                byteDescription.IsNotEqualToOriginal = true;
            }

			var fileName = @"C:\Program Files\Firesec\TstData\Рубеж-2AM - 1.1_flash.bin"; //"DeviceRom.bin";
            if (File.Exists(fileName))
            {
                var byteArray = File.ReadAllBytes(fileName);
                if (byteArray != null)
                {
                    var bytes = byteArray.ToList();
                    var emptyBytes = new List<byte>();
					//for (int i = 0; i < 256; i++)
					//{
					//    emptyBytes.Add(0);
					//}
                    bytes.InsertRange(0, emptyBytes);

                    foreach (var byteDescription in PanelDatabase2.BytesDatabase.ByteDescriptions)
                    {
                        if (bytes.Count > byteDescription.Offset)
                        {
                            var originalByte = bytes[byteDescription.Offset];
                            byteDescription.OriginalValue = originalByte;
                            if (byteDescription.Value == originalByte)
                                byteDescription.IsNotEqualToOriginal = false;
                        }
                    }
                }
            }
        }

        void MergeDatabase1()
        {
            foreach (var byteDescription in PanelDatabase1.BytesDatabase.ByteDescriptions)
            {
                byteDescription.IsNotEqualToOriginal = true;
            }

			var fileName = @"C:\Program Files\Firesec\TstData\Рубеж-2AM - 1.1_rom.bin";  //"DeviceRam.bin";
            if (File.Exists(fileName))
            {
                var byteArray = File.ReadAllBytes(fileName);
                if (byteArray != null)
                {
                    var bytes = byteArray.ToList();
					//bytes.RemoveRange(0, 0x2000);
					//var emptyBytes = new List<byte>();
					////for (int i = 0; i < 12288; i++)
					//for (int i = 0; i < 0x2000; i++)
					//{
					//    emptyBytes.Add(0);
					//}
					//bytes.InsertRange(0, emptyBytes);

                    foreach (var byteDescription in PanelDatabase1.BytesDatabase.ByteDescriptions)
                    {
                        if (bytes.Count > byteDescription.Offset)
                        {
                            var originalByte = bytes[byteDescription.Offset];
                            byteDescription.OriginalValue = originalByte;
                            if (byteDescription.Value == originalByte)
                                byteDescription.IsNotEqualToOriginal = false;
                        }
                    }
                }
            }
        }
    }
}