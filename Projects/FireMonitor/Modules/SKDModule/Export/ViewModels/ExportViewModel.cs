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
			ImportJournalCommand = new RelayCommand(OnImportJournal);
			ExportPassJournalCommand = new RelayCommand(OnExportPassJournal);
			ImportPassJournalCommand = new RelayCommand(OnImportPassJournal);
		}

		public RelayCommand ExportOrganisationCommand { get; private set; }
		void OnExportOrganisation()
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter());
			if (organisations == null)
				return;
			OrganisationHelper.Export(organisations.FirstOrDefault().UID);
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
			PassJournalHelper.ExportJournal();
		}

		public RelayCommand ImportJournalCommand { get; private set; }
		void OnImportJournal()
		{
			var openFileDialog = new OpenFileDialog() { Filter = "Xml files|*.xml" };
			if (openFileDialog.ShowDialog() == true)
				PassJournalHelper.ImportJournal(openFileDialog.FileName);
		}

		public RelayCommand ExportPassJournalCommand { get; private set; }
		void OnExportPassJournal()
		{
			PassJournalHelper.ExportPassJournal();
		}

		public RelayCommand ImportPassJournalCommand { get; private set; }
		void OnImportPassJournal()
		{
			var openFileDialog = new OpenFileDialog() { Filter = "Xml files|*.xml" };
			if (openFileDialog.ShowDialog() == true)
				PassJournalHelper.ImportPassJournal(openFileDialog.FileName);
		}
	}
}
