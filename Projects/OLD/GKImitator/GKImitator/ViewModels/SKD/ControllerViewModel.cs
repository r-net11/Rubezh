using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.Windows.ViewModels;
using FiresecAPI;
using GKImitator.Processor;
using Infrastructure.Common.Windows;
using System.Collections.ObjectModel;
using XFiresecAPI;

namespace GKImitator.ViewModels
{
	public class ControllerViewModel : BaseViewModel
	{
		public SKDDevice Device { get; private set; }
		SKDImitatorProcessor SKDImitatorProcessor;
		public int PortNo { get; private set; }

		public ControllerViewModel(SKDDevice device)
		{
			Device = device;
			var portProperty = device.Properties.FirstOrDefault(x => x.Name == "Port");
			if (portProperty != null)
			{
				PortNo = portProperty.Value;
				SKDImitatorProcessor = new SKDImitatorProcessor(PortNo);
				SKDImitatorProcessor.Start();
			}
			IsConnected = true;

			SKDEvents = new ObservableCollection<ControllerEventViewModel>();
			foreach (var skdEvent in SKDEventsHelper.SKDEvents)
			{
				if (skdEvent.DriverType == SKDDriverType.Controller)
					SKDEvents.Add(new ControllerEventViewModel(this, skdEvent));
			}

			Readers = new ObservableCollection<ReaderViewModel>();
			foreach (var childDevice in Device.Children)
			{
				if (childDevice.DriverType == SKDDriverType.Reader)
				{
					var readerViewModel = new ReaderViewModel(childDevice, SKDImitatorProcessor);
					Readers.Add(readerViewModel);
				}
			}
		}

		bool _isConnected;
		public bool IsConnected
		{
			get { return _isConnected; }
			set
			{
				_isConnected = value;
				OnPropertyChanged("IsConnected");
				SKDImitatorProcessor.IsConnected = value;
			}
		}

		public ObservableCollection<ControllerEventViewModel> SKDEvents { get; private set; }

		public void NewEvent(SKDEvent skdEvent)
		{
			var imitatorJournalItem = new SKDImitatorJournalItem()
			{
				NameCode = skdEvent.No,
				Source = 1,
				Address = 0,
				CardNo = 0
			};
			SKDImitatorProcessor.AddJournalItem(imitatorJournalItem);
		}

		public ObservableCollection<ReaderViewModel> Readers { get; private set; }
	}
}