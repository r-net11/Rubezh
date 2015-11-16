using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace ResursDAL
{
	public static partial class DbCache
	{
		public static List<Tariff> Tariffs { get; set; }
		public static void CreateTariff(Tariff tariff)
		{
			using (var context = DatabaseContext.Initialize())
			{
				var devicesToUpdate = tariff.Devices;
				tariff.Devices = new List<Device>();
				var tariffEntity = context.Tariffs.Add(tariff);

				foreach (var device in devicesToUpdate)
				{
					var dbDevice = context.Devices.FirstOrDefault(x => x.UID == device.UID);
					if (dbDevice != null)
						dbDevice.TariffUID = tariff.UID;
				}
				context.SaveChanges();
			}
			Tariffs.Add(tariff);
		}

		public static void CreateTariffs(IEnumerable<Tariff> tariffs)
		{
			using (var context = DatabaseContext.Initialize())
			{
				foreach (var tariff in tariffs)
				{
					var devicesToUpdate = tariff.Devices;
					tariff.Devices = new List<Device>();
					var tariffEntity = context.Tariffs.Add(tariff);

					foreach (var device in devicesToUpdate)
					{
						var dbDevice = context.Devices.FirstOrDefault(x => x.UID == device.UID);
						if (dbDevice != null)
							dbDevice.TariffUID = tariff.UID;
					}
				}
				context.SaveChanges();
			}
			Tariffs.AddRange(tariffs);
		}

		public static Tariff ReadTariff(Guid id)
		{
			using (var context = DatabaseContext.Initialize())
			{
				return context.Tariffs.Include(x => x.Devices).Include(x => x.TariffParts).FirstOrDefault(x => x.UID == id);
			}
		}

		public static List<Tariff> ReadAllTariffs()
		{
			using (var context = DatabaseContext.Initialize())
			{
				return context.Tariffs.Include(x => x.Devices).Include(x => x.TariffParts).ToList();
			}
		}

		public static void UpdateTariff(Tariff tariff)
		{
			using (var context = DatabaseContext.Initialize())
			{
				var tariffEntity = context.Tariffs.Include(x => x.Devices).Include(x => x.TariffParts).FirstOrDefault(x => x.UID == tariff.UID);
				if (tariffEntity != null)
				{
					context.TariffParts.RemoveRange(tariffEntity.TariffParts);
					tariffEntity.UID = tariff.UID;
					tariffEntity.Name = tariff.Name;
					tariffEntity.Description = tariff.Description;
					tariffEntity.TariffType = tariff.TariffType;
					tariffEntity.IsDiscount = tariff.IsDiscount;
					tariffEntity.Devices = new List<Device>();
					var devicesToUpdate = tariff.Devices;
					foreach (var device in devicesToUpdate)
					{
						var dbDevice = context.Devices.FirstOrDefault(x => x.UID == device.UID);
						if (dbDevice != null)
							dbDevice.TariffUID = tariff.UID;
						tariffEntity.Devices.Add(dbDevice);
					}
					tariffEntity.TariffParts = new List<TariffPart>();
					tariffEntity.TariffParts.AddRange(tariff.TariffParts
						.Select(x => new TariffPart
						{
							UID = x.UID,
							Price = x.Price,
							Threshold = x.Threshold,
							Discount = x.Discount,
							StartTime = x.StartTime,
							Tariff = tariffEntity,
						}));
					context.SaveChanges();
				}
			}
			Tariffs.RemoveAll(x => x.UID == tariff.UID);
			Tariffs.Add(tariff);
		}

		//device is not deleted from dbcashe
		public static void DeleteTariff(Tariff tariff)
		{
			using (var context = DatabaseContext.Initialize())
			{
				var tmp = context.Tariffs.FirstOrDefault(x => x.UID == tariff.UID);
				foreach (var device in context.Devices)
				{
					device.Tariff = null;
				}
				context.Tariffs.Remove(tmp);
				context.SaveChanges();
			}
			Tariffs.Remove(tariff);
		}
	}
}
