using System;

namespace ChinaSKDDriverAPI
{
	public sealed class SearchDevicesEventArgs : EventArgs
	{
		private readonly DeviceSearchInfo _deviceSearchInfo;

		public DeviceSearchInfo DeviceSearchInfo
		{
			get { return _deviceSearchInfo; }
		}

		public SearchDevicesEventArgs(DeviceSearchInfo deviceSearchInfo)
		{
			_deviceSearchInfo = deviceSearchInfo;
		}
	}
}