using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.TreeList;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class NewPositionViewModel : TreeNodeViewModel<NewPositionViewModel>
	{
		public Organization Organization { get; private set; }
		public Position Position { get; private set; }

		public NewPositionViewModel(Organization organization)
		{
			Organization = organization;
			IsOrganisation = true;
			Name = organization.Name;
		}

		public NewPositionViewModel(Position position)
		{
			Position = position;
			IsOrganisation = false;
			Name = position.Name;
			Description = position.Description;
		}

		public void Update(Position position)
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