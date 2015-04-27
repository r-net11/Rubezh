using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SKDDriver.DataAccess;
using FiresecAPI.GK;

namespace SKDDriver
{
	public class GKCardTranslator
	{
		DataAccess.SKDDataContext Context;

		public GKCardTranslator(SKDDatabaseService databaseService)
		{
			Context = databaseService.Context;
		}

		public int GetGKNoByCardNo(string gkIPAddress, uint cardNo)
		{
			foreach (var card in Context.GKCards)
			{
				System.Diagnostics.Trace.WriteLine(card.IPAddress + " " + (uint)card.CardNo);
			}
			var gkCard = Context.GKCards.FirstOrDefault(x => x.IPAddress == gkIPAddress && x.CardNo == (int)cardNo);
			if (gkCard != null)
			{
				return gkCard.GKNo;
			}
			return -1;
		}

		public int GetCardNoByGKNo(string gkIPAddress, int gkNo)
		{
			var gkCard = Context.GKCards.FirstOrDefault(x => x.IPAddress == gkIPAddress && x.GKNo == gkNo);
			if (gkCard != null)
			{
				return gkCard.CardNo;
			}
			return -1;
		}

		public int GetFreeGKNo(string gkIPAddress, uint cardNo, out bool isNew)
		{
			var gkCard = Context.GKCards.FirstOrDefault(x => x.IPAddress == gkIPAddress && (uint)x.CardNo == cardNo);
			if (gkCard != null)
			{
				isNew = false;
				return gkCard.GKNo;
			}
			gkCard = Context.GKCards.FirstOrDefault(x => x.IPAddress == gkIPAddress && !x.IsActive);
			if (gkCard != null)
			{
				isNew = false;
				return gkCard.GKNo;
			}
			if (Context.GKCards.Where(x => x.IPAddress == gkIPAddress).Count() > 0)
			{
				isNew = true;
				return Context.GKCards.Where(x => x.IPAddress == gkIPAddress).Max(x => x.GKNo) + 1;
			}
			else
			{
				isNew = true;
				return 1;
			}
		}

		public void AddOrEdit(string gkIPAddress, int gkNo, uint cardNo, string employeeName)
		{
			if (string.IsNullOrEmpty(gkIPAddress))
			{
				return;
			}
			var gkCard = Context.GKCards.FirstOrDefault(x => x.IPAddress == gkIPAddress && x.GKNo == gkNo);
			if (gkCard != null)
			{
				gkCard.CardNo = (int)cardNo;
				gkCard.FIO = employeeName;
				gkCard.IsActive = true;
				Context.SubmitChanges();
			}
			else
			{
				gkCard = new GKCard()
				{
					UID = Guid.NewGuid(),
					IPAddress = gkIPAddress,
					GKNo = gkNo,
					CardNo = (int)cardNo,
					FIO = employeeName,
					IsActive = true
				};
				Context.GKCards.InsertOnSubmit(gkCard);
				Context.SubmitChanges();
			}
		}

		public void Remove(string gkIPAddress, int gkNo, uint cardNo)
		{
			if (string.IsNullOrEmpty(gkIPAddress))
			{
				return;
			}
			var gkCard = Context.GKCards.FirstOrDefault(x => x.IPAddress == gkIPAddress && x.GKNo == gkNo);
			if (gkCard != null)
			{
				gkCard.CardNo = (int)cardNo;
				gkCard.FIO = "Удален";
				gkCard.IsActive = false;
			}
			Context.SubmitChanges();
		}

		public void RemoveAll(string gkIPAddress)
		{
			if (!string.IsNullOrEmpty(gkIPAddress))
			{
				var gkCards = Context.GKCards.Where(x => x.IPAddress == gkIPAddress);
				if (gkCards != null)
				{
					Context.GKCards.DeleteAllOnSubmit(gkCards);
				}
				Context.SubmitChanges();
			}
		}
	}
}