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
		}

		public List<PanelDatabase> PanelDatabases { get; set; }
		public static BinaryConfigurationHelper BinaryConfigurationHelper { get; set; }

		public void Run()
		{
			var startDateTime = DateTime.Now;
			BinaryConfigurationHelper = new BinaryConfigurationHelper();

			var deltaMiliseconds = (DateTime.Now - startDateTime).TotalMilliseconds;
			Trace.WriteLine("ConfigurationWriterHelper Miliseconds=" + deltaMiliseconds.ToString());

			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.IsPanel)
				{
					var panelDatabase = new PanelDatabase(device);
					PanelDatabases.Add(panelDatabase);
				}
			}

			deltaMiliseconds = (DateTime.Now - startDateTime).TotalMilliseconds;
			Trace.WriteLine("ConfigurationWriterHelper Miliseconds=" + deltaMiliseconds.ToString());
		}
	}
}