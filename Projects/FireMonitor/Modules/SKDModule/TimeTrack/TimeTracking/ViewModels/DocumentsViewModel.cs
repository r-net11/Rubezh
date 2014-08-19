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

			Documents = new ObservableCollection<DocumentViewModel>();
			var operationResult = FiresecManager.FiresecService.GetTimeTrackDocument(shortEmployee.UID, startDate, endDate);
			if (!operationResult.HasError)
			{
				foreach (var document in operationResult.Result)
				{
					var documentViewModel = new DocumentViewModel(document);
					Documents.Add(documentViewModel);
				}
			}
			SelectedDocument = Documents.FirstOrDefault();
		}

		public ObservableCollection<DocumentViewModel> Documents { get; private set; }

		DocumentViewModel _selectedDocument;
		public DocumentViewModel SelectedDocument
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
				var document = documentDetailsViewModel.TimeTrackDocument;
				document.EmployeeUID = ShortEmployee.UID;
				var operationResult = FiresecManager.FiresecService.AddTimeTrackDocument(document);
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
				else
				{
					var documentViewModel = new DocumentViewModel(document);
					Documents.Add(documentViewModel);
					SelectedDocument = documentViewModel;
				}
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var documentDetailsViewModel = new DocumentDetailsViewModel(SelectedDocument.Document);
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var document = documentDetailsViewModel.TimeTrackDocument;
				document.EmployeeUID = ShortEmployee.UID;
				var operationResult = FiresecManager.FiresecService.EditTimeTrackDocument(document);
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
				SelectedDocument.Update();
			}
		}
		bool CanEdit()
		{
			return SelectedDocument != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var operationResult = FiresecManager.FiresecService.RemoveTimeTrackDocument(SelectedDocument.Document.UID);
			if (operationResult.HasError)
			{
				MessageBoxService.ShowWarning(operationResult.Error);
			}
			else
			{
				Documents.Remove(SelectedDocument);
			}
		}
		bool CanRemove()
		{
			return SelectedDocument != null;
		}
	}
}