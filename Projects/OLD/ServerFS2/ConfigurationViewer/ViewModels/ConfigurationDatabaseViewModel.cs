using System.Collections.Generic;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2.ConfigurationWriter;

namespace ConfigurationViewer.ViewModels
{
	public class ConfigurationDatabaseViewModel : DialogViewModel
	{
		public List<PanelDatabaseViewModel> PanelDatabases { get; set; }
		public List<NonPanelDatabaseViewModel> NonPanelDatabases { get; set; }

		public ConfigurationDatabaseViewModel(SystemDatabaseCreator configurationWriterHelper)
		{
			Title = "Структура распределения памяти";

			PanelDatabases = new List<PanelDatabaseViewModel>();
			foreach (var panelDatabase in configurationWriterHelper.PanelDatabases)
			{
				PanelDatabases.Add(new PanelDatabaseViewModel(panelDatabase));
			}

			NonPanelDatabases = new List<NonPanelDatabaseViewModel>();
			foreach (var nonPanelDatabase in configurationWriterHelper.NonPanelDatabases)
			{
				NonPanelDatabases.Add(new NonPanelDatabaseViewModel(nonPanelDatabase));
			}
		}
	}
}