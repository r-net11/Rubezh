using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.Models.Binary
{
	public class BinaryPanel
	{
		public Device ParentPanel { get; set; }
		public List<Zone> LocalZones = new List<Zone>();
		public List<Zone> RemoteZones = new List<Zone>();
		public List<BinaryZone> BinaryLocalZones = new List<BinaryZone>();
		public List<BinaryZone> BinaryRemoteZones = new List<BinaryZone>();
		public List<BinaryDevice> BinaryLocalDevices = new List<BinaryDevice>();
		public List<Device> remoteDevices = new List<Device>();
		public List<BinaryDevice> BinaryRemoteDevices = new List<BinaryDevice>();

		public BinaryPanel(Device parentPanel)
		{
			ParentPanel = parentPanel;
		}

		public void CreatreDevices()
		{
			foreach (var device in ParentPanel.Children)
			{
				var binaryDevice = new BinaryDevice()
				{
					ParentPanel = ParentPanel,
					Device = device,
					BinaryPanel = this
				};
				device.BinaryDevice = binaryDevice;
				BinaryLocalDevices.Add(binaryDevice);
			}
		}

		public void Initialize()
		{
			foreach (var device in ParentPanel.Children)
			{
				var binaryDevice = new BinaryDevice()
				{
					ParentPanel = ParentPanel,
					Device = device
				};
				device.BinaryDevice = binaryDevice;
				BinaryLocalDevices.Add(binaryDevice);

				foreach (var zone in device.ZonesInLogic)
				{
					var binaryLocalZone = BinaryLocalZones.FirstOrDefault(x => x.Zone == zone);
					if (binaryLocalZone != null)
					{
						if (!binaryDevice.BinaryZones.Contains(binaryLocalZone))
							binaryDevice.BinaryZones.Add(binaryLocalZone);
					}

					if (binaryLocalZone == null)
					{
						foreach (var binaryRemoteZone in BinaryRemoteZones)
						{
							if (!binaryDevice.BinaryZones.Contains(binaryRemoteZone))
								binaryDevice.BinaryZones.Add(binaryRemoteZone);

							foreach (var binaryPanel in binaryRemoteZone.BinaryPanels)
							{
								if (binaryPanel.ParentPanel != ParentPanel)
								{
									if (!remoteDevices.Contains(device))
									{
										remoteDevices.Add(device);
										binaryPanel.BinaryRemoteDevices.Add(binaryDevice);
									}
								}
							}
						}
					}
				}
			}
		}
	}
}