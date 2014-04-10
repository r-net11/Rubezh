using Entities.DeviceOriented;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class FoundCameraViewModel:BaseViewModel
	{
		public FoundCameraViewModel(DeviceSearchInfo deviceSearchInfo)
		{
			Address = deviceSearchInfo.IpAddress;
			DeviceType = deviceSearchInfo.DeviceType;
			Port = deviceSearchInfo.Port;
			SerialNo = deviceSearchInfo.SerialNo;
		}

		private string _address;
		public string Address
		{
			get { return _address; }
			set
			{
				_address = value;
				OnPropertyChanged("Address");
			}
		}
		
		private string _deviceType;
		public string DeviceType
		{
			get { return _deviceType; }
			set
			{
				_deviceType = value;
				OnPropertyChanged("DeviceType");
			}
		}

		private int _port;
		public int Port
		{
			get { return _port; }
			set
			{
				_port = value;
				OnPropertyChanged("Port");
			}
		}

		private string _serialNo;
		public string SerialNo
		{
			get { return _serialNo; }
			set
			{
				_serialNo = value;
				OnPropertyChanged("SerialNo");
			}
		}

		private bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(()=>IsChecked);
			}
		}

		public bool IsAdded { get; set; }
	}
}
