using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Runtime.Serialization;
using Infrastructure.Common.Windows;


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
					
					var tableItems = context.Devices.Select(x => new 
						{ 
							UID = x.UID, 
							Address = x.Address, 
							DriverType = x.DriverType, 
							Description = x.Description,
							ParentUID = x.ParentUID, 
							IsActive = x.IsActive,
							TariffType = x.TariffType,
							ComPort = x.ComPort,
							TariffUID = x.TariffUID,
						}).ToList();
					var allDevices = tableItems.Select(x => new Device
						{
							UID = x.UID,
							Address = x.Address,
							DriverType =(DriverType)x.DriverType,
							Description = x.Description,
							ParentUID = x.ParentUID,
							IsActive = x.IsActive,
							TariffType = x.TariffType,
							ComPort = x.ComPort,
							TariffUID = x.TariffUID,
						}).OrderBy(x => x.Address).ToList();
					
					SetChildren(allDevices);
					var result = allDevices.FirstOrDefault(x => x.ParentUID == null);
					if (result != null)
					{
						SetFullAddress(result);
						result.IsActive = true;
					}
					allDevices.ForEach(x => InitializeDevice(x));
					
					return result;

				}
			}
			catch (Exception e)
			{
				MessageBoxService.Show(e.Message);
				return null;
			}
		}

		public static bool SaveDevice(Device device)
		{
			try
			{
				foreach (var item in device.Parameters)
				{
					var validateResult = item.Validate();
					if (validateResult != null)
					{
						MessageBoxService.Show(string.Format("Устройство {0} \n Параметр {1} \n {2}", device.Name, item.DriverParameter.Name, validateResult));
						return false;
					}
				}
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
				MessageBoxService.Show(e.Message);
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
				MessageBoxService.Show(e.Message);
				return false;
			}
		}

		public static void CreateSystem()
		{
			try
			{
				var devices = new List<Device>();
				Random random = new Random();
				RootDevice = new Device(DriverType.System);
				devices.Add(RootDevice);
#if DEBUG
				int interfaces = 2;
				int devicesPerInterface = 2;
				for (int i = 0; i < interfaces / 2; i++)
				{
					var interfaceDevice = new Device(DriverType.BeregunNetwork, RootDevice);
					interfaceDevice.ComPort = "COM" + (i + 1);
					InitializeTestDevice(interfaceDevice, random);
					devices.Add(interfaceDevice);
					for (int j = 0; j < devicesPerInterface; j++)
					{
						var counter = new Device(DriverType.BeregunCounter, interfaceDevice);
						InitializeTestDevice(counter, random);
						devices.Add(counter);
					}
				}
				for (int i = 0; i < interfaces / 2; i++)
				{
					var interfaceDevice = new Device(DriverType.MZEP55Network, RootDevice);
					interfaceDevice.ComPort = "COM" + (interfaces / 2 + i + 1);
					devices.Add(interfaceDevice);
					InitializeTestDevice(interfaceDevice, random);
					for (int j = 0; j < devicesPerInterface; j++)
					{
						var counter = new Device(DriverType.MZEP55Counter, interfaceDevice);
						InitializeTestDevice(counter, random);
						devices.Add(counter);
					}
				}
				var dateTimes = devices.SelectMany(x => x.Parameters).Select(x => x.DateTimeValue).Distinct();
#endif
				using (var context = DatabaseContext.Initialize())
				{
					context.Devices.RemoveRange(context.Devices);
					context.Devices.AddRange(devices);
					context.SaveChanges();
					var tableDateTimes = context.Devices.SelectMany(x => x.Parameters).Select(x => x.DateTimeValue).Distinct();
				}
			}
			catch (Exception e)
			{
				MessageBoxService.Show(e.Message);
			}
		}

		static void InitializeTestDevice(Device device, Random random)
		{
			device.Description = device.Name + " " + device.FullAddress;
			if(device.DeviceType == DeviceType.Counter && device.Driver.CanEditTariffType)
			{
				device.TariffType = (TariffType)random.Next(4);
			}
			foreach (var item in device.Parameters)
			{
				switch (item.DriverParameter.ParameterType)
				{
					case ParameterType.Enum:
						var maxValue = item.DriverParameter.ParameterEnumItems.Max(x => x.Value);
						item.IntValue = random.Next(maxValue + 1);
						break;
					case ParameterType.String:
						item.StringValue = Guid.NewGuid().ToString();
						break;
					case ParameterType.Int:
						var intMinValue = item.DriverParameter.IntMinValue ?? 0;
						var intMaxValue = item.DriverParameter.IntMaxValue ?? 10000;
						item.IntValue = random.Next(intMinValue, intMaxValue);
						break;
					case ParameterType.Double:
						var doubleMinValue = item.DriverParameter.DoubleMinValue ?? 0;
						var doubleMaxValue = item.DriverParameter.DoubleMaxValue ?? 10000;
						item.DoubleValue = Math.Truncate(doubleMinValue + random.NextDouble() * doubleMaxValue * 100) / 100;
						break;
					case ParameterType.Bool:
						item.BoolValue = random.Next(2) > 0;
						break;
					case ParameterType.DateTime:
						var minDateTime = item.DriverParameter.DateTimeMinValue ?? new DateTime(2000, 1, 1);
						var maxDateTime = item.DriverParameter.DateTimeMaxValue != null && item.DriverParameter.DateTimeMaxValue.Value < DateTime.Now ? item.DriverParameter.DateTimeMaxValue.Value : DateTime.Now;
						var delta = (maxDateTime - minDateTime).TotalSeconds;
						var randomDelta = TimeSpan.FromSeconds(random.NextDouble() * delta);
						item.DateTimeValue = minDateTime + randomDelta;
						break;
					default:
						break;
				}
			}
		}

		public static Device GetDevice(Guid uid)
		{
			try
			{
				var cashedDevice = GetAllChildren(RootDevice).FirstOrDefault(x => x.UID == uid);
				if (cashedDevice != null && cashedDevice.IsLoaded)
					return cashedDevice;

				using (var context = DatabaseContext.Initialize())
				{
					var device = context.Devices.Include(x => x.Parent).Include(x => x.Parameters).Include(x => x.Bill).FirstOrDefault(x => x.UID == uid);
					device.Parameters = device.Parameters.OrderBy(x => x.Number).ToList();
					InitializeDevice(device);
					device.IsLoaded = true;
					var parent = GetAllChildren(RootDevice).FirstOrDefault(x => x.UID == device.ParentUID);
					if (parent != null)
					{
						parent.Children.RemoveAll(x => x.UID == device.UID);
						parent.Children.Add(device);
						device.Parent = parent;
						device.SetFullAddress();
					}
					return device;
				}
			}
			catch (Exception e)
			{
				MessageBoxService.Show(e.Message);
				return null;
			}
		}

		static void InitializeDevice(Device device)
		{
			device.Driver = DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == device.DriverType);
			foreach (var item in device.Parameters)
			{
				var driverParameter = device.Driver.DriverParameters.FirstOrDefault(x => x.Number == item.Number);
				item.Initialize(driverParameter);
			}
			if (!device.Driver.CanEditTariffType)
				device.TariffType = device.Driver.DefaultTariffType;
		}

		static void SetChildren(List<Device> allDevices)
		{
			foreach (var sameParent in allDevices.GroupBy(x => x.ParentUID))
			{
				Device parent = null;
				foreach (var item in allDevices)
				{
					if (item.UID == sameParent.Key)
					{
						parent = item;
						break;
					}
				}

				if (parent != null)
				{
					foreach (var item in sameParent)
					{
						item.Parent = parent;
						parent.Children.Add(item);
					}
				}
			}
		}
		static void SetFullAddress(Device device)
		{
			device.SetFullAddress();
			foreach (var item in device.Children)
			{
				SetFullAddress(item);
			}
		}

		static void SetChildren(Device device, List<Device> allDevices)
		{
			device.Children = new List<Device>(allDevices.Where(x => x.ParentUID != null && x.ParentUID == device.UID).OrderBy(x => x.Address));
			foreach (var item in device.Children)
			{
				item.Parent = device;
				SetChildren(item, allDevices);
			}
		}

		static void CopyDevice(Device device, Device tableDevice, DatabaseContext context)
		{
			tableDevice.Address = device.Address;
			tableDevice.BillUID = device.BillUID;
			tableDevice.Bill = device.Bill != null ? context.Bills.FirstOrDefault(x => x.UID == device.Bill.UID) : null;
			tableDevice.Description = device.Description;
			tableDevice.IsActive = device.IsActive;
			tableDevice.Tariff = device.Tariff != null ? context.Tariffs.FirstOrDefault(x => x.UID == device.Tariff.UID) : null;
			tableDevice.TariffType = device.TariffType;
			tableDevice.Parent = device.Parent != null ? context.Devices.FirstOrDefault(x => x.UID == device.Parent.UID) : null;
			tableDevice.DriverType = device.DriverType;
			tableDevice.IsDbMissmatch = device.IsDbMissmatch;
			tableDevice.TariffUID = device.TariffUID;
			if (device.DeviceType == DeviceType.Network)
				tableDevice.ComPort = device.ComPort;
			tableDevice.Parameters = device.Parameters.Select(x => new Parameter
			{
				BoolValue = x.BoolValue,
				DateTimeValue = x.DateTimeValue,
				Device = tableDevice,
				DoubleValue = x.DoubleValue,
				IntValue = x.IntValue,
				Number = x.Number,
				StringValue = x.StringValue,
			}).ToList();
		}
	}
}
