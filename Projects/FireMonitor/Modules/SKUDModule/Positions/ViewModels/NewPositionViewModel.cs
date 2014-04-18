using FiresecAPI;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class NewPositionViewModel : TreeNodeViewModel<NewPositionViewModel>
	{
		public Organisation Organization { get; private set; }
		public ShortPosition Position { get; private set; }

		public NewPositionViewModel(Organisation organization)
		{
			Organization = organization;
			IsOrganisation = true;
			Name = organization.Name;
		}

		public NewPositionViewModel(ShortPosition position)
		{
			Position = position;
			IsOrganisation = false;
			Name = position.Name;
			Description = position.Description;
		}

		public void Update(ShortPosition position)
		{
			Name = position.Name;
			Description = position.Description;
			OnPropertyChanged("Name");
			OnPropertyChanged("Description");
		}

		public bool IsOrganisation { get; private set; }

		public string Name { get; private set; }
		public string Description { get; private set; }
	}
}