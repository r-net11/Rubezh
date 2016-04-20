using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Common.Windows;
using FiresecAPI;

namespace GKImitator.ViewModels
{
	public class ReaderEventViewModel : BaseViewModel
	{
		ReaderViewModel ReaderViewModel;
		public SKDEvent SKDEvent { get; private set; }

		public ReaderEventViewModel(ReaderViewModel readerViewModel, SKDEvent skdEvent)
		{
			NewEventCommand = new RelayCommand(OnNewEvent);
			ReaderViewModel = readerViewModel;
			SKDEvent = skdEvent;
		}

		public RelayCommand NewEventCommand { get; private set; }
		void OnNewEvent()
		{
			ReaderViewModel.NewEvent(SKDEvent);
		}
	}
}