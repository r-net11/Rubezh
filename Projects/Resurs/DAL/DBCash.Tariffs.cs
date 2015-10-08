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
				var list = context.Tariffs.Select(x => new { UID = x.UID, Name = x.Name, Devices = x.Devices, Description = x.Description, TariffParts = x.TariffParts, TariffType = x.TariffType, IsDiscount = x.IsDiscount }).ToList();
				List<Tariff> result = new List<Tariff>();
				foreach (var item in list)
				{
					result.Add(new Tariff
					{
						UID = item.UID,
						Name = item.Name,
						Description = item.Description,
						IsDiscount = item.IsDiscount,
						TariffType = item.TariffType,
						Devices = item.Devices,
						TariffParts = item.TariffParts,
					});
				}
				return result;
			}
		}
		public static void UpdateTariff(Tariff tariff)
		{
			using (var context = DatabaseContext.Initialize())
			{
				var tariffTableLink = context.Tariffs.Include(x => x.TariffParts).FirstOrDefault(x => x.UID == tariff.UID);
				var tariffEntity = context.Tariffs.FirstOrDefault(x => x.UID == tariff.UID);
				if (tariffEntity != null)
				{
					context.TariffParts.RemoveRange(tariffEntity.TariffParts);
					tariffEntity.UID = tariff.UID;
					tariffEntity.Name = tariff.Name;
					tariffEntity.Description = tariff.Description;
					tariffEntity.TariffType = tariff.TariffType;
					tariffEntity.IsDiscount = tariff.IsDiscount;
					tariffEntity.Devices = new List<Device>();
					//tariffEntity.Devices = tariff.Devices;
					tariffEntity.TariffParts = new List<TariffPart>();
					tariffEntity.TariffParts.AddRange(tariff.TariffParts
						.Select(x => new TariffPart { 
							UID = x.UID, 
							Price = x.Price, 
							Threshold = x.Threshold,
							Discount = x.Discount, 
							StartTime = x.StartTime,
							Tariff = tariffTableLink, 
						}));
					context.SaveChanges();
				}
			}
			Tariffs.RemoveAll(x => x.UID == tariff.UID);
			Tariffs.Add(tariff);
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
