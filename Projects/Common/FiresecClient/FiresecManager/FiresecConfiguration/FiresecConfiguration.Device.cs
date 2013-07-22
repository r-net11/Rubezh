using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace FiresecClient
{
	public partial class FiresecConfiguration
	{
		public Device CopyDevice(Device device, bool fullCopy)
		{
			var newDevice = new Device()
			{
				DriverUID = device.DriverUID,
				Driver = device.Driver,
				IntAddress = device.IntAddress,
				Description = device.Description,
				ZoneUID = device.ZoneUID,
				Zone = device.Zone,
				HasExternalDevices = device.HasExternalDevices
			};

			if (fullCopy)
			{
				newDevice.UID = device.UID;
				newDevice.DatabaseId = device.DatabaseId;
				newDevice.PlanElementUIDs = device.PlanElementUIDs;
			}

			if (device.ZoneLogic != null)
			{
				newDevice.ZoneLogic = new ZoneLogic();
				newDevice.ZoneLogic.JoinOperator = device.ZoneLogic.JoinOperator;
				foreach (var clause in device.ZoneLogic.Clauses)
				{
					newDevice.ZoneLogic.Clauses.Add(new Clause()
					{
						State = clause.State,
						Operation = clause.Operation,
                        ZoneUIDs = clause.ZoneUIDs.ToList()
					});
				}
			}

			newDevice.Properties = new List<Property>();
			foreach (var property in device.Properties)
			{
				newDevice.Properties.Add(new Property()
				{
					Name = property.Name,
					Value = property.Value
				});
			}

			newDevice.SystemAUProperties = new List<Property>();
			foreach (var property in device.SystemAUProperties)
			{
				newDevice.SystemAUProperties.Add(new Property()
				{
					Name = property.Name,
					Value = property.Value
				});
			}

			newDevice.DeviceAUProperties = new List<Property>();
			foreach (var property in device.DeviceAUProperties)
			{
				newDevice.DeviceAUProperties.Add(new Property()
				{
					Name = property.Name,
					Value = property.Value
				});
			}

			newDevice.Children = new List<Device>();
			foreach (var childDevice in device.Children)
			{
				var newChildDevice = CopyDevice(childDevice, fullCopy);
				newChildDevice.Parent = newDevice;
				newDevice.Children.Add(newChildDevice);
			}

			return newDevice;
		}

		public void SynchronizeChildern(Device device)
		{
			for (int i = device.Children.Count(); i > 0; i--)
			{
				var childDevice = device.Children[i - 1];

				if (device.Driver.AvaliableChildren.Contains(childDevice.Driver.UID) == false)
				{
					device.Children.RemoveAt(i - 1);
				}
			}

			foreach (var autoCreateChildUID in device.Driver.AutoCreateChildren)
			{
				var autoCreateDriver = DriversConfiguration.Drivers.FirstOrDefault(x => x.UID == autoCreateChildUID);

				for (int i = autoCreateDriver.MinAutoCreateAddress; i <= autoCreateDriver.MaxAutoCreateAddress; i++)
				{
					var newDevice = new Device()
					{
						DriverUID = autoCreateDriver.UID,
						Driver = autoCreateDriver,
						IntAddress = i
					};
					if (device.Children.Any(x => x.Driver.DriverType == newDevice.Driver.DriverType && x.IntAddress == newDevice.IntAddress) == false)
					{
						device.Children.Add(newDevice);
						newDevice.Parent = device;
					}
				}
			}

			device.SynchronizeChildern();
		}
	}
}