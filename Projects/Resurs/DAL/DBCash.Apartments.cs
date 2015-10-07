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
		public static Apartment GetApartment(Guid apartmentUid, bool withContent = false)
		{
			using (var context = DatabaseContext.Initialize())
			{
				return withContent ? 
					context.Apartments.Where(x => x.UID == apartmentUid).Include(x => x.Bills).FirstOrDefault() : 
					context.Apartments.FirstOrDefault(x => x.UID == apartmentUid);
			}
		}

		public static List<Apartment> GetAllApartments()
		{
			using (var context = DatabaseContext.Initialize())
			{
				return context.Apartments.ToList();
			}
		}

		static Apartment GetRootApartment()
		{
			try
			{
				using (var context = DatabaseContext.Initialize())
				{
					var apartments = context.Apartments.Select(x => new
					{
						UID = x.UID,
						Name = x.Name,
						IsFolder = x.IsFolder,
						Description = x.Description,
						Address = x.Address,
						ParentUID = x.ParentUID
					})
					.ToList()
					.Select(x => new Apartment()
					{
						UID = x.UID,
						Name = x.Name,
						IsFolder = x.IsFolder,
						Description = x.Description,
						Address = x.Address,
						ParentUID = x.ParentUID
					})
					.ToList();

					var folders = apartments.Where(x => x.IsFolder).ToList();
					foreach (var sameParent in apartments.GroupBy(x => x.ParentUID))
					{
						Apartment parent = null;
						foreach (var item in folders)
							if (item.UID == sameParent.Key)
							{
								parent = item;
								break;
							}
						if (parent != null)
							foreach (var item in sameParent)
							{
								item.Parent = parent;
								parent.Children.Add(item);
							}
					}

					var root = apartments.FirstOrDefault(x => x.Parent == null);

					if (root == null)
					{
						root = context.Apartments.Add(new Apartment() { Name = "Абоненты", IsFolder = true });
#if DEBUG
						for (int x = 0; x < 50; x++)
						{
							var a = new Apartment() { Parent = root, IsFolder = true, Name = "ДОМ №" + x, Description = "description !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" };
							context.Apartments.Add(a);
							for (int y = 0; y < 4; y++)
							{
								var b = new Apartment() { Parent = a, IsFolder = true, Name = "Подъезд №" + y, Description = "description !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" };
								context.Apartments.Add(b);
								for (int z = 0; z < 50; z++)
								{
									var c = new Apartment() { Parent = b, Name = "Квартира №" + z, Address = "address !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", Phone = "phone !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", Password = "password !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", Login = "login !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", FIO = "fio !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", Email = "email !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", Description = "description !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" };
									context.Apartments.Add(c);
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

		public static void SaveApartment(Apartment apartment)
		{
			using (var context = DatabaseContext.Initialize())
			{
				apartment.Parent = apartment.Parent == null ? null : context.Apartments.FirstOrDefault(x => x.UID == apartment.Parent.UID);
				var dbApartment = context.Apartments.FirstOrDefault(x => x.UID == apartment.UID);
				if (dbApartment == null)
				{

					dbApartment = context.Apartments.Add(apartment);
				}
				else
				{
					dbApartment.Address = apartment.Address;
					dbApartment.Description = apartment.Description;
					dbApartment.Email = apartment.Email;
					dbApartment.FIO = apartment.FIO;
					dbApartment.IsFolder = apartment.IsFolder;
					dbApartment.IsSendEmail = apartment.IsSendEmail;
					dbApartment.Login = apartment.Login;
					dbApartment.Name = apartment.Name;
					dbApartment.Parent = apartment.Parent;
					dbApartment.Password = apartment.Password;
					dbApartment.Phone = apartment.Phone;
				}
				context.SaveChanges();
			}
		}

		public static void DeleteApartment(Apartment apartment)
		{
			using (var context = DatabaseContext.Initialize())
			{
				context.Apartments.Remove(context.Apartments.FirstOrDefault(x => x.UID == apartment.UID));
				context.SaveChanges();
			}
		}
	}
}
