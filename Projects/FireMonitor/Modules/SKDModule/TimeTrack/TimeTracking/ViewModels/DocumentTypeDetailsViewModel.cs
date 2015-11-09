using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DocumentTypeDetailsViewModel : SaveCancelDialogViewModel
	{
		#region Properties

		private IEnumerable<TimeTrackDocumentType> _systemTimeTrackDocumentTypes;
		private Model.DocumentType _inputDocumentType;

		private Guid OrganisationUID { get; set; }

		public TimeTrackDocumentType TimeTrackDocumentType { get; private set; }

		public bool IsReadOnly { get; private set; }

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

		public List<DocumentType> AvailableDocumentTypes { get; private set; }

		DocumentType _selectedDocumentType;
		public DocumentType SelectedDocumentType
		{
			get { return _selectedDocumentType; }
			set
			{
				if (_selectedDocumentType == value) return;
				_selectedDocumentType = value;
				OnPropertyChanged(() => SelectedDocumentType);
			}
		}

		#endregion

		public DocumentTypeDetailsViewModel(Model.DocumentType documentType, IEnumerable<TimeTrackDocumentType> systemTimeTrackDocumentTypes,  bool isEdit = false)
		{
			OrganisationUID = documentType.Organisation.UID;
			_inputDocumentType = documentType;
			_systemTimeTrackDocumentTypes = systemTimeTrackDocumentTypes;

			if (documentType.TimeTrackDocumentType == null || !isEdit)
			{
				Title = "Создание вида оправдательного документа";
				TimeTrackDocumentType = new TimeTrackDocumentType
				{
					Name = "Название документа",
					OrganisationUID = OrganisationUID
				};
			}
			else
			{
				TimeTrackDocumentType = documentType.TimeTrackDocumentType;
				Title = "Редактирование вида оправдательных документов";
				IsReadOnly = documentType.IsSystem;
			}

			AvailableDocumentTypes = Enum.GetValues(typeof(DocumentType)).Cast<DocumentType>().ToList();

			CopyProperties();
		}

		public void CopyProperties()
		{
			Name = TimeTrackDocumentType.Name;
			ShortName = TimeTrackDocumentType.ShortName;
			Code = TimeTrackDocumentType.Code;
			SelectedDocumentType = AvailableDocumentTypes.FirstOrDefault(x => x == TimeTrackDocumentType.DocumentType);
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			if (!_inputDocumentType.IsSystem)
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

				if (_systemTimeTrackDocumentTypes.Any(x => x.Name == Name))
				{
					MessageBoxService.ShowWarning("Название документа совпадает с одним из предопределенных");
					return false;
				}
				if (_systemTimeTrackDocumentTypes.Any(x => x.ShortName == ShortName))
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
			}
			TimeTrackDocumentType.Name = Name;
			TimeTrackDocumentType.ShortName = ShortName;
			TimeTrackDocumentType.Code = Code;
			TimeTrackDocumentType.DocumentType = SelectedDocumentType;
			return true;
		}
	}
}