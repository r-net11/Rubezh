using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using System.Diagnostics;

namespace ClientFS2.ConfigurationWriter
{
	public class ConfigurationWriterHelper
	{
		public ConfigurationWriterHelper()
		{
			PanelDatabases = new List<PanelDatabase>();
			BIDatabases = new List<SingleTable>();
		}

		public List<PanelDatabase> PanelDatabases { get; set; }
		public List<SingleTable> BIDatabases { get; set; }

		public static BinaryConfigurationHelper BinaryConfigurationHelper { get; set; }

		public void Run()
		{
			var startDateTime = DateTime.Now;
			BinaryConfigurationHelper = new BinaryConfigurationHelper();

			var deltaMiliseconds = (DateTime.Now - startDateTime).TotalMilliseconds;
			Trace.WriteLine("ConfigurationWriterHelper Miliseconds=" + deltaMiliseconds.ToString());

			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				switch (device.Driver.DriverType)
				{
					case DriverType.Rubezh_2AM:
					case DriverType.Rubezh_4A:
					case DriverType.Rubezh_2OP:
					case DriverType.BUNS:
					case DriverType.BUNS_2:
						var panelDatabase = new PanelDatabase(device);
						PanelDatabases.Add(panelDatabase);
						break;
				}
			}

			BytesHelper.IsRevese = true;
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				switch (device.Driver.DriverType)
				{
					case DriverType.IndicationBlock:
						var biDatabase = new BIDatabase(device, this);
						BIDatabases.Add(biDatabase);
						break;

					case DriverType.PDU:
						var pduDatabase = new PDUDatabase(device, this);
						BIDatabases.Add(pduDatabase);
						break;

					case DriverType.PDU_PT:
						var pduPTDatabase = new PDUPTDatabase(device, this);
						BIDatabases.Add(pduPTDatabase);
						break;
				}
			}
			BytesHelper.IsRevese = false;

			deltaMiliseconds = (DateTime.Now - startDateTime).TotalMilliseconds;
			Trace.WriteLine("ConfigurationWriterHelper Miliseconds=" + deltaMiliseconds.ToString());
		}
	}
}