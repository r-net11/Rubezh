using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class CardDeviceViewModel : BaseViewModel
	{
		public SKDDevice Device { get; private set; }

		public CardDeviceViewModel(SKDDevice device)
		{
			Device = device;
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			var cardDeviceDetailsViewModel = new CardDeviceDetailsViewModel(Device);
			if (DialogService.ShowModalWindow(cardDeviceDetailsViewModel))
			{

			}
		}
	}
}