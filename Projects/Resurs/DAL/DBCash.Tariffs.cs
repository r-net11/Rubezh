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
					if (true)
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
			using (var context = DatabaseContext.Initialize())
			{
				var tariffEntity = context.Tariffs.FirstOrDefault(x => x.UID == tariff.UID);
				if (tariffEntity != null)
				{
					context.TariffParts.RemoveRange(tariffEntity.TariffParts);
					tariffEntity.Description = tariff.Description;
					tariffEntity.Name = tariff.Name;
					tariffEntity.TariffType = tariff.TariffType;
					tariffEntity.UID = tariff.UID;
					tariffEntity.Devices = new List<Device>();
					tariffEntity.Devices = tariff.Devices;
					tariffEntity.TariffParts = new List<TariffPart>();
					tariffEntity.TariffParts.AddRange(tariff.TariffParts.Select(x => new TariffPart { Discount = x.Discount, Price=x.Price, StartTime=x.StartTime, Tariff=tariff, UID = x.UID, Threshhold = x.Threshhold }));
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
