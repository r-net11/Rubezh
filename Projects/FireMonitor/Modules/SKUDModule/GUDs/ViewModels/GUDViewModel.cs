using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class GUDViewModel : BaseViewModel
	{
		public GUD GUD { get; private set; }
		public CardZonesViewModel CardZonesViewModel { get; private set; }

		public GUDViewModel(GUD gud)
		{
			GUD = gud;
			CardZonesViewModel = new CardZonesViewModel(gud.CardZones);
		}

		public void Update(GUD gud)
		{
			GUD = gud;
			OnPropertyChanged("GUD");
			CardZonesViewModel = new CardZonesViewModel(gud.CardZones);
			OnPropertyChanged("CardZonesViewModel");
		}
	}
}