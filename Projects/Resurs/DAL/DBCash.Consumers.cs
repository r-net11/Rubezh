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
	public static partial class DBCash
	{
		public static Consumer GetConsumer(Guid consumerUid)
		{
			using (var context = DatabaseContext.Initialize())
			{
				var consumer = context.Consumers.Where(x => x.UID == consumerUid).Include(x => x.Bills).FirstOrDefault();
				if (consumer != null)
				{
					foreach (var bill in consumer.Bills)
						bill.Devices.AddRange(context.Devices.Where(x => x.BillUID == bill.UID).ToList().Select(x => DBCash.GetDevice(x.UID)));
				}
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
						ParentUID = x.ParentUID
					})
					.ToList()
					.Select(x => new Consumer()
					{
						UID = x.UID,
						Name = x.Name,
						IsFolder = x.IsFolder,
						Description = x.Description,
						Address = x.Address,
						ParentUID = x.ParentUID
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
						root = context.Consumers.Add(new Consumer() { Name = "Абоненты", IsFolder = true });
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
						for (int z = 1; z <= 50; z++)
						{
							var c = new Consumer
							{
								ParentUID = b.UID,
								Name = "Квартира №" + z,
								Address = string.Format("410012, г. Саратов, ул. Московская, д. {0}, кв. {1}", x, z),
								Phone = "(8452) " + string.Format("{0:00-00-00}", random.Next(200000, 799999)),
								Password = "password" + x + y + z,
								Login = "login" + x + y + z,
								FIO = "Иванов Петр Сидорович",
								Email = "consumer" + x + y + z + "@gmail.com",
								IsSendEmail = random.Next(2) == 1,
								Description = "Описание квартриры №" + z,
							};
							for (int i = 0; i < random.Next(1, 3); i++)
							{
								string billName =  string.Format("{0:00000000}", random.Next(99999999));
								c.Bills.Add(new Bill
								{
									Balance = (decimal)random.Next(-500000, 2000000) / 100,
									Consumer = c,
									Name = billName,
									Description = "Счет " + billName,
									TemplatePath = "D:\\RubezhResurs\\Templates\\Template01.xml"
								});
							}
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
				var dbConsumer = context.Consumers.Where(x => x.UID == consumer.UID).Include(x => x.Bills).FirstOrDefault();//GetConsumer(consumer.UID);//

				if (dbConsumer == null)
				{
					dbConsumer = context.Consumers.Add(consumer);
					foreach (var bill in consumer.Bills)
						foreach ( var device in bill.Devices)
						{
							var dbDevice = context.Devices.FirstOrDefault(x => x.UID == device.UID);
							if (dbDevice != null)
								dbDevice.BillUID = bill.UID;
						}
				}
				else
				{
					dbConsumer.Address = consumer.Address;
					dbConsumer.Description = consumer.Description;
					dbConsumer.Email = consumer.Email;
					dbConsumer.FIO = consumer.FIO;
					dbConsumer.IsFolder = consumer.IsFolder;
					dbConsumer.IsSendEmail = consumer.IsSendEmail;
					dbConsumer.Login = consumer.Login;
					dbConsumer.Name = consumer.Name;
					dbConsumer.Parent = consumer.Parent;
					dbConsumer.Password = consumer.Password;
					dbConsumer.Phone = consumer.Phone;
					dbConsumer.ParentUID = consumer.ParentUID;

					var devicesToUpdate = new Dictionary<Guid, bool>();
					var billsToDelete = context.Bills
						.Where(x => x.Consumer.UID == dbConsumer.UID)
						.ToList()
						.Where(x => !consumer.Bills.Any(y => x.UID == y.UID))
						.ToList();
					foreach (var billToDelete in billsToDelete)
					{
						context.Devices.Where(x => x.BillUID == billToDelete.UID).ToList().ForEach(x => devicesToUpdate.Add(x.UID, false));
						context.Bills.Remove(billToDelete);
					}

					foreach (var sourceBill in consumer.Bills)
					{
						var targetBill = context.Bills.FirstOrDefault(x => x.UID == sourceBill.UID);
						if (targetBill == null)
						{
							context.Bills.Add(sourceBill);
							dbConsumer.Bills.Add(sourceBill);
						}
						else
						{
							targetBill.Balance = sourceBill.Balance;
							targetBill.Description = sourceBill.Description;
							targetBill.Name = sourceBill.Name;
							targetBill.TemplatePath = sourceBill.TemplatePath;
						}
					}

					foreach (var dbBill in dbConsumer.Bills)
					{
						dbBill.Consumer = dbConsumer;

						var bill = consumer.Bills.FirstOrDefault(x => x.UID == dbBill.UID);
						if (bill != null)
							foreach (var device in bill.Devices)
								if (devicesToUpdate.ContainsKey(device.UID))
									devicesToUpdate[device.UID] = true;
								else
									devicesToUpdate.Add(device.UID, true);

						foreach (var device in dbBill.Devices.Where(x => !bill.Devices.Any(y => x.UID == y.UID)).ToList())
							if (devicesToUpdate.ContainsKey(device.UID))
								devicesToUpdate[device.UID] = false;
							else
								devicesToUpdate.Add(device.UID, false);

						foreach (var update in devicesToUpdate)
						{
							var dbDevice = context.Devices.FirstOrDefault(x => x.UID == update.Key);
							if (dbDevice != null)
								dbDevice.BillUID = update.Value ? (Guid?)dbBill.UID : null;
						}
					}
				}
				dbConsumer.Parent = dbConsumer.ParentUID == null ? null : context.Consumers.FirstOrDefault(x => x.UID == dbConsumer.ParentUID);
				context.SaveChanges();
			}
		}

		static void MergeBills(List<Bill> source, List<Bill> target, DatabaseContext context)
		{
			target.RemoveAll(x => !source.Any(y => x.UID == y.UID));

			foreach (var sourceBill in source)
			{
				var targetBill = target.FirstOrDefault(x => x.UID == sourceBill.UID);
				if (targetBill == null)
				{
					target.Add(sourceBill);
				}
				else
				{
					targetBill.Balance = sourceBill.Balance;
					targetBill.Description = sourceBill.Description;
					targetBill.Name = sourceBill.Name;
					targetBill.TemplatePath = sourceBill.TemplatePath;
				}
			}
		}

		public static void DeleteConsumer(Consumer consumer)
		{
			using (var context = DatabaseContext.Initialize())
			{
				var dbConsumer = context.Consumers.Where(x => x.UID == consumer.UID).Include(x => x.Bills).FirstOrDefault();
				if (dbConsumer != null)
				{
					foreach (var dbBill in dbConsumer.Bills)
						context.Devices.Where(x => x.BillUID == dbBill.UID).ToList().ForEach(x => x.BillUID = null);
					context.Consumers.Remove(dbConsumer);
				}
				context.SaveChanges();
			}
		}
	}
}
