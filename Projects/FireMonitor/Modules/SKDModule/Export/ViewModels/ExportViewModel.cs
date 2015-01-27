using System;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;

namespace SKDModule.ViewModels
{
	public class ExportViewModel : ViewPartViewModel
	{
		public ExportViewModel()
		{
			ExportOrganisationCommand = new RelayCommand(OnExportOrganisation);
			ImportOrganisationCommand = new RelayCommand(OnImportOrganisation);
			ExportJournalCommand = new RelayCommand(OnExportJournal);
			ExportPassJournalCommand = new RelayCommand(OnExportPassJournal);
		}

		public RelayCommand ExportOrganisationCommand { get; private set; }
		void OnExportOrganisation()
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter());
			if (organisations == null)
				return;
			var filter = new ExportFilter { OrganisationUID = organisations.FirstOrDefault().UID };
			OrganisationHelper.Export(filter);
		}


		public RelayCommand ImportOrganisationCommand { get; private set; }
		void OnImportOrganisation()
		{
			var openFileDialog = new OpenFileDialog() { Filter = "Zip files|*.zip" };
			if (openFileDialog.ShowDialog() == true)
				OrganisationHelper.Import(openFileDialog.FileName);
		}

		public RelayCommand ExportJournalCommand { get; private set; }
		void OnExportJournal()
		{
			var maxDate = DateTime.Now;
			var minDate = maxDate.AddDays(-1);
			PassJournalHelper.ExportJournal(minDate, maxDate);
		}

		public RelayCommand ExportPassJournalCommand { get; private set; }
		void OnExportPassJournal()
		{
			var maxDate = DateTime.Now;
			var minDate = maxDate.AddDays(-1);
			PassJournalHelper.ExportPassJournal(minDate, maxDate);
		}
	}
}
