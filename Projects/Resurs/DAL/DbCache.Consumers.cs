using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Infrastructure.Common.Windows;
using System.Diagnostics;
using System.Data.Objects;
using System.Data.Entity.Infrastructure;

namespace ResursDAL
{
	public static partial class DbCache
	{
		public static Consumer GetConsumer(Guid consumerUID)
		{
			using (var context = DatabaseContext.Initialize())
			{
				var consumer = context.Consumers.FirstOrDefault(x => x.UID == consumerUID);
				consumer.Devices.AddRange(context.Devices.Where(x => x.ConsumerUID == consumerUID));
				consumer.Devices.ForEach(x => x.Driver = DriversConfiguration.Drivers.FirstOrDefault(y => y.UID == x.DriverUID));
				consumer.Deposits.AddRange(context.Deposits.Where(x => x.ConsumerUID == consumerUID));
				return consumer;
			}
		}

		public static List<Consumer> GetAllConsumers()
		{
			using (var context = DatabaseContext.Initialize())
			{
				return context.Consumers.ToList();
			}
		}

		static Consumer GetRootConsumer()
		{
			try
			{
				using (var context = DatabaseContext.Initialize())
				{
					var consumers = context.Consumers.Select(x => new
					{
						UID = x.UID,
						Name = x.Name,
						IsFolder = x.IsFolder,
						Description = x.Description,
						Address = x.Address,
						ParentUID = x.ParentUID,
						Number = x.Number
					})
					.ToList()
					.Select(x => new Consumer()
					{
						UID = x.UID,
						Name = x.Name,
						IsFolder = x.IsFolder,
						Description = x.Description,
						Address = x.Address,
						ParentUID = x.ParentUID,
						Number = x.Number
					})
					.ToList();

					var folders = consumers.Where(x => x.IsFolder).ToList();
					foreach (var sameParent in consumers.GroupBy(x => x.ParentUID))
					{
						Consumer parent = null;
						foreach (var item in folders)
							if (item.UID == sameParent.Key)
							{
								parent = item;
								break;
							}
						if (parent != null)
							foreach (var item in sameParent.OrderByDescending(x => x.IsFolder).ThenBy(x => x.Name))
							{
								item.Parent = parent;
								parent.Children.Add(item);
							}
					}

					var root = consumers.FirstOrDefault(x => x.Parent == null);

					if (root == null)
					{
						root = context.Consumers.Add(new Consumer() { Name = "Лицевые счета", IsFolder = true });
						context.SaveChanges();
					}

					return root;
				}
			}
			catch (Exception e)
			{
				MessageBoxService.Show(e.Message);
				return null;
			}
		}

		public static void CreateConsumers()
		{
			if (RootConsumer == null)
				RootConsumer = GetRootConsumer();
			using (var context = DatabaseContext.Initialize())
			{
				context.Configuration.AutoDetectChangesEnabled = false;
				Random random = new Random();
				for (int x = 1; x <= 10; x++)
				{
					var a = new Consumer
					{ 
						ParentUID = RootConsumer.UID, 
						IsFolder = true, 
						Name = "ДОМ №" + x, 
						Description = 
						"Описание дома №" + x 
					};
					context.Consumers.Add(a);
					for (int y = 1; y <= 4; y++)
					{
						var b = new Consumer 
						{ 
							ParentUID = a.UID, 
							IsFolder = true, 
							Name = "Подъезд №" + y, 
							Description = "Описание подъезда №" + y
						};
						context.Consumers.Add(b);
						for (int z = (y - 1) * 50 + 1; z <= y * 50; z++)
						{
							var c = new Consumer
							{
								ParentUID = b.UID,
								Name = "Квартира №" + z,
								Address = string.Format("410012, г. Саратов, ул. Московская, д. {0}, кв. {1}", x, z),
								Phone = string.Format("(8452) {0:00-00-00}", random.Next(200000, 799999)),
								Password = "password" + x + y + z,
								Login = "login" + x + y + z,
								Number = string.Format("{0:00000000}", random.Next(99999999)),
								Email = "consumer" + x + y + z + "@gmail.com",
								IsSendEmail = random.Next(2) == 1,
								Description = "Описание квартриры №" + z,
								Balance = (decimal)random.Next(-500000, 2000000) / 100
							};
							context.Consumers.Add(c);
						}
					}
				}
				context.ChangeTracker.DetectChanges();
				context.SaveChanges();
			}

		}

		public static void SaveConsumer(Consumer consumer)
		{
			using (var context = DatabaseContext.Initialize())
			{
				var dbConsumer = context.Consumers.Where(x => x.UID == consumer.UID).FirstOrDefault();
								
				if (dbConsumer == null)
				{
					dbConsumer = context.Consumers.Add(consumer);
					foreach (var device in consumer.Devices)
					{
						var dbDevice = context.Devices.FirstOrDefault(x => x.UID == device.UID);
						if (dbDevice != null)
							dbDevice.ConsumerUID = consumer.UID;
					}
				}
				else
				{
					dbConsumer.Devices.AddRange(context.Devices.Where(x => x.ConsumerUID == consumer.UID));
					dbConsumer.Address = consumer.Address;
					dbConsumer.Description = consumer.Description;
					dbConsumer.Email = consumer.Email;
					dbConsumer.Number = consumer.Number;
					dbConsumer.IsFolder = consumer.IsFolder;
					dbConsumer.IsSendEmail = consumer.IsSendEmail;
					dbConsumer.Login = consumer.Login;
					dbConsumer.Name = consumer.Name;
					dbConsumer.Parent = consumer.Parent;
					dbConsumer.Password = consumer.Password;
					dbConsumer.Phone = consumer.Phone;
					dbConsumer.ParentUID = consumer.ParentUID;

					dbConsumer.Devices.Except(consumer.Devices).ToList().ForEach(x => x.ConsumerUID = null);
					
					foreach (var device in consumer.Devices.Except(dbConsumer.Devices).ToList())
					{
						var dbDevice = context.Devices.FirstOrDefault(x => x.UID == device.UID);
						if (dbDevice != null)
							dbDevice.ConsumerUID = consumer.UID;
					}

				}
				dbConsumer.Parent = dbConsumer.ParentUID == null ? null : context.Consumers.FirstOrDefault(x => x.UID == dbConsumer.ParentUID);
				context.SaveChanges();
			}
		}
		
		public static void DeleteConsumer(Consumer consumer)
		{
			using (var context = DatabaseContext.Initialize())
			{
				var dbConsumer = context.Consumers.Where(x => x.UID == consumer.UID).FirstOrDefault();
				if (dbConsumer != null)
				{
					context.Devices.Where(x => x.ConsumerUID == dbConsumer.UID).ToList().ForEach(x => x.ConsumerUID = null);
					context.Consumers.Remove(dbConsumer);
				}
				context.SaveChanges();
			}
		}
	}
}
