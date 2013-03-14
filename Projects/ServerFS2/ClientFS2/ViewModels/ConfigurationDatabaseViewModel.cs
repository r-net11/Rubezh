using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using ClientFS2.ConfigurationWriter;

namespace ClientFS2.ViewModels
{
	public class ConfigurationDatabaseViewModel : DialogViewModel
	{
		public ConfigurationDatabaseViewModel(List<PanelDatabase> panelDatabases)
		{
			Title = "Структура распределения памяти";
			PanelDatabases = panelDatabases;
		}

		public List<PanelDatabase> PanelDatabases { get; set; }
	}
}