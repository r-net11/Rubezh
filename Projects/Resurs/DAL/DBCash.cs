using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Runtime.Serialization;
using Common;

namespace ResursDAL
{
	public static class DBCash
	{
		public static Device RootDevice { get; set; }
		public static Apartment RootApartment { get; set; }
		public static List<User> Users { get; set; }
		public static User CurrentUser { get; set; }
		static DBCash()
		{
			//RootDevice = GetRootDevice();
			Users = GetAllUsers();
			RootDevice = new Device(DriverType.System);
			var interface1 = new Device(DriverType.BeregunInterface);
			interface1.AddChild(DriverType.BeregunCounter);
			interface1.AddChild(DriverType.BeregunCounter);
			interface1.AddChild(DriverType.BeregunCounter);
			RootDevice.AddChild(interface1);
			var interface2 = new Device(DriverType.MZEP55Interface);
			interface2.AddChild(DriverType.MZEP55Counter);
			interface2.AddChild(DriverType.MZEP55Counter);
			interface2.AddChild(DriverType.MZEP55Counter);
			RootDevice.AddChild(interface2);
			//ResursDAL.DatabaseCash.SaveDevice(RootDevice);
			
			RootApartment = new Apartment()
			{
				Name = "Жилой комплекс",
				Children = new List<Apartment>()
				{
					new Apartment()
					{
						Name = "Дом 1",
						Children = new List<Apartment>()
						{
							new Apartment() {Name = "Квартира 1"},
							new Apartment() {Name = "Квартира 2"},
							new Apartment() {Name = "Квартира 3"},
							new Apartment() {Name = "Квартира 4"},
							new Apartment() {Name = "Квартира 5"},
							new Apartment() {Name = "Квартира 6"},
							new Apartment() {Name = "Квартира 7"},
							new Apartment() {Name = "Квартира 8"},
						}
					},
					new Apartment()
					{
						Name = "Дом 2",
						Children = new List<Apartment>()
						{
							new Apartment() {Name = "Квартира 1"},
							new Apartment() {Name = "Квартира 2"},
							new Apartment() {Name = "Квартира 3"},
							new Apartment() {Name = "Квартира 4"},
							new Apartment() {Name = "Квартира 5"},
							new Apartment() {Name = "Квартира 6"},
							new Apartment() {Name = "Квартира 7"},
							new Apartment() {Name = "Квартира 8"},
							new Apartment() {Name = "Квартира 9"},
							new Apartment() {Name = "Квартира 10"},
							new Apartment() {Name = "Квартира 11"},
							new Apartment() {Name = "Квартира 12"},
							new Apartment() {Name = "Квартира 13"},
							new Apartment() {Name = "Квартира 14"},
							new Apartment() {Name = "Квартира 15"},
							new Apartment() {Name = "Квартира 66"},
							new Apartment() {Name = "Квартира 666"},
						}
					}
				}
			};
		}
		
		public static Device GetRootDevice()
		{
			using(var context = DatabaseContext.Initialize())
			{
				var allDevices = context.Devices.Include(x => x.Parent).Include(x => x.Parameters).ToList();
				allDevices.ForEach(x => SetDeviceDriver(x));
				var result = allDevices.FirstOrDefault(x => x.Parent == null);
				SetChildren(result, allDevices);
				return result;
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

		public static void SaveDevice(Device device)
		{
			using (var context = DatabaseContext.Initialize())
			{
				var tableDevice = context.Devices.Include(x => x.Parameters).FirstOrDefault(x => x.UID == device.UID);
				if (tableDevice != null)
				{
					context.Parameters.RemoveRange(tableDevice.Parameters);
					tableDevice.Parameters = new List<Parameter>();
					tableDevice.Address = device.Address;
					tableDevice.Bill = device.Bill;
					tableDevice.Description = device.Description;
					tableDevice.IsActive = device.IsActive;
					tableDevice.Tariff = device.Tariff;
					tableDevice.Parameters.AddRange(device.Parameters.Select(x => new Parameter
					{
						BoolValue = x.BoolValue,
						DateTimeValue = x.DateTimeValue,
						Device = tableDevice,
						DoubleValue = x.DoubleValue,
						IntValue = x.IntValue,
						IsPollingEnabled = x.IsPollingEnabled,
						Number = x.Number,
						StringValue = x.StringValue,
					}));
				}
				
				//var devices = context.Devices.Include(x => x.Children);
				//devices.Add(device);
				//context.Devices.Add(device);
				context.SaveChanges();
			}
		}

		public static List<User> GetAllUsers()
		{
			using (var context = DatabaseContext.Initialize())
			{
				var result = new List<User>();
				foreach (var item in context.Users.Select(x => new { UID = x.UID, Name = x.Name, Login = x.Login }))
				{
					 result.Add(new User
					 {
						 UID = item.UID, 
						 Name = item.Name, 
						 Login = item.Login
					 });
				}

				if (!result.Any())
				{
					var userpermissions = new List<UserPermission>();
					User user = new User() { Name = "Adm", Login = "Adm", PasswordHash = HashHelper.GetHashFromString("")};
					userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.Apartment });
					userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.Device });
					userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.EditApartment });
					userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.EditDevice });
					userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.EditUser });
					userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.User });
					user.UserPermissions = userpermissions;
					context.Users.Add(user);
					context.SaveChanges();
					result.Add(user);
				}
				return result;
			}
		}

		public static User GetUser(Guid UID)
		{
			using (var context = DatabaseContext.Initialize())
			{
				return context.Users.Include(x=>x.UserPermissions).FirstOrDefault(x => x.UID == UID);
			}
		}

		public static void SaveUser(User user)
		{
			try
			{
				using (var context = DatabaseContext.Initialize())
				{
					var tableUser = context.Users.Include(x => x.UserPermissions).FirstOrDefault(x => x.UID == user.UID);
					if (tableUser != null)
					{
						context.UserPermissions.RemoveRange(tableUser.UserPermissions);
						tableUser.Login = user.Login;
						tableUser.Name = user.Name;
						tableUser.PasswordHash = user.PasswordHash;
						tableUser.UserPermissions = new List<UserPermission>();
						tableUser.UserPermissions.AddRange(user.UserPermissions.Select(x => new UserPermission { PermissionType = x.PermissionType, User = tableUser }));
					}
					else
						context.Users.Add(user);

					context.SaveChanges();

					if (tableUser == null)
						Users.Add(user);
					else
					{
						Users.RemoveAll(x => x.UID == user.UID);
						Users.Add(user);
					}
				}
			}

			catch(Exception e)
			{

				
			}
		}

		public static void DeleteUser(User user)
		{
			using (var context = DatabaseContext.Initialize())
			{
				var _user = context.Users.FirstOrDefault(x => x.UID == user.UID);
				context.Users.Remove(_user);
				context.SaveChanges();
				Users.Remove(user);
			}
		}

		public static string CheckLogin(string login, string password)
		{ 
			if (password == null)
				password = "";
			var _password = HashHelper.GetHashFromString(password);
			using (var context = DatabaseContext.Initialize())
			{
				var user = context.Users.Include(x=>x.UserPermissions).FirstOrDefault(x => x.PasswordHash == _password && x.Login == login);
				if (user== null)
					return "неверный логин или пароль";
				DBCash.CurrentUser = user;
			}
			return null;
		}

		public static List<Journal> GetJournal()
		{
			using (var context = DatabaseContext.Initialize())
			{
				return context.Journal.ToList();
			}
		}

		public static void SaveJournal(JournalType journalType)
		{
			var journalEvent = new Journal() { NameUser = CurrentUser.Name, DateTime = DateTime.Now, JurnalType = journalType };
			using (var context = DatabaseContext.Initialize())
			{
				context.Journal.Add(journalEvent);
				context.SaveChanges();
			}
		}


		public static List<Device> GetAllChildren(Device device, bool isWithSelf = true)
		{
			var result = new List<Device>();
			if (isWithSelf)
				result.Add(device);
			result.AddRange(device.Children.SelectMany(x => GetAllChildren(x)));
			return result;
		}

		public static bool CheckConnection()
		{
			try
			{
				using (var databaseContext = DatabaseContext.Initialize())
				{
					databaseContext.Measures.FirstOrDefault();
					return true;
				}
			}
			catch (Exception e)
			{
				return false;
			}
		}
 	}
}
