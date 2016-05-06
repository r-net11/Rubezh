using Infrastructure.Common.Windows.ViewModels;
using StrazhDeviceSDK.API;

namespace ControllerSDK.ViewModels
{
	public class SearchDeviceViewModel : BaseViewModel
	{
		private DeviceType _deviceType;
		public DeviceType DeviceType
		{
			get { return _deviceType; }
			set
			{
				_deviceType = value;
				OnPropertyChanged(() => DeviceType);
			}
		}

		private string _ipAddress;
		public string IpAddress
		{
			get { return _ipAddress; }
			set
			{
				_ipAddress = value;
				OnPropertyChanged(() => IpAddress);
			}
		}

		private int _port;
		public int Port
		{
			get { return _port; }
			set
			{
				_port = value;
				OnPropertyChanged(() => Port);
			}
		}

		private string _submask;
		public string Submask
		{
			get { return _submask; }
			set
			{
				_submask = value;
				OnPropertyChanged(() => Submask);
			}
		}

		private string _gateway;
		public string Gateway
		{
			get { return _gateway; }
			set
			{
				_gateway = value;
				OnPropertyChanged(() => Gateway);
			}
		}

		private string _mac;
		public string Mac
		{
			get { return _mac; }
			set
			{
				_mac = value;
				OnPropertyChanged(() => Mac);
			}
		}
	}
}
