using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using FiresecAPI.GK;

namespace FiresecClient
{
	public partial class GKManager
	{
		public static GKDevice CopyDevice(GKDevice device, bool fullCopy)
		{
			var newDevice = new GKDevice();
			CopyDevice(device, newDevice);

			if (fullCopy)
			{
				newDevice.UID = device.UID;
			}

			return newDevice;
		}

		public static GKDevice CopyDevice(GKDevice deviceFrom, GKDevice deviceTo)
		{
			deviceTo.DriverUID = deviceFrom.DriverUID;
			deviceTo.IntAddress = deviceFrom.IntAddress;
			deviceTo.Description = deviceFrom.Description;
			deviceTo.PredefinedName = deviceFrom.PredefinedName;

			deviceTo.Properties = new List<GKProperty>();
			foreach (var property in deviceFrom.Properties)
			{
				deviceTo.Properties.Add(new GKProperty()
				{
					Name = property.Name,
					Value = property.Value,
				});
			}

			deviceTo.Logic.OnClausesGroup = deviceFrom.Logic.OnClausesGroup.Clone();
			deviceTo.Logic.OffClausesGroup = deviceFrom.Logic.OffClausesGroup.Clone();
			deviceTo.Logic.StopClausesGroup = deviceFrom.Logic.StopClausesGroup.Clone();
			deviceTo.Logic.OnNowClausesGroup = deviceFrom.Logic.OnNowClausesGroup.Clone();
			deviceTo.Logic.OffNowClausesGroup = deviceFrom.Logic.OffNowClausesGroup.Clone();

			deviceTo.Children = new List<GKDevice>();
			foreach (var childDevice in deviceFrom.Children)
			{
				var newChildDevice = CopyDevice(childDevice, false);
				newChildDevice.Parent = deviceTo;
				deviceTo.Children.Add(newChildDevice);
			}

			deviceTo.PlanElementUIDs = new List<Guid>();
			foreach (var deviceElementUID in deviceFrom.PlanElementUIDs)
				deviceTo.PlanElementUIDs.Add(deviceElementUID);

			return deviceTo;
		}

		public static GKDevice AddChild(GKDevice parentDevice, GKDevice previousDevice, byte intAddress)
		{
			var device = new GKDevice()
			{
				DriverUID = Guid.NewGuid(),
				IntAddress = intAddress,
				Parent = parentDevice
			};

			if (previousDevice == null || parentDevice == previousDevice)
			{
				parentDevice.Children.Add(device);
			}
			else
			{
				var index = parentDevice.Children.IndexOf(previousDevice);
				parentDevice.Children.Insert(index + 1, device);
			}
			return device;
		}
	}
}