using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class DocumentDetailsViewModel : SaveCancelDialogViewModel
	{
		public TimeTrackDocument TimeTrackDocument { get; private set; }

		public DocumentDetailsViewModel(TimeTrackDocument timeTrackDocument = null)
		{
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

			StartDateTime = timeTrackDocument.StartDateTime;
			EndDateTime = timeTrackDocument.EndDateTime;
			Comment = timeTrackDocument.Comment;
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
			TimeTrackDocument.StartDateTime = StartDateTime;
			TimeTrackDocument.EndDateTime = EndDateTime;
			TimeTrackDocument.Comment = Comment;
			if (SelectedDocument != null)
				TimeTrackDocument.DocumentCode = SelectedDocument.Code;
			return base.Save();
		}
	}
}