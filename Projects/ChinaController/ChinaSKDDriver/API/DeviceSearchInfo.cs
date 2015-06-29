using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChinaSKDDriverAPI
{
	public class DeviceSearchInfo
	{
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

		private readonly string _submask ;
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

		public DeviceSearchInfo(string ipAddress, int port, string submask, string gateway, string mac)
		{
			_ipAddress = ipAddress;
			_port = port;
			_submask = submask;
			_gateway = gateway;
			_mac = mac;
		}
	}
}
