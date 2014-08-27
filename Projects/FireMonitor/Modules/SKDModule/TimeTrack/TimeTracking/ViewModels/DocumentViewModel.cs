using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;

namespace SKDModule.ViewModels
{
	public class DocumentViewModel : BaseViewModel
	{
		public TimeTrackDocument Document { get; private set; }

		public DocumentViewModel(TimeTrackDocument timeTrackDocument)
		{
			Document = timeTrackDocument;
			Update();
		}

		public void Update()
		{
			var timeTrackDocumentType = TimeTrackDocumentTypesCollection.TimeTrackDocumentTypes.FirstOrDefault(x => x.Code == Document.DocumentCode);
			if (timeTrackDocumentType != null)
			{
				Name = timeTrackDocumentType.Name;
				ShortName = timeTrackDocumentType.ShortName;
				StartDateTime = Document.StartDateTime.ToString("yyyy-MM-dd HH:mm");
				EndDateTime = Document.EndDateTime.ToString("yyyy-MM-dd HH:mm");
			}
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => ShortName);
			OnPropertyChanged(() => StartDateTime);
			OnPropertyChanged(() => EndDateTime);
		}

		public string Name { get; private set; }
		public string ShortName { get; private set; }
		public string StartDateTime { get; private set; }
		public string EndDateTime { get; private set; }
	}
}