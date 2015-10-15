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
		public static Consumer RootConsumer { get; set; }
		public static List<User> Users { get; set; }
		public static User CurrentUser { get; set; }

		static DBCash()
		{
			RootDevice = GetRootDevice();
			if (RootDevice == null)
				CreateSystem();
			Users = GetAllUsers();
			RootConsumer = GetRootConsumer();
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
					userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.ViewConsumer});
					userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.ViewDevice});
					userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.EditConsumer});
					userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.EditDevice});
					userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.EditUser});
					userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.ViewUser});
					userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.ViewJournal});
					userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.ViewPlot});
					userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.ViewReport});
					userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.ViewTariff});
					userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.EditTariff});
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

						Users.RemoveAll(x => x.UID == user.UID);
						Users.Add(user);
					}
					else
					{
						context.Users.Add(user);
						Users.Add(user);
					}

					context.SaveChanges();

					//if (tableUser == null)
					//	Users.Add(user);
					//else
					//{
					//	Users.RemoveAll(x => x.UID == user.UID);
					//	Users.Add(user);
					//}
				}
			}

			catch(Exception e)
			{
				MessageBoxService.ShowException(e);
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
			try
			{
				using (var context = DatabaseContext.Initialize())
				{
					var user = context.Users.Include(x => x.UserPermissions).FirstOrDefault(x => x.PasswordHash == _password && x.Login == login);
					if (user == null)
						return "неверный логин или пароль";
					DBCash.CurrentUser = user;
				}
				return null;
			}
			catch (Exception e)
			{
				MessageBoxService.ShowException(e);
				return e.ToString();
			}
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
				MessageBoxService.ShowException(e);
				return null;
			}
		}

		static IQueryable<Journal> GetFiltered(Filter filter, DatabaseContext context)
		{
				IQueryable<Journal> result = context.Journal;
				if (filter.JournalTypes.Any())
					result = result.Where(x => filter.JournalTypes.Contains(x.JournalType));

				if (filter.ConsumerUIDs.Any())
					result = result.Where(x => filter.ConsumerUIDs.Contains(x.ObjectUID));
			
				if (filter.DeviceUIDs.Any())
					result = result.Where(x => filter.DeviceUIDs.Contains(x.ObjectUID));

				if (filter.UserUIDs.Any())
					result = result.Where(x => filter.UserUIDs.Contains(x.UserUID));

				if (filter.TariffUIDs.Any())
					result = result.Where(x => filter.TariffUIDs.Contains(x.ObjectUID));

					result = result.Where(x => x.DateTime > filter.StartDate && x.DateTime < filter.EndDate);
				
				if (filter.IsSortAsc)
					result = result.OrderBy(x => x.DateTime);
				else
					result = result.OrderByDescending(x => x.DateTime);

				return result;
		}

		public static void AddJournal(JournalType journalType, string Description = null)
		{
			var journalEvent = new Journal()
			{
				DateTime = DateTime.Now,
				JournalType = journalType,
				Description = Description
			};

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

		public static void AddJournalForUser(JournalType journalType, ModelBase modelBase = null, string Description = null)
		{
			var journalEvent = new Journal();
			if (modelBase != null)
			{
				if (modelBase is User)
					journalEvent.ClassType = ClassType.IsUser;
				if (modelBase is Consumer)
					journalEvent.ClassType = ClassType.IsConsumer;
				if (modelBase is Tariff)
					journalEvent.ClassType = ClassType.IsTariff;
				if (modelBase is Device)
					journalEvent.ClassType = ClassType.IsDevice;

					journalEvent.UserName = CurrentUser.Name;
					journalEvent.UserUID = CurrentUser.UID;
					journalEvent.DateTime = DateTime.Now;
					journalEvent.JournalType = journalType;
					journalEvent.ObjectUID = modelBase.UID;
					journalEvent.ObjectName = modelBase.Name;
					journalEvent.Description = Description;
			}
			else 
			{
				journalEvent.UserName = CurrentUser.Name;
				journalEvent.UserUID = CurrentUser.UID;
				journalEvent.DateTime = DateTime.Now;
				journalEvent.JournalType = journalType;
				journalEvent.Description = Description;
			}

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

		public static void AddJournal(JournalType journalType, Guid? objectUID, string ObjectName = null, string Description = null)
		{
			AddJournal(journalType, CurrentUser.UID, objectUID, CurrentUser.Name, ObjectName, Description);
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
