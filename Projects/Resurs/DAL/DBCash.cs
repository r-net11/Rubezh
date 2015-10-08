using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Runtime.Serialization;
using Common;
using Infrastructure.Common.Windows;

namespace ResursDAL
{
	public static partial class DBCash
	{
		public static Apartment RootApartment { get; set; }
		public static List<User> Users { get; set; }
		public static User CurrentUser { get; set; }
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
			Tariffs = ReadAllTariffs();
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

		public static int? GetJournalCount(Filter filter)
		{
			try
			{
				using (var context = DatabaseContext.Initialize())
				{
					return GetFiltered(filter, context).Count();
				}
			}

			catch (Exception e)
			{
				MessageBoxService.ShowException(e);
				return null;
			}
		}

		public static List<Journal> GetJournalPage(Filter  filter, int page)
		{
			try
			{

				using (var context = DatabaseContext.Initialize())
				{
					var journalItems = GetFiltered(filter, context);
					return journalItems.Skip((page - 1) * filter.PageSize).Take(filter.PageSize).ToList();
				}
			}
			catch(Exception e)
			{
				return null;
			}
		}

		static IQueryable<Journal> GetFiltered(Filter filter, DatabaseContext context)
		{
				IQueryable<Journal> result = context.Journal;
				if (filter.JournalTypes.Any())
				{
					var names = filter.JournalTypes.Select(x => x).ToList();
					result = result.Where(x => names.Contains(x.JournalType));
				}
				
					result = result.Where(x => x.DateTime > filter.StartDate && x.DateTime < filter.EndDate);
				
				if (filter.IsSortAsc)
				{
						result = result.OrderBy(x => x.DateTime);
				}
				else
				{
						result = result.OrderByDescending(x => x.DateTime);
				}
				return result;
		}

		public static void AddJournal(JournalType journalType, Guid? objectUID , string UserName = null , string ObjectName = null )
		{
			var journalEvent = new Journal() { UserName = UserName, DateTime = DateTime.Now, JournalType = journalType, ObjectUID = objectUID };
			try
			{
				using (var context = DatabaseContext.Initialize())
				{
					context.Journal.Add(journalEvent);
					context.SaveChanges();
				}
			}
			catch (Exception e)
			{
				MessageBoxService.ShowException(e);
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
