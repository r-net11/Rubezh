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
			PanelDatabase1 = new PanelDatabase1(PanelDatabase2, 12288);
			Trace.WriteLine("PanelDatabase Done");

			var deltaMiliseconds = (DateTime.Now - startDateTime).TotalMilliseconds;
			Total_Miliseconds += deltaMiliseconds;
			Trace.WriteLine("PanelDatabase Total_Miliseconds=" + Total_Miliseconds.ToString());

			foreach (var byteDescription in PanelDatabase2.BytesDatabase.ByteDescriptions)
			{
				byteDescription.IsNotEqualToOriginal = true;
			}

			var fileName = "FSBytes.bin";
			if (File.Exists(fileName))
			{
				var byteArray = File.ReadAllBytes(fileName);
				if (byteArray != null)
				{
					var bytes = byteArray.ToList();

					foreach (var byteDescription in PanelDatabase2.BytesDatabase.ByteDescriptions)
					{
						if (bytes.Count > byteDescription.Offset)
						{
							var b = bytes[byteDescription.Offset];
							if (byteDescription.Value == b)
								byteDescription.IsNotEqualToOriginal = false;
						}
					}
				}
			}
		}
	}
}