using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Infrastructure.Common.Windows.Windows;

namespace ResursDAL
{
	public static partial class DBCash
	{
		public static Bill GetBill(Guid billUid)
		{
			using (var context = DatabaseContext.Initialize())
			{
				return context.Bills.FirstOrDefault(x => x.UID == billUid); 
			}
		}

		public static List<Bill> GetAllBills()
		{
			using (var context = DatabaseContext.Initialize())
			{
				return context.Bills.ToList(); 
			}
		}	
		public static void SaveBill(Bill bill)
		{
			using (var context = DatabaseContext.Initialize())
			{
				var dbBill = context.Bills.FirstOrDefault(x => x.UID == bill.UID);
				if (dbBill == null)
				{
					dbBill = context.Bills.Add(bill);
				}
				else
				{
					dbBill.Name = bill.Name;
					dbBill.Description = bill.Description;
					dbBill.Balance = bill.Balance;
					dbBill.TemplatePath = bill.TemplatePath;
					dbBill.ReceiptUid = bill.ReceiptUid;
				}
				context.SaveChanges();
			}
		}

		public static void DeleteBill(Bill bill)
		{
			using (var context = DatabaseContext.Initialize())
			{
				context.Bills.Remove(context.Bills.FirstOrDefault(x => x.UID == bill.UID));
				context.SaveChanges();
			}
		}
	}
}
