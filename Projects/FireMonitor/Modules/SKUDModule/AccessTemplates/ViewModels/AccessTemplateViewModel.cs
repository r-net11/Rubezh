using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
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
			CardZonesViewModel = new CardZonesViewModel(new List<CardZone>());
			Organisation = organisation;
			IsOrganisation = true;
			Name = organisation.Name;
			IsExpanded = true;
		}

		public AccessTemplateViewModel(AccessTemplate accessTemplate)
		{
			AccessTemplate = accessTemplate;
			CardZonesViewModel = new CardZonesViewModel(accessTemplate.CardZones);
			IsOrganisation = false;
			Name = accessTemplate.Name;
			Description = accessTemplate.Description;
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