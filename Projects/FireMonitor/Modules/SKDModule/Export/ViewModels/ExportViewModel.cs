using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

namespace SKDModule.ViewModels
{
	public class ExportViewModel : ViewPartViewModel
	{
		public ExportViewModel()
		{
			ExportOrganisationCommand = new RelayCommand(OnExportOrganisation);
			ImportOrganisationCommand = new RelayCommand(OnImportOrganisation);
			ExportJournalCommand = new RelayCommand(OnExportJournal);
			ExportConfigurationCommand = new RelayCommand(OnExportConfiguration);

			FilterFolders = new ObservableCollection<ExportFilterListItem>();
			ExportsFolder = new ExportFilterListItem { Name = "Экспорт" };
			ImportsFolder = new ExportFilterListItem { Name = "Импорт" };
			FilterFolders.Add(ExportsFolder);
			FilterFolders.Add(ImportsFolder);
			//var f1 = new ExportFilter { Name = "Тест" };
			//ExportsFolder.AddChild(new ExportFilterListItem(f1));
		}

		public ObservableCollection<ExportFilterListItem> FilterFolders { get; private set; }
		ExportFilterListItem ExportsFolder;
		ExportFilterListItem ImportsFolder;

		ExportFilterListItem _SelectedFilter;
		public ExportFilterListItem SelectedFilter
		{
			get { return _SelectedFilter; }
			set
			{
				_SelectedFilter = value;
				OnPropertyChanged(() => _SelectedFilter);
			}
		}
		
		public RelayCommand ExportOrganisationCommand { get; private set; }
		void OnExportOrganisation()
		{
			try
			{
				var folderBrowserDialog = new FolderBrowserDialog();
				folderBrowserDialog.ShowDialog();
				var organisations = OrganisationHelper.Get(new OrganisationFilter());
				if (organisations == null)
					return;
				var filter = new ExportFilter { OrganisationUID = organisations.FirstOrDefault().UID, Path = folderBrowserDialog.SelectedPath };
				ExportHelper.ExportOrganisation(filter);
			}
			catch (Exception e)
			{
				MessageBoxService.ShowWarning(e.Message);
			}
		}

		public RelayCommand ImportOrganisationCommand { get; private set; }
		void OnImportOrganisation()
		{
			try
			{
				var folderBrowserDialog = new FolderBrowserDialog();
				folderBrowserDialog.ShowDialog();
				ExportHelper.ImportOrganisation(folderBrowserDialog.SelectedPath);
			}
			catch (Exception e)
			{
				MessageBoxService.ShowWarning(e.Message);
			}
		}

		public RelayCommand ExportJournalCommand { get; private set; }
		void OnExportJournal()
		{
			var folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.ShowDialog();
			var maxDate = DateTime.Now;
			var minDate = maxDate.AddDays(-1);
			var filter = new JournalExportFilter { MinDate = minDate, MaxDate = maxDate, IsExportJournal = true, IsExportPassJournal = true, Path = folderBrowserDialog.SelectedPath };
			ExportHelper.ExportJournal(filter);
		}

		public RelayCommand ExportConfigurationCommand { get; private set; }
		void OnExportConfiguration()
		{
			var folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.ShowDialog();
			var filter = new ConfigurationExportFilter { IsExportDevices = true, IsExportDoors = true, IsExportZones = true, Path = folderBrowserDialog.SelectedPath };
			ExportHelper.ExportConfiguration(filter);
		}
	}

	public class ExportFilterListItem: TreeNodeViewModel<ExportFilterListItem>
	{
		public string Name { get; set; }
		public IExportFilter Filter { get; private set; }
		public bool IsExportFilter { get { return Filter is ExportFilter; } }
		public bool IsConfigurationFilter { get { return Filter is ConfigurationExportFilter; } }
		public bool IsJournalFilter { get { return Filter is JournalExportFilter; } }
		public bool IsImportFilter { get { return Filter is ExportFilter; } }

		public ExportFilterListItem() { }
		
		public ExportFilterListItem(IExportFilter filter)
		{
			Filter = filter;
			Name = filter.Name;
		}
	}
}
