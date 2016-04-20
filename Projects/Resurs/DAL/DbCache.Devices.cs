using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Runtime.Serialization;
using Infrastructure.Common.Windows.Windows;
using System.Diagnostics;


namespace ResursDAL
{
	public static partial class DbCache
	{
		public static Device RootDevice { get; private set; }
		public static List<Device> Devices { get { return GetAllChildren(RootDevice); } }
		
		public static Device GetRootDevice()
		{
			try
			{
				using (var context = DatabaseContext.Initialize())
				{
					var allDevices = context.Devices.Include(x => x.Parameters).Include(x => x.Tariff).Include(x => x.Tariff.TariffParts).OrderBy(x => x.Address).ToList();
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

		public static bool CanAddActive
		{
			get
			{
				var currentDevices = Devices.Where(x => x.DeviceType == DeviceType.Counter && x.IsActive).Count();
				var maxDevices = ResursAPI.License.LicenseManager.CurrentLicenseInfo.DevicesCount;
				return currentDevices <= maxDevices;
			}
		}

		public static bool SaveDevice(Device device)
		{
			try
			{
				bool isNew = false;
				foreach (var item in device.Parameters)
				{
					var validateResult = item.Validate();
					if (validateResult != null)
					{
						MessageBoxService.Show(string.Format("Устройство {0} \n Параметр {1} \n {2}", device.Name, item.DriverParameter.Description, validateResult));
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
						isNew = true;
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
					AddJournalForUser(isNew ? JournalType.AddDevice : JournalType.EditDevice, device);
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
				Guid deviceUID;
				string deviceName;
				using (var context = DatabaseContext.Initialize())
				{
					var tableItem = context.Devices.FirstOrDefault(x => x.UID == device.UID);
					if (tableItem == null)
						return false;
					deviceUID = tableItem.UID;
					deviceName = tableItem.Name;
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
				AddJournalForUser(JournalType.DeleteDevice, device);
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
				using (var context = DatabaseContext.Initialize())
				{
					RootDevice = new Device(DriverType.System);
					RootDevice.DateTime = RootDevice.DateTime.CheckDate();
					context.Devices.Add(RootDevice);
					context.SaveChanges();
				}
			}
			catch (Exception e)
			{
				MessageBoxService.Show(e.Message);
			}
		}

		public static void GenerateTestDevices()
		{
			try
			{
				using (var context = DatabaseContext.Initialize())
				{
					context.Devices.RemoveRange(context.Devices);
					context.SaveChanges();
				}
				var devices = new List<Device>();
				Random random = new Random();
				RootDevice = new Device(DriverType.System);
				devices.Add(RootDevice);
#if DEBUG
				int interfaces = 10;
				int devicesPerInterface = 100;
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

				AddRangeDevicesQuery(devices);
				AddRangeParametersQuery(devices.SelectMany(x => x.Parameters));
				
			}
			catch (Exception e)
			{
				MessageBoxService.Show(e.Message);
			}
		}


		static void AddRangeDevicesQuery(IEnumerable<Device> devices)
		{

			if (devices.Count() == 0)
				return;
			var index = 0;
			while (true)
			{
				var portion = devices.Skip(1000 * index).Take(1000);
				index++;
				if (portion.Count() == 0)
					break;
				var query = "INSERT INTO dbo.\"Devices\" (\"UID\", \"Name\", \"Description\", \"ParentUID\", \"TariffUID\", \"BillUID\", \"DriverUID\", \"Address\", \"IsActive\", \"IsDbMissmatch\", \"TariffType\", \"DateTime\") VALUES";
				foreach (var item in portion)
				{
					query += string.Format("('{0}', '{1}', '{2}', {3}, {4}, {5}, '{6}', '{7}', '{8}', '{9}', '{10}', '{11}'),",
						item.UID,
						item.Name,
						item.Description,
						item.ParentUID.ToSqlStr(),
						item.TariffUID.ToSqlStr(),
						item.ConsumerUID.ToSqlStr(),
						item.DriverUID,
						item.Address,
						item.IsActive,
						item.IsDbMissmatch,
						(int)item.TariffType,
						item.DateTime.CheckDate());
				}
				query = query.TrimEnd(',');
				using (var context = DatabaseContext.Initialize())
				{
					context.Database.ExecuteSqlCommand(query);
				}
			}
		}

		static void AddRangeParametersQuery(IEnumerable<Parameter> parameters)
		{
			if (parameters.Count() == 0)
				return;
			var index = 0;
			while (true)
			{
				var portion = parameters.Skip(1000 * index).Take(1000);
				index++;
				if (portion.Count() == 0)
					break;
				var query = "INSERT INTO dbo.\"Parameters\" (\"UID\", \"Device_UID\", \"DriverParameterUID\", \"IntValue\", \"DoubleValue\", \"BoolValue\", \"StringValue\", \"DateTimeValue\") VALUES";
				foreach (var item in portion)
				{
					query += string.Format("('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}'),",
						item.UID,
						item.Device.UID,
						item.DriverParameterUID,
						item.IntValue,
						item.DoubleValue != null ? item.DoubleValue.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : item.DoubleValue.ToString(),
						item.BoolValue,
						item.StringValue,
						item.DateTimeValue);
				}
				query = query.TrimEnd(',');
				using (var context = DatabaseContext.Initialize())
				{
					context.Database.ExecuteSqlCommand(query);
				}
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
					var device = context.Devices
						.Include(x => x.Parent)
						.Include(x => x.Parameters)
						.Include(x => x.Consumer)
						.Include(x => x.Tariff)
						.Include(x => x.Tariff.TariffParts)
						.FirstOrDefault(x => x.UID == uid);
					InitializeDevice(device);
					device.Parameters = device.Parameters.OrderBy(x => x.DriverParameter.Number).ToList();
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
			device.Driver = DriversConfiguration.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
			foreach (var item in device.Parameters)
			{
				var driverParameter = device.Driver.DriverParameters.FirstOrDefault(x => x.UID == item.DriverParameterUID);
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
			tableDevice.ConsumerUID = device.ConsumerUID;
			tableDevice.Consumer = device.Consumer != null ? context.Consumers.FirstOrDefault(x => x.UID == device.Consumer.UID) : null;
			tableDevice.Name = device.Name;
			tableDevice.Description = device.Description;
			tableDevice.IsActive = device.IsActive;
			tableDevice.TariffUID = device.Tariff != null ? (Guid?)device.Tariff.UID : null;
			tableDevice.TariffType = device.TariffType;
			tableDevice.Parent = device.Parent != null ? context.Devices.FirstOrDefault(x => x.UID == device.Parent.UID) : null;
			tableDevice.DriverUID = device.DriverUID;
			tableDevice.IsDbMissmatch = device.IsDbMissmatch;
			tableDevice.TariffUID = device.TariffUID;
			tableDevice.DateTime = device.DateTime.CheckDate();
			if (device.DeviceType == DeviceType.Network)
				tableDevice.ComPort = device.ComPort;
			tableDevice.Parameters = device.Parameters.Select(x => new Parameter
			{
				BoolValue = x.BoolValue,
				DateTimeValue = x.DateTimeValue,
				Device = tableDevice,
				DoubleValue = x.DoubleValue,
				IntValue = x.IntValue,
				DriverParameterUID = x.DriverParameterUID,
				StringValue = x.StringValue,
			}).ToList();
		}

		static readonly DateTime MinYear = new DateTime(1900, 1, 1);
		static readonly DateTime MaxYear = new DateTime(9000, 1, 1);
		static DateTime CheckDate(this DateTime value)
		{
			if (value < MinYear)
				return MinYear;
			if (value > MaxYear)
				return MaxYear;
			return value;
		}

		static DateTime? CheckDate(this DateTime? value)
		{
			if (value == null)
				return null;
			return value.Value.CheckDate();
		}

		static string CheckDateSqlStr(this DateTime? value)
		{
			if (value == null)
				return "NULL";
			return "'" + value.Value.CheckDate().ToString("yyyyMMdd HH:mm:ss") + "'";
		}

		static string ToSqlStr(this Guid? value)
		{
			if (value == null)
				return "NULL";
			return value.Value.ToSqlStr();
		}

		static string ToSqlStr(this Guid value)
		{
			return "'" + value.ToString() + "'";
		}
	}
}
