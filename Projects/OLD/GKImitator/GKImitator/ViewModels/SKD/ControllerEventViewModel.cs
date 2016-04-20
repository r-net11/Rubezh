using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Common.Windows;
using FiresecAPI;

namespace GKImitator.ViewModels
{
	public class ControllerEventViewModel : BaseViewModel
	{
		ControllerViewModel ControllerViewModel;
		public SKDEvent SKDEvent { get; private set; }

		public ControllerEventViewModel(ControllerViewModel controllerViewModel, SKDEvent skdEvent)
		{
			NewEventCommand = new RelayCommand(OnNewEvent);
			ControllerViewModel = controllerViewModel;
			SKDEvent = skdEvent;
		}

		public RelayCommand NewEventCommand { get; private set; }
		void OnNewEvent()
		{
			ControllerViewModel.NewEvent(SKDEvent);
		}
	}
}