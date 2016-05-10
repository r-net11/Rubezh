
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using LocalizationConveters;

namespace StrazhDeviceSDK.API
{
	public class DeviceSearchInfo
	{
		private readonly DeviceType _deviceType;
		public DeviceType DeviceType
		{
			get { return _deviceType; }
		}

		private readonly string _ipAddress;

		public string IpAddress
		{
			get { return _ipAddress; }
		}

		private readonly int _port;

		public int Port
		{
			get { return _port; }
		}

		private readonly string _submask;

		public string Submask
		{
			get { return _submask; }
		}

		private readonly string _gateway;

		public string Gateway
		{
			get { return _gateway; }
		}

		private readonly string _mac;

		public string Mac
		{
			get { return _mac; }
		}

		public DeviceSearchInfo(DeviceType deviceType, string ipAddress, int port, string submask, string gateway, string mac)
		{
			_deviceType = deviceType;
			_ipAddress = ipAddress;
			_port = port;
			_submask = submask;
			_gateway = gateway;
			_mac = mac;
		}
	}

	public enum DeviceType
	{
        //[DeviceTypeLabel("Не известно")]
        [LocalizedDeviceTypeLabel(typeof(ChinaSKDDriver.Resources.Language.API.DeviceSearchInfo), "Unknown")]
		Unknown = 0,
		#region <Контроллер Dahua>
		//[DeviceTypeLabel("Однодверный контроллер", "SR-NC101")]
        [LocalizedDeviceTypeLabel(typeof(ChinaSKDDriver.Resources.Language.API.DeviceSearchInfo), "NC101")]
		DahuaBsc1221A,
        //[DeviceTypeLabel("Двухдверный контроллер", "SR-NC002")]
        [LocalizedDeviceTypeLabel(typeof(ChinaSKDDriver.Resources.Language.API.DeviceSearchInfo), "NC002")]
		DahuaBsc1201B,
        //[DeviceTypeLabel("Четырехдверный контроллер", "SR-NC004")]
        [LocalizedDeviceTypeLabel(typeof(ChinaSKDDriver.Resources.Language.API.DeviceSearchInfo), "NC004")]
		DahuaBsc1202B
		#endregion </Контроллер Dahua>
	}

	public class DeviceTypeLabelAttribute : Attribute
	{
		public string Type { get; set; }

		public string Label { get; set; }

		public DeviceTypeLabelAttribute(string type, string label = null)
		{
			Type = type;
			Label = label;
		}
	}
}

