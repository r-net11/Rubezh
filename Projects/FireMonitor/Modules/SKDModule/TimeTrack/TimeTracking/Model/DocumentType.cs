using FiresecAPI.SKD;
using Infrastructure.Common.TreeList;

namespace SKDModule.Model
{
	public class DocumentType : TreeNodeViewModel<DocumentType>
	{
		public Organisation Organisation { get; private set; }
		public bool IsOrganisation { get; private set; }
		public string Name { get; private set; }
		public bool IsSystem { get; private set; }
		public TimeTrackDocumentType TimeTrackDocumentType { get; private set; }

		public DocumentType(Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			Name = organisation.Name;
			IsExpanded = true;
		}

		public DocumentType(Organisation organisation, TimeTrackDocumentType timeTrackDocumentType, bool isSystem = false)
		{
			Organisation = organisation;
			TimeTrackDocumentType = timeTrackDocumentType;
			IsOrganisation = false;
			Name = timeTrackDocumentType.Name;
			IsSystem = isSystem;
		}

		public void Update(TimeTrackDocumentType timeTrackDocumentType)
		{
			TimeTrackDocumentType = timeTrackDocumentType;
			Name = timeTrackDocumentType.Name;
			OnPropertyChanged(() => TimeTrackDocumentType);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => IsSystem);
		}
	}
}