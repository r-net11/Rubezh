using System;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DocumentDetailsViewModel : SaveCancelDialogViewModel
	{
		public TimeTrackDocument TimeTrackDocument { get; private set; }

		public DocumentDetailsViewModel(bool canEditStartDateTime, Guid organisationUID, TimeTrackDocument timeTrackDocument = null)
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
			}
			TimeTrackDocument = timeTrackDocument;

			AvailableDocuments = new ObservableCollection<TimeTrackDocumentType>();
			foreach (var timeTrackDocumentType in TimeTrackDocumentTypesCollection.TimeTrackDocumentTypes)
			{
				AvailableDocuments.Add(timeTrackDocumentType);
			}
			var documentTypes = DocumentTypeHelper.GetByOrganisation(organisationUID);
			foreach (var documentType in documentTypes)
			{
				AvailableDocuments.Add(documentType);
			}

			StartDateTime = timeTrackDocument.StartDateTime.Date;
			StartTime = timeTrackDocument.StartDateTime.TimeOfDay;
			EndDateTime = timeTrackDocument.EndDateTime.Date;
			EndTime = timeTrackDocument.EndDateTime.TimeOfDay;
			Comment = timeTrackDocument.Comment;
			DocumentNumber = timeTrackDocument.DocumentNumber;
			DocumentDateTime = timeTrackDocument.DocumentDateTime;
			SelectedDocument = AvailableDocuments.FirstOrDefault(x => x.Code == timeTrackDocument.DocumentCode);
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

		public ObservableCollection<TimeTrackDocumentType> AvailableDocuments { get; private set; }

		TimeTrackDocumentType _selectedDocument;
		public TimeTrackDocumentType SelectedDocument
		{
			get { return _selectedDocument; }
			set
			{
				_selectedDocument = value;
				OnPropertyChanged(() => SelectedDocument);
			}
		}

		protected override bool Save()
		{
			if (SelectedDocument == null)
			{
				MessageBoxService.ShowWarning("Необходимо выбрать тип документа");
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
			TimeTrackDocument.TimeTrackDocumentType = SelectedDocument;
			TimeTrackDocument.DocumentCode = SelectedDocument.Code;
			return base.Save();
		}
	}
}