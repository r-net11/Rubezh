using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class DocumentDetailsViewModel : SaveCancelDialogViewModel
	{
		DocumentsViewModel DocumentsViewModel;
		public Document Document { get; private set; }

		public DocumentDetailsViewModel(DocumentsViewModel documentsViewModel, Document document = null)
		{
			DocumentsViewModel = documentsViewModel;
			if (document == null)
			{
				Title = "Создание документа";
				document = new Document()
				{
					Name = "Новый документ",
				};
			}
			else
			{
				Title = string.Format("Свойства документа: {0}", document.Name);
			}
			Document = document;
			CopyProperties();
		}

		public void CopyProperties()
		{
			Name = Document.Name;
			Description = Document.Description;
			No = Document.No;
			if (Document.IssueDate.HasValue)
				StartDateTime = Document.IssueDate.Value;
			if (Document.LaunchDate.HasValue)
				EndDateTime = Document.LaunchDate.Value;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged("Name");
				}
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				if (_description != value)
				{
					_description = value;
					OnPropertyChanged("Description");
				}
			}
		}

		int _no;
		public int No
		{
			get { return _no; }
			set
			{
				if (_no != value)
				{
					_no = value;
					OnPropertyChanged("No");
				}
			}
		}

		DateTime _startDateTime;
		public DateTime StartDateTime
		{
			get { return _startDateTime; }
			set
			{
				if (_startDateTime != value)
				{
					_startDateTime = value;
					OnPropertyChanged("StartDateTime");
				}
			}
		}

		DateTime _endDateTime;
		public DateTime EndDateTime
		{
			get { return _endDateTime; }
			set
			{
				if (_endDateTime != value)
				{
					_endDateTime = value;
					OnPropertyChanged("EndDateTime");
				}
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			if (DocumentsViewModel.Documents.Any(x => x.Document.Name == Name && x.Document.UID != Document.UID))
			{
				MessageBoxService.ShowWarning("Серия и номер карты совпадает с введеннымы ранее");
				return false;
			}

			Document = new Document()
			{
				Name = Name,
				Description = Description,
				No = No,
				IssueDate = StartDateTime,
				LaunchDate = EndDateTime
			};
			return true;
		}
	}
}