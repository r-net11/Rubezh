using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;
using System;
using System.Collections.Generic;

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
			Initialize();
		}

		void Initialize()
		{
			Documents = new ObservableCollection<DocumentViewModel>();
			//var documents = FiresecManager.GetDocuments(null);
			var documents = new List<Document>();
			foreach (var document in documents)
			{
				var documentViewModel = new DocumentViewModel(document);
				Documents.Add(documentViewModel);
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
				SelectedDocument = documentViewModel;
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var index = Documents.IndexOf(SelectedDocument);
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
				SelectedDocument.Update(documentDetailsViewModel.Document);
			}
		}
		bool CanEdit()
		{
			return SelectedDocument != null;
		}
	}
}