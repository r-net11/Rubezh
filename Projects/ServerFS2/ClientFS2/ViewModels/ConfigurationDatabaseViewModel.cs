using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using ClientFS2.ConfigurationWriter;
using System.Collections.ObjectModel;

namespace ClientFS2.ViewModels
{
	public class ConfigurationDatabaseViewModel : DialogViewModel
	{
		public List<PanelDatabase> PanelDatabases { get; set; }
		public List<SingleTable> BIDatabases { get; set; }

		public ConfigurationDatabaseViewModel(ConfigurationWriterHelper configurationWriterHelper)
		{
			Title = "Структура распределения памяти";
			PanelDatabases = configurationWriterHelper.PanelDatabases;
			BIDatabases = configurationWriterHelper.BIDatabases;
		}
	}
}