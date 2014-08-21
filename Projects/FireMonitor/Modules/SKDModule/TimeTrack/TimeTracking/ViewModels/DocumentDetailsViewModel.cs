using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class DocumentDetailsViewModel : SaveCancelDialogViewModel
	{
		public TimeTrackDocument TimeTrackDocument { get; private set; }

		public DocumentDetailsViewModel(bool canEditStartDateTime, TimeTrackDocument timeTrackDocument = null)
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

			StartDateTime = timeTrackDocument.StartDateTime.Date;
			StartTime = timeTrackDocument.StartDateTime;
			EndDateTime = timeTrackDocument.EndDateTime.Date;
			EndTime = timeTrackDocument.EndDateTime;
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

		DateTime _startTime;
		public DateTime StartTime
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

		DateTime _endTime;
		public DateTime EndTime
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
			var startDateTime = StartDateTime + StartTime.TimeOfDay;
			var endDateTime = EndDateTime + EndTime.TimeOfDay;

			if (startDateTime >= endDateTime)
			{
				MessageBoxService.ShowWarning("Время окончания не может быть раньше времени начала");
				return false;
			}

			TimeTrackDocument.StartDateTime = startDateTime;
			TimeTrackDocument.EndDateTime = endDateTime;
			TimeTrackDocument.Comment = Comment;
			TimeTrackDocument.DocumentNumber = DocumentNumber;
			TimeTrackDocument.DocumentDateTime = DocumentDateTime;
			if (SelectedDocument != null)
				TimeTrackDocument.DocumentCode = SelectedDocument.Code;
			return base.Save();
		}
	}
}