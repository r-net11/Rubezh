using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using GKImitator.Processor;
using Infrastructure.Common;
using System.Collections.ObjectModel;

namespace GKImitator.ViewModels
{
	public class SKDDeviceViewModel : BaseViewModel
	{
		public SKDDevice SKDDevice { get; private set; }
		SKDImitatorProcessor SKDImitatorProcessor;
		public int PortNo { get; private set; }

		public SKDDeviceViewModel(SKDDevice device)
		{
			NewEventCommand = new RelayCommand(OnNewEvent);
			SKDDevice = device;
			var portProperty = device.Properties.FirstOrDefault(x => x.Name == "Port");
			if (portProperty != null)
			{
				PortNo = portProperty.Value;
				SKDImitatorProcessor = new SKDImitatorProcessor(PortNo);
				SKDImitatorProcessor.Start();
			}

			SKDEvents = new ObservableCollection<SKDEvent>();
			foreach (var skdEvent in SKDEventsHelper.SKDEvents)
			{
				SKDEvents.Add(skdEvent);
			}
			SelectedSKDEvent = SKDEvents.FirstOrDefault();

			DeviceSources = new ObservableCollection<SKDDeviceSourceViewModel>();
			DeviceSources.Add(new SKDDeviceSourceViewModel(0, "Система"));
			DeviceSources.Add(new SKDDeviceSourceViewModel(1, "Контроллер"));
			for (int i = 0; i < SKDDevice.Children.Count; i++)
			{
				var childDevice = SKDDevice.Children[i];
				DeviceSources.Add(new SKDDeviceSourceViewModel(i + 2, childDevice.Driver.ShortName + " " + childDevice.Address));
			}
			SelectedDeviceSource = DeviceSources.FirstOrDefault();

			IsConnected = true;
		}

		int _cardNo;
		public int CardNo
		{
			get { return _cardNo; }
			set
			{
				_cardNo = value;
				OnPropertyChanged("CardNo");
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

		public ObservableCollection<SKDEvent> SKDEvents { get; private set; }

		SKDEvent _selectedSKDEvent;
		public SKDEvent SelectedSKDEvent
		{
			get { return _selectedSKDEvent; }
			set
			{
				_selectedSKDEvent = value;
				OnPropertyChanged("SelectedSKDEvent");
			}
		}

		public ObservableCollection<SKDDeviceSourceViewModel> DeviceSources { get; private set; }

		SKDDeviceSourceViewModel _selectedDeviceSource;
		public SKDDeviceSourceViewModel SelectedDeviceSource
		{
			get { return _selectedDeviceSource; }
			set
			{
				_selectedDeviceSource = value;
				OnPropertyChanged("SelectedDeviceSource");
			}
		}

		public RelayCommand NewEventCommand { get; private set; }
		void OnNewEvent()
		{
			SKDImitatorProcessor.LastJournalNo++;
			var imitatorJournalItem = new SKDImitatorJournalItem()
			{
				No = SKDImitatorProcessor.LastJournalNo,
				Source = SelectedDeviceSource.No,
				Code = SelectedSKDEvent.No,
				CardNo = CardNo
			};
			SKDImitatorProcessor.JournalItems.Add(imitatorJournalItem);
		}
	}
}