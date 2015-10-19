using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursDAL
{
	public static partial class DBCash
	{
		public static List<Receipt> GetAllReceipts()
		{
			using (var context = DatabaseContext.Initialize())
			{
				return context.Receipts.ToList();
			}
		}
		public static void SaveReceipt(Receipt receipt)
		{
			using (var context = DatabaseContext.Initialize())
			{
				var dbReceipt = context.Receipts.Where(x => x.UID == receipt.UID).FirstOrDefault();
				if (dbReceipt == null)
				{
					dbReceipt = context.Receipts.Add(receipt);
				}
				else
				{
					dbReceipt.Name = receipt.Name;
					dbReceipt.Description = receipt.Description;
					dbReceipt.Template = receipt.Template;
				}
				context.SaveChanges();
			}
		}
		public static void DeleteReceiptByUid(Guid receiptUid)
		{
			using (var context = DatabaseContext.Initialize())
			{
				var dbReceipt = context.Receipts.Where(x => x.UID == receiptUid).FirstOrDefault();
				if (dbReceipt != null)
				{
					context.Receipts.Remove(dbReceipt);
				}
				context.SaveChanges();
			}
			
		}
	}
}