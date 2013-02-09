using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace MonitorClientFS2.ViewModels
{
	public class DeviceViewModel : TreeItemViewModel<DeviceViewModel>
	{
		public Device Device { get; private set; }

		public DeviceViewModel(Device device)
		{
			Device = device;
		}

		public string UsbChannel
		{
			get
			{
				var property = Device.Properties.FirstOrDefault(x => x.Name == "UsbChannel");
				if (property != null)
					return property.Value;
				else
					return null;
			}
		}

		public string SerialNo
		{
			get
			{
				var property = Device.Properties.FirstOrDefault(x => x.Name == "SerialNo");
				if (property != null)
					return property.Value;
				else
					return null;
			}
		}

		public string Version
		{
			get
			{
				var property = Device.Properties.FirstOrDefault(x => x.Name == "Version");
				if (property != null)
					return property.Value;
				else
					return null;
			}
		}

		public string Address
		{
			get { return Device.PresentationAddress; }
		}

		public Driver Driver
		{
			get { return Device.Driver; }
		}

		public int ShleifNo
		{
			get { return Device.IntAddress / 256; }
		}

		public int AddressOnShleif
		{
			get { return Device.IntAddress % 256; }
		}
	}
}