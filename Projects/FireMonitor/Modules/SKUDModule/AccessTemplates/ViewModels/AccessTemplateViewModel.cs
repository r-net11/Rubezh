using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class AccessTemplateViewModel : BaseViewModel
	{
		public AccessTemplate AccessTemplate { get; private set; }
		public CardZonesViewModel CardZonesViewModel { get; private set; }

		public AccessTemplateViewModel(AccessTemplate accessTemplate)
		{
			AccessTemplate = accessTemplate;
			CardZonesViewModel = new CardZonesViewModel(accessTemplate.CardZones);
		}

		public void Update(AccessTemplate accessTemplate)
		{
			AccessTemplate = accessTemplate;
			OnPropertyChanged("AccessTemplate");
			CardZonesViewModel = new CardZonesViewModel(accessTemplate.CardZones);
			OnPropertyChanged("CardZonesViewModel");
		}
	}
}