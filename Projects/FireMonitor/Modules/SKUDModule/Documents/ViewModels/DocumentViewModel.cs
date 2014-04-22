using FiresecAPI;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class DocumentViewModel : TreeNodeViewModel<DocumentViewModel>
	{
		public Organisation Organisation { get; private set; }
		public ShortDocument Document { get; private set; }

		public DocumentViewModel(Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			Name = organisation.Name;
		}

		public DocumentViewModel(ShortDocument document)
		{
			Document = document;
			IsOrganisation = false;
			Name = document.Name;
			Description = document.Description;
		}

		public void Update(ShortDocument document)
		{
			Name = document.Name;
			Description = document.Description;
			OnPropertyChanged("Name");
			OnPropertyChanged("Description");
		}

		public bool IsOrganisation { get; private set; }

		public string Name { get; private set; }
		public string Description { get; private set; }
	}
}