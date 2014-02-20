using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;
using System;
using System.Collections.Generic;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class DocumentsViewModel : ViewPartViewModel
	{
		public DocumentsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RefreshCommand = new RelayCommand(OnRefresh);
			EditFilterCommand = new RelayCommand(OnEditFilter);
			Filter = new DocumentFilter();
			Initialize();
		}

		DocumentFilter Filter;

		void Initialize()
		{
			Documents = new ObservableCollection<DocumentViewModel>();
			var operationResult = FiresecManager.FiresecService.GetDocuments(Filter);
			if (operationResult.HasError)
				MessageBoxService.ShowWarning(operationResult.Error);
			else
			{
				var documents = operationResult.Result;
				foreach (var document in documents)
					Documents.Add(new DocumentViewModel(document));
			}
			SelectedDocument = Documents.FirstOrDefault();
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}

		ObservableCollection<DocumentViewModel> _documents;
		public ObservableCollection<DocumentViewModel> Documents
		{
			get { return _documents; }
			set
			{
				_documents = value;
				OnPropertyChanged("Documents");
			}
		}

		DocumentViewModel _selectedDocument;
		public DocumentViewModel SelectedDocument
		{
			get { return _selectedDocument; }
			set
			{
				_selectedDocument = value;
				OnPropertyChanged("SelectedDocument");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var documentDetailsViewModel = new DocumentDetailsViewModel(this);
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var document = documentDetailsViewModel.Document;
				var documentViewModel = new DocumentViewModel(document);
				Documents.Add(documentViewModel);
				DocumentHelper.Save(document);
				SelectedDocument = documentViewModel;
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var index = Documents.IndexOf(SelectedDocument);
			DocumentHelper.MarkDeleted(SelectedDocument.Document);
			Documents.Remove(SelectedDocument);
			index = Math.Min(index, Documents.Count - 1);
			if (index > -1)
				SelectedDocument = Documents[index];
		}
		bool CanRemove()
		{
			return SelectedDocument != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var documentDetailsViewModel = new DocumentDetailsViewModel(this, SelectedDocument.Document);
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var document = documentDetailsViewModel.Document;
				SelectedDocument.Update(document);
				DocumentHelper.Save(document);
			}
		}
		bool CanEdit()
		{
			return SelectedDocument != null;
		}

		public RelayCommand EditFilterCommand { get; private set; }
		void OnEditFilter()
		{
			var documentFilterViewModel = new DocumentFilterViewModel(Filter);
			if (DialogService.ShowModalWindow(documentFilterViewModel))
			{
				Filter = documentFilterViewModel.Filter;
				Initialize();
			}
		}
	}
}