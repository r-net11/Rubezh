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
#if DEBUG
						for (int x = 1; x <= 5; x++)
						{
							var a = new Consumer() { Parent = root, IsFolder = true, Name = "ДОМ №" + x, Description = "description !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" };
							context.Consumers.Add(a);
							for (int y = 1; y <= 4; y++)
							{
								var b = new Consumer() { Parent = a, IsFolder = true, Name = "Подъезд №" + y, Description = "description !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" };
								context.Consumers.Add(b);
								for (int z = 1; z <= 50; z++)
								{
									var c = new Consumer() { Parent = b, Name = "Квартира №" + z, Address = "address !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", Phone = "phone !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", Password = "password !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", Login = "login !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", FIO = "fio !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", Email = "email !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", Description = "description !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" };
									context.Consumers.Add(c);
								}
							}
						}
#endif
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
