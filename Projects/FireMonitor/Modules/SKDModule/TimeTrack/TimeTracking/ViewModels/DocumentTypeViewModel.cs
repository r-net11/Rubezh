using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.SKD;
using Infrastructure.Common.Windows.TreeList;

namespace SKDModule.ViewModels
{
	public class DocumentTypeViewModel : TreeNodeViewModel<DocumentTypeViewModel>
	{
		public Organisation Organisation { get; private set; }
		public bool IsOrganisation { get; private set; }
		public string Name { get; private set; }
		public TimeTrackDocumentType TimeTrackDocumentType { get; private set; }

		public DocumentTypeViewModel(Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			Name = organisation.Name;
			IsExpanded = true;
		}

		public DocumentTypeViewModel(Organisation organisation, TimeTrackDocumentType timeTrackDocumentType)
		{
			Organisation = organisation;
			TimeTrackDocumentType = timeTrackDocumentType;
			IsOrganisation = false;
			Name = timeTrackDocumentType.Name;
		}

		public void Update(TimeTrackDocumentType timeTrackDocumentType)
		{
			TimeTrackDocumentType = timeTrackDocumentType;
			Name = timeTrackDocumentType.Name;
			OnPropertyChanged(() => TimeTrackDocumentType);
			OnPropertyChanged(() => Name);
		}
	}
}