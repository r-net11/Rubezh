using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class ConfigurationWriterHelper2
	{
		public ConfigurationWriterHelper2()
		{
			PanelDatabases = new List<PanelDatabase>();
		}

		public List<PanelDatabase> PanelDatabases { get; set; }

		public void Run()
		{
			ZonePanelRelations.Initialize();
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.IsPanel)
				{
					var panelDatabase = new PanelDatabase(device);
					PanelDatabases.Add(panelDatabase);
				}
			}
		}
	}
}