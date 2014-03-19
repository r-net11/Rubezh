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
		private Device Device { get; set; }
		public DeviceViewModel(Device device)
		{
			Device = device;
			Channels = new ObservableCollection<Channel>(device.Channels);
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
