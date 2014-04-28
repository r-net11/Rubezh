using System.Collections.Generic;
using FiresecAPI;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class AccessTemplateViewModel : TreeNodeViewModel<AccessTemplateViewModel>
	{
		public Organisation Organisation { get; private set; }
		public bool IsOrganisation { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public AccessTemplate AccessTemplate { get; private set; }
		public CardZonesViewModel CardZonesViewModel { get; private set; }

		public AccessTemplateViewModel(Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			Name = organisation.Name;
			IsExpanded = true;
			CardZonesViewModel = new CardZonesViewModel(new List<CardZone>());
		}

		public AccessTemplateViewModel(Organisation organisation, AccessTemplate accessTemplate)
		{
			Organisation = organisation;
			AccessTemplate = accessTemplate;
			IsOrganisation = false;
			Name = accessTemplate.Name;
			Description = accessTemplate.Description;
			CardZonesViewModel = new CardZonesViewModel(accessTemplate.CardZones);
		}

		public void Update(AccessTemplate accessTemplate)
		{
			Name = accessTemplate.Name;
			Description = accessTemplate.Description;
			OnPropertyChanged("Name");
			OnPropertyChanged("Description");
		}
	}
}