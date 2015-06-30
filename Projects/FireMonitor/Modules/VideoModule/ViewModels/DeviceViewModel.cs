﻿using System.Collections.ObjectModel;
using Entities.DeviceOriented;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class DeviceViewModel : BaseViewModel
	{
		public ObservableCollection<IChannel> Channels { get; set; }
		public Device Device { get; private set; }
		public DeviceViewModel(Device device)
		{
			Device = device;
			Address = device.IP;
			Channels = new ObservableCollection<IChannel>(device.Channels);
		}

		private string _address;
		public string Address
		{
			get { return _address; }
			set
			{
				_address = value;
				OnPropertyChanged(() => Address);
			}
		}

		private Channel _sellectedChannel;
		public Channel SelectedChannel 
		{
			get { return _sellectedChannel; }
			set
			{
				_sellectedChannel = value;
				OnPropertyChanged(() => SelectedChannel);
			} 
		}
	}
}
