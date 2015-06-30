﻿using Infrastructure.Common.Windows.ViewModels;
using RviClient.RVIServiceReference;

namespace VideoModule.ViewModels
{
	public class DeviceViewModel : BaseViewModel
	{
		public DeviceViewModel(Device device, Channel channel, int streamNo, bool isEnabled)
		{
			Device = device;
			Channel = channel;
			StreamNo = streamNo;

			DeviceName = device.Name + " (" + "поток " + StreamNo + ")";
			DeviceIP = device.Ip;
			ChannalNumber = channel.Number;
			ChannalName = channel.Name;
			IsEnabled = isEnabled;
		}

		public int StreamNo { get; private set; }
		public Device Device { get; private set; }
		public Channel Channel { get; private set; }

		public string DeviceName { get; private set; }
		public string DeviceIP { get; private set; }
		public int ChannalNumber { get; private set; }
		public string ChannalName { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}

		public bool IsEnabled { get; private set; }
	}
}