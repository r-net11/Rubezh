using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Runtime.Serialization;
using Common;
using Infrastructure.Common.Windows;
using System.Threading;
using System.Diagnostics;

namespace ResursDAL
{
	public static partial class DbCache
	{
		public static List<User> Users { get; set; }
		public static User CurrentUser { get; set; }

		public static List<User> GetAllUsers()
		{
			try
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
						User user = new User() { Name = "Adm", Login = "Adm", PasswordHash = HashHelper.GetHashFromString("") };
						userpermissions.Add(new UserPermission { User = user, PermissionType = PermissionType.ViewConsumer });
						userpermissions.Add(new UserPermission { User = user, PermissionType = PermissionType.ViewDevice });
						userpermissions.Add(new UserPermission { User = user, PermissionType = PermissionType.EditConsumer });
						userpermissions.Add(new UserPermission { User = user, PermissionType = PermissionType.EditDevice });
						userpermissions.Add(new UserPermission { User = user, PermissionType = PermissionType.EditUser });
						userpermissions.Add(new UserPermission { User = user, PermissionType = PermissionType.ViewUser });
						userpermissions.Add(new UserPermission { User = user, PermissionType = PermissionType.ViewJournal });
						userpermissions.Add(new UserPermission { User = user, PermissionType = PermissionType.ViewPlot });
						userpermissions.Add(new UserPermission { User = user, PermissionType = PermissionType.ViewReport });
						userpermissions.Add(new UserPermission { User = user, PermissionType = PermissionType.ViewTariff });
						userpermissions.Add(new UserPermission { User = user, PermissionType = PermissionType.EditTariff });
						user.UserPermissions = userpermissions;
						context.Users.Add(user);
						context.SaveChanges();
						result.Add(user);
					}
					return result;
				}
			}
			catch (Exception e)
			{
				MessageBoxService.ShowException(e);
				return null;
			}
		}

		public static User GetUser(Guid UID)
		{
			try
			{
				using (var context = DatabaseContext.Initialize())
				{
					return context.Users.Include(x => x.UserPermissions).FirstOrDefault(x => x.UID == UID);
				}
			}
			catch (Exception e)
			{
				MessageBoxService.ShowException(e);
				return null;
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
				}
			}
			catch (Exception e)
			{
				MessageBoxService.ShowException(e);
			}
		}

		public static void DeleteUser(User user)
		{
			try
			{
				using (var context = DatabaseContext.Initialize())
				{
					var tableUser = context.Users.FirstOrDefault(x => x.UID == user.UID);
					context.Users.Remove(tableUser);
					context.SaveChanges();
					Users.Remove(user);
				}
			}
			catch(Exception e)
			{
				MessageBoxService.ShowException(e);
			}
		}
		public static string CheckLogin(string login, string password)
		{
			if (password == null)
				password = "";
			var passwordHash = HashHelper.GetHashFromString(password);
			try
			{
				using (var context = DatabaseContext.Initialize())
				{
					var user = context.Users.Include(x => x.UserPermissions).FirstOrDefault(x => x.PasswordHash == passwordHash && x.Login == login);
					if (user == null)
						return "неверный логин или пароль";
					DbCache.CurrentUser = user;
				}
				return null;
			}
			catch (Exception e)
			{
				MessageBoxService.ShowException(e);
				return e.ToString();
			}
		}
		public static bool CheckPermission(PermissionType permissionType)
		{
			if(CurrentUser!= null)
			{
				return CurrentUser.UserPermissions.Any(x=> x.PermissionType == permissionType);
			}
			return false;
		}

		public static void CreateUsers()
		{
			try
			{
				using (var context = DatabaseContext.Initialize())
				{
					context.Users.RemoveRange(context.Users);
					context.SaveChanges();
					Users = GetAllUsers();
					var users = new List<User>();
					var password = HashHelper.GetHashFromString("");
					for (int i = 1; i <= 50; i++)
					{
						users.Add(new User() { Name = "User" + i, Login = "User" + i, PasswordHash = password });
						Thread.Sleep(200);
						Random random = new Random();
						var permissons = new List<UserPermission>();
						for (int y = 0; y < random.Next(11); y++)
						{
							permissons.Add(new UserPermission() { User = users[i - 1], PermissionType = (PermissionType)y });
						}

						users[i - 1].UserPermissions = permissons;
					}

					context.Users.AddRange(users);
					context.SaveChanges();
					Users.AddRange(users);
				}
			}
			catch (Exception e)
			{
				MessageBoxService.ShowException(e);
			}
		}
	}
}