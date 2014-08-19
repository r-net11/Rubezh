using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.SKD;
using FiresecClient;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class DocumentsViewModel : BaseViewModel
	{
		public ShortEmployee ShortEmployee { get; private set; }

		public DocumentsViewModel(ShortEmployee shortEmployee, DateTime startDate, DateTime endDate)
		{
			ShortEmployee = shortEmployee;
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);

			Documents = new ObservableCollection<TimeTrackDocument>();
			var operationResult = FiresecManager.FiresecService.GetTimeTrackDocument(shortEmployee.UID, startDate, endDate);
			if (!operationResult.HasError)
			{
				foreach (var timeTrackDocument in operationResult.Result)
				{
					Documents.Add(timeTrackDocument);
				}
			}
			SelectedDocument = Documents.FirstOrDefault();
		}

		public ObservableCollection<TimeTrackDocument> Documents { get; private set; }

		TimeTrackDocument _selectedDocument;
		public TimeTrackDocument SelectedDocument
		{
			get { return _selectedDocument; }
			set
			{
				_selectedDocument = value;
				OnPropertyChanged(() => SelectedDocument);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var documentDetailsViewModel = new DocumentDetailsViewModel();
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var operationResult = FiresecManager.FiresecService.AddTimeTrackDocument(documentDetailsViewModel.TimeTrackDocument);
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var documentDetailsViewModel = new DocumentDetailsViewModel(SelectedDocument);
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var operationResult = FiresecManager.FiresecService.EditTimeTrackDocument(documentDetailsViewModel.TimeTrackDocument);
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
			}
		}
		bool CanEdit()
		{
			return SelectedDocument != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var operationResult = FiresecManager.FiresecService.RemoveTimeTrackDocument(SelectedDocument.UID);
			if (operationResult.HasError)
			{
				MessageBoxService.ShowWarning(operationResult.Error);
			}
		}
		bool CanRemove()
		{
			return SelectedDocument != null;
		}
	}
}