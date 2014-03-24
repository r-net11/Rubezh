using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Entities.DeviceOriented;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.RVI_VSS.ViewModels
{
	public class DeviceViewModel : BaseViewModel
	{
		public ObservableCollection<Channel> Channels { get; set; }
		public Device Device { get; private set; }
		public DeviceViewModel(Device device)
		{
			Device = device;
			Address = device.IP;
			Channels = new ObservableCollection<Channel>(device.Channels);
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
