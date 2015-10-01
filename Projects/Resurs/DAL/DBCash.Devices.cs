using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Runtime.Serialization;


namespace ResursDAL
{
	public static partial class DBCash
	{
		public static Device RootDevice { get; private set; }
		
		public static Device GetRootDevice()
		{
			try
			{
				using (var context = DatabaseContext.Initialize())
				{
					var allDevices = context.Devices.Include(x => x.Parent).Include(x => x.Parameters).ToList();
					allDevices.ForEach(x => SetDeviceDriver(x));
					var result = allDevices.FirstOrDefault(x => x.Parent == null);
					if(result != null)
						SetChildren(result, allDevices);
					return result;
				}
			}
			catch (Exception e)
			{
				return null;
			}
		}

		public static bool SaveDevice(Device device)
		{
			try
			{
				using (var context = DatabaseContext.Initialize())
				{
					var tableDevice = context.Devices.Include(x => x.Parameters).FirstOrDefault(x => x.UID == device.UID);
					if (tableDevice != null)
					{
						context.Parameters.RemoveRange(tableDevice.Parameters);
						CopyDevice(device, tableDevice, context);
						var parent = GetAllChildren(RootDevice).FirstOrDefault(x => x.UID == device.Parent.UID);
						parent.Children.RemoveAll(x => x.UID == device.UID);
						parent.Children.Add(device);
					}
					else
					{
						tableDevice = new Device { UID = device.UID };
						CopyDevice(device, tableDevice, context);
						context.Devices.Add(tableDevice);
						if (device.Parent != null)
						{
							var parent = GetAllChildren(RootDevice).FirstOrDefault(x => x.UID == device.Parent.UID);
							if (!parent.Children.Any(x => x.UID == device.UID))
								parent.Children.Add(device);
						}
					}
					context.SaveChanges();
				}
				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}

		public static bool DeleteDevice(Device device)
		{
			try
			{
				using (var context = DatabaseContext.Initialize())
				{
					var tableItem = context.Devices.FirstOrDefault(x => x.UID == device.UID);
					if (tableItem == null)
						return false;
					var items = new List<Device>();
					var currentItems = context.Devices.Where(x => x.Parent.UID == device.UID).ToList();
					items.AddRange(currentItems);
					while (currentItems.Count > 0)
					{
						var childrenItems = new List<Device>();
						foreach (var item in currentItems)
						{
							var childItems = context.Devices.Where(x => x.Parent.UID == item.UID).ToList();
							childrenItems.AddRange(childItems);
							items.AddRange(childItems);
						}
						currentItems = childrenItems;
					}
					items.Add(tableItem);
					context.Devices.RemoveRange(items);
					context.SaveChanges();
				
					var parent = GetAllChildren(RootDevice).FirstOrDefault(x => x.UID == device.Parent.UID);
					parent.Children.RemoveAll(x => x.UID == device.UID);
				}
				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}

		public static void CreateSystem()
		{
			var devices = new List<Device>();
			RootDevice = new Device(DriverType.System);
			devices.Add(RootDevice);
			
			//for (int i = 0; i < 5; i++)
			//{
			//	var interfaceDevice = new Device(DriverType.BeregunInterface, RootDevice);
			//	devices.Add(interfaceDevice);
			//	for (int j = 0; j < 1000; j++)
			//	{
			//		var counter = new Device(DriverType.BeregunCounter, interfaceDevice);
			//		devices.Add(counter);
			//	}
			//}
			//for (int i = 0; i < 5; i++)
			//{
			//	var interfaceDevice = new Device(DriverType.MZEP55Interface, RootDevice);
			//	devices.Add(interfaceDevice);
			//	for (int j = 0; j < 1000; j++)
			//	{
			//		var counter = new Device(DriverType.MZEP55Counter, interfaceDevice);
			//		devices.Add(counter);
			//	}
			//}
			using (var context = DatabaseContext.Initialize())
			{
				context.Devices.AddRange(devices);
				context.SaveChanges();
			}
		}

		static void SetDeviceDriver(Device device)
		{
			device.Driver = DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == device.DriverType);
			foreach (var item in device.Parameters)
			{
				item.DriverParameter = device.Driver.DriverParameters.FirstOrDefault(x => x.Number == item.Number);
			}
		}

		static void SetChildren(Device device, List<Device> allDevices)
		{
			device.Children = new List<Device>(allDevices.Where(x => x.Parent != null && x.Parent == device));
			device.SetFullAddress();
			foreach (var item in device.Children)
			{
				SetChildren(item, allDevices);
			}
		}

		static void CopyDevice(Device device, Device tableDevice, DatabaseContext context)
		{
			tableDevice.Address = device.Address;
			tableDevice.Bill = device.Bill != null ? context.Bills.FirstOrDefault(x => x.UID == device.Bill.UID) : null;
			tableDevice.Description = device.Description;
			tableDevice.IsActive = device.IsActive;
			tableDevice.Tariff = device.Tariff != null ? context.Tariffs.FirstOrDefault(x => x.UID == device.Tariff.UID) : null;
			tableDevice.Parent = device.Parent != null ? context.Devices.FirstOrDefault(x => x.UID == device.Parent.UID) : null;
			tableDevice.DriverType = device.DriverType;
			tableDevice.Parameters = device.Parameters.Select(x => new Parameter
			{
				BoolValue = x.BoolValue,
				DateTimeValue = x.DateTimeValue,
				Device = tableDevice,
				DoubleValue = x.DoubleValue,
				IntValue = x.IntValue,
				IsPollingEnabled = x.IsPollingEnabled,
				Number = x.Number,
				StringValue = x.StringValue,
			}).ToList();
		}
	}
}
