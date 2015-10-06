using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Runtime.Serialization;
using System.Data.SqlTypes;

namespace ResursDAL
{
	public static partial class DBCash
	{
		public static List<Tariff> Tariffs { get; set; }
		public static void CreateTariff(Tariff tariff)
		{
			using (var context = DatabaseContext.Initialize())
			{
				foreach (var item in tariff.TariffParts)
				{
					if(true)
					{
						item.StartTime = (DateTime)SqlDateTime.MinValue;
					}
				}


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

		public static List<Tariff> ReadAllTariffs()
		{
			using (var context = DatabaseContext.Initialize())
			{
				var list = context.Tariffs.Select(x => new { UID = x.UID, Name = x.Name, Devices = x.Devices, Description = x.Description, TariffParts = x.TariffParts, TariffType = x.TariffType }).ToList();
				List<Tariff> result = new List<Tariff>();
				foreach (var item in list)
				{
					result.Add(new Tariff
					{
						Description = item.Description,
						Devices = item.Devices,
						Name = item.Name,
						TariffParts = item.TariffParts,
						TariffType = item.TariffType,
						UID = item.UID,
					});
				}
				return result;
			}
		}
		public static void UpdateTariff(Tariff tariff)
		{
			//if (tableUser != null)
			//{
			//	context.UserPermissions.RemoveRange(tableUser.UserPermissions);
			//	tableUser.Login = user.Login;
			//	tableUser.Name = user.Name;
			//	tableUser.PasswordHash = user.PasswordHash;
			//	tableUser.UserPermissions = new List<UserPermission>();
			//	tableUser.UserPermissions.AddRange(user.UserPermissions.Select(x => new UserPermission { PermissionType = x.PermissionType, User = tableUser }));
			//}
		}

		public static void DeleteTariff(Tariff tariff)
		{
			using (var context = DatabaseContext.Initialize())
			{
				var tmp = context.Tariffs.FirstOrDefault(x => x.UID == tariff.UID);
				context.Tariffs.Remove(tmp);
				context.SaveChanges();
			}
			Tariffs.Remove(tariff);
		}
	}
}
