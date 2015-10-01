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
		public static Apartment RootApartment { get; set; }

		public static List<User> Users { get; set; }

		static DBCash()
		{
			RootDevice = GetRootDevice();
			if (RootDevice == null)
				CreateSystem();
			Users = GetAllUsers();

			RootApartment = new Apartment()
			{
				Name = "Жилой комплекс",
				IsFolder = true,
				Children = new List<Apartment>()
				{
					new Apartment()
					{
						Name = "Дом 1",
						IsFolder = true,
						Children = new List<Apartment>()
						{
							new Apartment() { Name = "Квартира 1" },
							new Apartment() { Name = "Квартира 2" },
							new Apartment() { Name = "Квартира 3" },
							new Apartment() { Name = "Квартира 4" },
							new Apartment() { Name = "Квартира 5" },
							new Apartment() { Name = "Квартира 6" },
							new Apartment() { Name = "Квартира 7" },
							new Apartment() { Name = "Квартира 8" },
						}
					},
					new Apartment()
					{
						Name = "Дом 2",
						IsFolder = true,
						Children = new List<Apartment>()
						{
							new Apartment() { Name = "Квартира 1" },
							new Apartment() { Name = "Квартира 2" },
							new Apartment() { Name = "Квартира 3" },
							new Apartment() { Name = "Квартира 4" },
							new Apartment() { Name = "Квартира 5" },
							new Apartment() { Name = "Квартира 6" },
							new Apartment() { Name = "Квартира 7" }, 
							new Apartment() { Name = "Квартира 8" },
							new Apartment() { Name = "Квартира 9" },
							new Apartment() { Name = "Квартира 10" },
							new Apartment() { Name = "Квартира 11" },
							new Apartment() { Name = "Квартира 12" },
							new Apartment() { Name = "Квартира 13" },
							new Apartment() { Name = "Квартира 14" },
							new Apartment() { Name = "Квартира 15" },
							new Apartment() { Name = "Квартира 66" },
							new Apartment() { Name = "Квартира 666" },
						}
					}
				}
			};
		}
		
		public static List<User> GetAllUsers()
		{
			using (var context = DatabaseContext.Initialize())
			{
				var shortUsers = context.Users.Select(x => new { UID = x.UID, Name = x.Name, Login = x.Login }).ToList();
				var result = new List<User>();
				foreach (var item in shortUsers)
				{
					 result.Add(new User
					 {
						 UID = item.UID, 
						 Name = item.Name, 
						 Login = item.Login
					 });
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
			using (var context = DatabaseContext.Initialize())
			{
				var tableUser = context.Users.FirstOrDefault(x => x.UID == user.UID);
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

		public static void DeleteUser(User user)
		{
			using (var context = DatabaseContext.Initialize())
			{
				context.Users.Remove(user);
				context.SaveChanges();
			}
			Users.Remove(user);
		}
		public static List<Device> GetAllChildren(Device device, bool isWithSelf = true)
		{
			var result = new List<Device>();
			if (isWithSelf)
				result.Add(device);
			result.AddRange(device.Children.SelectMany(x => GetAllChildren(x)));
			return result;
		}

		#region Tariffs CRUD
		public static List<Tariff> Tariffs { get; set; } 
		public static void CreateTariff(Tariff tariff)
		{
			using (var context = DatabaseContext.Initialize())
			{
				context.Tariffs.Add(tariff);
				context.SaveChanges();
			}
			Tariffs.Add(tariff);

		}

		public static Tariff ReadTariff(Guid id)
		{
			using (var context = DatabaseContext.Initialize())
			{
				return context.Tariffs.FirstOrDefault(x => x.UID == id);
			}
		}

		public static IEnumerable<Tariff> ReadAllTariffs()
		{
			using (var context = DatabaseContext.Initialize())
			{
				return context.Tariffs.Select(x => new Tariff { UID = x.UID, Name = x.Name }).ToList();
			}
		}

		public static void DeleteTariff(Tariff tariff)
		{
			using (var context = DatabaseContext.Initialize())
			{
				context.Tariffs.Remove(tariff);
				context.SaveChanges();
			}
			Tariffs.Remove(tariff);
		}

		public static Tariff UpdateTariff(Tariff tariff)
		{
			throw new NotImplementedException();
		} 
		#endregion
 	}
}
