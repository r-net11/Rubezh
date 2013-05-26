using System.Collections.Generic;
using FiresecAPI.Models;

namespace ServerFS2.ConfigurationWriter
{
	public class SystemDatabaseCreator
	{
		public List<PanelDatabase> PanelDatabases { get; set; }
		public List<NonPanelDatabase> NonPanelDatabases { get; set; }
		public static BinaryConfigurationHelper BinaryConfigurationHelper { get; set; }

		public SystemDatabaseCreator()
		{
			PanelDatabases = new List<PanelDatabase>();
			NonPanelDatabases = new List<NonPanelDatabase>();
		}

		public void Run()
		{
			BinaryConfigurationHelper = new BinaryConfigurationHelper();

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
						NonPanelDatabases.Add(biDatabase);
						break;

					case DriverType.PDU:
						var pduDatabase = new PDUDatabase(device, this);
						NonPanelDatabases.Add(pduDatabase);
						break;

					case DriverType.PDU_PT:
						var pduPTDatabase = new PDUPTDatabase(device, this);
						NonPanelDatabases.Add(pduPTDatabase);
						break;
				}
			}
			BytesHelper.IsRevese = false;
		}
	}
}