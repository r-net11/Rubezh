using Caliburn.Micro;
using Common;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SKDModule.ViewModels
{
	public class DocumentTypeDetailsViewModel : SaveCancelDialogViewModel
	{
		#region Properties

		private readonly Model.DocumentType _inputDocumentType;
		private readonly IEnumerable<TimeTrackDocumentType> _inputDocumentTypesForOrganisation;

		private bool IsEdit { get; set; }

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

		public DocumentTypeDetailsViewModel(Model.DocumentType documentType, IEnumerable<TimeTrackDocumentType> documentTypesForOrganisation, bool isEdit = false)
		{
			OrganisationUID = documentType.Organisation.UID;
			_inputDocumentType = documentType;
			_inputDocumentTypesForOrganisation = documentTypesForOrganisation;
			IsEdit = isEdit;

			if (documentType.TimeTrackDocumentType == null || !IsEdit)
			{
				Title = "Новый вид документа";
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
				IsReadOnly = documentType.TimeTrackDocumentType.IsSystem;
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
			if (!ExecuteValidation()) return false;

			TimeTrackDocumentType.Name = Name;
			TimeTrackDocumentType.ShortName = ShortName;
			TimeTrackDocumentType.Code = Code;
			TimeTrackDocumentType.DocumentType = SelectedDocumentType;
			return true;
		}

		private bool BaseValidation()
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

			return true;
		}

		private bool ExecuteValidation()
		{
			if (!IsEdit)
			{
				if (BaseValidation())
				{
					if (!_inputDocumentTypesForOrganisation.IsEmpty() &&
					    _inputDocumentTypesForOrganisation.Any(x => x.Name == Name))
					{
						MessageBoxService.ShowWarning("Название документа совпадает с одним из предопределенных");
						return false;
					}
					if (!_inputDocumentTypesForOrganisation.IsEmpty() &&
					    _inputDocumentTypesForOrganisation.Any(x => x.ShortName == ShortName))
					{
						MessageBoxService.ShowWarning("Буквенный код документа совпадает с одним из предопределенных кодов");
						return false;
					}
					if (!_inputDocumentTypesForOrganisation.IsEmpty() &&
					    _inputDocumentTypesForOrganisation.Any(x => x.Code == Code))
					{
						MessageBoxService.ShowWarning("Числовой код документа совпадает с одним из предопределенных");
						return false;
					}
				}

				else return false;
			}
			else if (IsEdit && _inputDocumentType != null && _inputDocumentType.TimeTrackDocumentType != null &&
			         !_inputDocumentType.TimeTrackDocumentType.IsSystem)
			{
				if (BaseValidation())
				{
					if (!_inputDocumentTypesForOrganisation.IsEmpty() &&
					    _inputDocumentTypesForOrganisation.Where(x => x.UID != _inputDocumentType.UID).Any(x => x.Name == Name))
					{
						MessageBoxService.ShowWarning("Название документа совпадает с одним из предопределенных");
						return false;
					}
					if (!_inputDocumentTypesForOrganisation.IsEmpty() &&
					    _inputDocumentTypesForOrganisation.Where(x => x.UID != _inputDocumentType.UID)
						    .Any(x => x.ShortName == ShortName))
					{
						MessageBoxService.ShowWarning("Буквенный код документа совпадает с одним из предопределенных кодов");
						return false;
					}
					if (!_inputDocumentTypesForOrganisation.IsEmpty() &&
					    _inputDocumentTypesForOrganisation.Where(x => x.UID != _inputDocumentType.UID).Any(x => x.Code == Code))
					{
						MessageBoxService.ShowWarning("Числовой код документа совпадает с одним из предопределенных");
						return false;
					}
				}
				else return false;
			}
			else return false;

			return true;
		}
	}
}