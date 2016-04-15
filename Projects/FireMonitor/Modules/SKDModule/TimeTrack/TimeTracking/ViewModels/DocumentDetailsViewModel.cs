using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SKDModule.ViewModels
{
	public class DocumentDetailsViewModel : SaveCancelDialogViewModel
	{
		private bool _isNew = true;

		#region Properties

		public ICollectionView AvailableDocumentsCollectionView { get; private set; }
		public TimeTrackDocument TimeTrackDocument { get; private set; }

		private bool _isEnableAbsence;

		public bool IsEnableAbsence
		{
			get { return _isEnableAbsence; }
			set
			{
				if (_isEnableAbsence == value) return;
				_isEnableAbsence = value;
				OnPropertyChanged(() => IsEnableAbsence);
			}
		}

		private bool _isOutside;

		public bool IsOutside
		{
			get { return _isOutside; }
			set
			{
				_isOutside = value;
				OnPropertyChanged(() => IsOutside);
			}
		}

		public ObservableCollection<DocumentType> DocumentsTypes { get; private set; }

		private DocumentType _selectedDocumentType;

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

		DateTime _startDateTime;
		public DateTime StartDateTime
		{
			get { return _startDateTime; }
			set
			{
				_startDateTime = value;
				OnPropertyChanged(() => StartDateTime);
			}
		}

		public bool CanEditStartDateTime { get; private set; }

		TimeSpan _startTime;
		public TimeSpan StartTime
		{
			get { return _startTime; }
			set
			{
				_startTime = value;
				OnPropertyChanged(() => StartTime);
			}
		}

		DateTime _endDateTime;
		public DateTime EndDateTime
		{
			get { return _endDateTime; }
			set
			{
				_endDateTime = value;
				OnPropertyChanged(() => EndDateTime);
			}
		}

		TimeSpan _endTime;
		public TimeSpan EndTime
		{
			get { return _endTime; }
			set
			{
				_endTime = value;
				OnPropertyChanged(() => EndTime);
			}
		}

		string _comment;
		public string Comment
		{
			get { return _comment; }
			set
			{
				_comment = value;
				OnPropertyChanged(() => Comment);
			}
		}

		DateTime _documentDateTime;
		public DateTime DocumentDateTime
		{
			get { return _documentDateTime; }
			set
			{
				_documentDateTime = value;
				OnPropertyChanged(() => DocumentDateTime);
			}
		}

		int _documentNumber;
		public int DocumentNumber
		{
			get { return _documentNumber; }
			set
			{
				_documentNumber = value;
				OnPropertyChanged(() => DocumentNumber);
			}
		}

		public ObservableCollection<TimeTrackDocument> AvailableDocuments { get; private set; }

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
		#endregion

		public DocumentDetailsViewModel(bool canEditStartDateTime, Guid organisationUID, Guid employeeGuid, TimeTrackDocument timeTrackDocument = null)
		{
			CanEditStartDateTime = canEditStartDateTime;

			if (timeTrackDocument == null)
			{
				Title = "Добавление документа";
				timeTrackDocument = new TimeTrackDocument();
			}
			else
			{
				Title = "Редактирование документа";
				_isNew = false;
			}
			TimeTrackDocument = timeTrackDocument;
			TimeTrackDocument.EmployeeUID = employeeGuid;

			AvailableDocuments = new ObservableCollection<TimeTrackDocument>(SystemTypes(organisationUID)
				.Select(x => new TimeTrackDocument(x.Name, x.ShortName, x.Code, x.DocumentType)));

			DocumentsTypes = new ObservableCollection<DocumentType>(Enum.GetValues(typeof (DocumentType)).Cast<DocumentType>());

			AvailableDocumentsCollectionView = CollectionViewSource.GetDefaultView(AvailableDocuments);
			AvailableDocumentsCollectionView.Filter = DocumentsFilter;

			StartDateTime = timeTrackDocument.StartDateTime.Date;
			StartTime = timeTrackDocument.StartDateTime.TimeOfDay;
			EndDateTime = timeTrackDocument.EndDateTime.Date;
			EndTime = timeTrackDocument.EndDateTime.TimeOfDay;
			Comment = timeTrackDocument.Comment;
			DocumentNumber = timeTrackDocument.DocumentNumber;
			DocumentDateTime = timeTrackDocument.DocumentDateTime;
			SelectedDocument = AvailableDocuments.FirstOrDefault(x => x.TimeTrackDocumentType.Code == timeTrackDocument.DocumentCode);
			IsOutside = timeTrackDocument.IsOutside;

			this.WhenAny(x => x.SelectedDocumentType, x => x.Value)
				.Subscribe(_ =>
				{
					AvailableDocumentsCollectionView.Refresh();

					if (_isNew)
						SelectedDocument = (TimeTrackDocument) AvailableDocumentsCollectionView.CurrentItem;
					else
					{
						AvailableDocumentsCollectionView.MoveCurrentToFirst();
						SelectedDocument = (TimeTrackDocument) AvailableDocumentsCollectionView.CurrentItem;
					}

					IsEnableAbsence = SelectedDocumentType == DocumentType.Absence || SelectedDocumentType == DocumentType.AbsenceReasonable;

				});
		}

		private static IEnumerable<TimeTrackDocumentType> SystemTypes(Guid organisationUID)
		{
			var docTypes = Task.Factory.StartNew(() => DocumentTypeHelper.GetByOrganisation(organisationUID));
			return new List<TimeTrackDocumentType>(docTypes.Result);
		}

		private bool DocumentsFilter(object obj)
		{
			var doc = obj as TimeTrackDocument;
			return doc != null && doc.TimeTrackDocumentType.DocumentType == SelectedDocumentType;
		}

		protected override bool Save()
		{
			if (SelectedDocument == null)
			{
				MessageBoxService.ShowWarning("Необходимо выбрать вид документа");
				return false;
			}

			var startDateTime = StartDateTime + StartTime;
			var endDateTime = EndDateTime + EndTime;

			if (startDateTime >= endDateTime)
			{
				MessageBoxService.ShowWarning("Дата и время окончания должно быть позднее даты и времени начала");
				return false;
			}

			TimeTrackDocument.StartDateTime = startDateTime;
			TimeTrackDocument.EndDateTime = endDateTime;
			TimeTrackDocument.Comment = Comment;
			TimeTrackDocument.DocumentNumber = DocumentNumber;
			TimeTrackDocument.DocumentDateTime = DocumentDateTime;
			TimeTrackDocument.TimeTrackDocumentType = SelectedDocument.TimeTrackDocumentType;
			TimeTrackDocument.DocumentCode = SelectedDocument.TimeTrackDocumentType.Code;
			TimeTrackDocument.IsOutside = IsOutside;
			return base.Save();
		}
	}
}