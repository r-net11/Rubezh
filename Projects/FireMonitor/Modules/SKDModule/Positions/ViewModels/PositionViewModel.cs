﻿using FiresecAPI;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class PositionViewModel : TreeNodeViewModel<PositionViewModel>
	{
		public Organisation Organisation { get; private set; }
		public bool IsOrganisation { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public ShortPosition Position { get; private set; }

		public PositionViewModel(Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			Name = organisation.Name;
			IsExpanded = true;
		}

		public PositionViewModel(Organisation organisation, ShortPosition position)
		{
			Organisation = organisation;
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
	}
}