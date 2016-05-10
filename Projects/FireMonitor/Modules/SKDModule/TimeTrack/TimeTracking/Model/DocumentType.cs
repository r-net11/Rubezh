using System;
using StrazhAPI.SKD;
using Infrastructure.Common.TreeList;

namespace SKDModule.Model
{
	public class DocumentType : TreeNodeViewModel<DocumentType>
	{
		public Organisation Organisation { get; private set; }
		public bool IsOrganisation { get; private set; }
		public string Name { get; private set; }

		public Guid UID { get; private set; }

		public TimeTrackDocumentType TimeTrackDocumentType { get; private set; }

		public DocumentType(Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			Name = organisation.Name;
			UID = organisation.UID;
			IsExpanded = true;
		}

		public DocumentType(Organisation organisation, TimeTrackDocumentType timeTrackDocumentType)
		{
			Organisation = organisation;
			TimeTrackDocumentType = timeTrackDocumentType;
			IsOrganisation = false;
			Name = timeTrackDocumentType.Name;
			UID = timeTrackDocumentType.UID;
		}

		public void Update(TimeTrackDocumentType timeTrackDocumentType)
		{
			TimeTrackDocumentType = timeTrackDocumentType;
			Name = timeTrackDocumentType.Name;
			OnPropertyChanged(() => TimeTrackDocumentType);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => UID);
		}
	}
}