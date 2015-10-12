using System;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DocumentTypeDetailsViewModel : SaveCancelDialogViewModel
	{
		Guid OrganisationUID { get; set; }
		public TimeTrackDocumentType TimeTrackDocumentType { get; private set; }

		public DocumentTypeDetailsViewModel(Guid orgnaisationUID, TimeTrackDocumentType timeTrackDocumentType = null)
		{
			OrganisationUID = orgnaisationUID;
			if (timeTrackDocumentType == null)
			{
				Title = "Создание документа";
				TimeTrackDocumentType = new TimeTrackDocumentType()
				{
					Name = "Название документа",
					OrganisationUID = OrganisationUID
				};
			}
			else
			{
				TimeTrackDocumentType = timeTrackDocumentType;
				Title = string.Format("Свойства документа: {0}", TimeTrackDocumentType.Name);
			}

			AvailableDocumentTypes = new ObservableCollection<DocumentType>();
			AvailableDocumentTypes.Add(DocumentType.Overtime);
			AvailableDocumentTypes.Add(DocumentType.Presence);
			AvailableDocumentTypes.Add(DocumentType.Absence);

			CopyProperties();
		}

		public void CopyProperties()
		{
			Name = TimeTrackDocumentType.Name;
			ShortName = TimeTrackDocumentType.ShortName;
			Code = TimeTrackDocumentType.Code;
			SelectedDocumentType = AvailableDocumentTypes.FirstOrDefault(x => x == TimeTrackDocumentType.DocumentType);
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
					OnPropertyChanged(() => Name);
				}
			}
		}

		string _shortName;
		public string ShortName
		{
			get { return _shortName; }
			set
			{
				if (_shortName != value)
				{
					_shortName = value;
					OnPropertyChanged(() => ShortName);
				}
			}
		}

		int _code;
		public int Code
		{
			get { return _code; }
			set
			{
				if (_code != value)
				{
					_code = value;
					OnPropertyChanged(() => Code);
				}
			}
		}

		public ObservableCollection<DocumentType> AvailableDocumentTypes { get; private set; }

		DocumentType _selectedDocumentType;
		public DocumentType SelectedDocumentType
		{
			get { return _selectedDocumentType; }
			set
			{
				if (_selectedDocumentType != value)
				{
					_selectedDocumentType = value;
					OnPropertyChanged(() => SelectedDocumentType);
				}
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			if (string.IsNullOrEmpty(Name))
			{
				MessageBoxService.ShowWarning("Не задано название документа");
				return false;
			}
			if (string.IsNullOrEmpty(ShortName))
			{
				MessageBoxService.ShowWarning("Не задан буквенный код документа");
				return false;
			}
			if (Code <= 0)
			{
				MessageBoxService.ShowWarning("Числовой код документа должен быть положительным числом");
				return false;
			}
			if (TimeTrackDocumentTypesCollection.TimeTrackDocumentTypes.Any(x=>x.Name == Name))
			{
				MessageBoxService.ShowWarning("Название документа совпадает с одним из предопределенных");
				return false;
			}
			if (TimeTrackDocumentTypesCollection.TimeTrackDocumentTypes.Any(x => x.ShortName == ShortName))
			{
				MessageBoxService.ShowWarning("Буквенный код документа совпадает с одним из предопределенных кодов");
				return false;
			}
			if (Code <= 36)
			{
				MessageBoxService.ShowWarning("Числовой код документа совпадает с одним из предопределенных");
				return false;
			}

			var documentTypes = DocumentTypeHelper.GetByOrganisation(OrganisationUID);
			if (documentTypes.Any(x => x.Name == Name && x.UID != TimeTrackDocumentType.UID))
			{
				MessageBoxService.ShowWarning("Название документа совпадает с одним из ранее введенным");
				return false;
			}
			if (documentTypes.Any(x => x.ShortName == ShortName && x.UID != TimeTrackDocumentType.UID))
			{
				MessageBoxService.ShowWarning("Буквенный код документа совпадает с одним из ранее введенным");
				return false;
			}
			if (documentTypes.Any(x => x.Code == Code && x.UID != TimeTrackDocumentType.UID))
			{
				MessageBoxService.ShowWarning("Числовой код документа совпадает с одним из ранее введенным");
				return false;
			}
			if (Name.Length > 4000)
			{
				MessageBoxService.ShowWarning("Значение поля 'Название' не может быть длиннее 4000 символов");
				return false;
			}
			if (ShortName.Length > 50)
			{
				MessageBoxService.ShowWarning("Значение поля 'Сокращённое название' не может быть длиннее 50 символов");
				return false;
			}
			TimeTrackDocumentType.Name = Name;
			TimeTrackDocumentType.ShortName = ShortName;
			TimeTrackDocumentType.Code = Code;
			TimeTrackDocumentType.DocumentType = SelectedDocumentType;
			return true;
		}
	}
}