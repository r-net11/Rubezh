using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Infrastructure.Common.Windows;

namespace ResursDAL
{
	public static partial class DBCash
	{
		public static Consumer GetConsumer(Guid consumerUid, bool withContent = false)
		{
			using (var context = DatabaseContext.Initialize())
			{
				return withContent ? 
					context.Consumers.Where(x => x.UID == consumerUid).Include(x => x.Bills).FirstOrDefault() : 
					context.Consumers.FirstOrDefault(x => x.UID == consumerUid);
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
						for (int x = 1; x <= 50; x++)
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
				consumer.Parent = consumer.Parent == null ? null : context.Consumers.FirstOrDefault(x => x.UID == consumer.Parent.UID);
				var dbConsumer = context.Consumers.FirstOrDefault(x => x.UID == consumer.UID);
				if (dbConsumer == null)
				{

					dbConsumer = context.Consumers.Add(consumer);
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
				}
				context.SaveChanges();
			}
		}

		public static void DeleteConsumer(Consumer consumer)
		{
			using (var context = DatabaseContext.Initialize())
			{
				context.Consumers.Remove(context.Consumers.FirstOrDefault(x => x.UID == consumer.UID));
				context.SaveChanges();
			}
		}
	}
}
