using System;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DocumentDetailsViewModel : SaveCancelDialogViewModel
	{
		DocumentsViewModel DocumentsViewModel;
		public Document Document { get; private set; }
		public ShortDocument ShortDocument
		{
			get
			{
				return new ShortDocument
				{
					UID = Document.UID,
					Name = Document.Name,
					Description = Document.Description,
					OrganisationUID = Document.OrganisationUID
				};
			}
		}

		public Organisation Organisation { get; private set; }

		public DocumentDetailsViewModel(DocumentsViewModel documentsViewModel, Organisation orgnaisation, Guid? documentUID = null)
		{
			DocumentsViewModel = documentsViewModel;
			Organisation = orgnaisation;
			if (documentUID == null)
			{
				Title = "Создание документа";
				Document = new Document()
				{
					Name = "Новый документ",
					OrganisationUID = Organisation.UID
				};
			}
			else
			{
				Document = DocumentHelper.GetDetails(documentUID);
				Title = string.Format("Свойства документа: {0}", Document.Name);
			}
			CopyProperties();
		}

		public void CopyProperties()
		{
			Name = Document.Name;
			Description = Document.Description;
			No = Document.No;
			StartDateTime = Document.IssueDate;
			EndDateTime = Document.LaunchDate;
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
			Document.Name = Name;
			Document.Description = Description;
			Document.No = No;
			Document.IssueDate = StartDateTime;
			Document.LaunchDate = EndDateTime;
			Document.OrganisationUID = Organisation.UID;
			return true;
		}
	}
}