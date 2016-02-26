using System;
using System.Linq;

namespace RubezhDAL.DataClasses
{
	public class GKCardTranslator
	{
		DatabaseContext Context;

		public GKCardTranslator(DbService databaseService)
		{
			Context = databaseService.Context;
		}

		public int GetGKNoByCardNo(string gkIPAddress, uint cardNo)
		{
			foreach (var card in Context.GKCards)
			{
				System.Diagnostics.Trace.WriteLine(card.IpAddress + " " + (uint)card.CardNo);
			}
			var gkCard = Context.GKCards.FirstOrDefault(x => x.IpAddress == gkIPAddress && x.CardNo == (int)cardNo);
			if (gkCard != null)
			{
				return gkCard.GKNo;
			}
			return -1;
		}

		public int GetCardNoByGKNo(string gkIPAddress, int gkNo)
		{
			var gkCard = Context.GKCards.FirstOrDefault(x => x.IpAddress == gkIPAddress && x.GKNo == gkNo);
			if (gkCard != null)
			{
				return gkCard.CardNo;
			}
			return -1;
		}

		public int GetFreeGKNo(string gkIPAddress, uint cardNo, out bool isNew)
		{
			var gkCard = Context.GKCards.FirstOrDefault(x => x.IpAddress == gkIPAddress && x.CardNo == (int)cardNo && x.IsActive);
			if (gkCard != null)
			{
				isNew = false;
				return gkCard.GKNo;
			}
			gkCard = Context.GKCards.OrderBy(x => x.GKNo).FirstOrDefault(x => x.IpAddress == gkIPAddress && !x.IsActive);
			if (gkCard != null)
			{
				isNew = false;
				return gkCard.GKNo;
			}
			if (Context.GKCards.Where(x => x.IpAddress == gkIPAddress).Count() > 0)
			{
				isNew = true;
				return Context.GKCards.Where(x => x.IpAddress == gkIPAddress).Max(x => x.GKNo) + 1;
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
			var gkCard = Context.GKCards.FirstOrDefault(x => x.IpAddress == gkIPAddress && x.GKNo == gkNo);
			if (gkCard != null)
			{
				gkCard.CardNo = (int)cardNo;
				gkCard.FIO = employeeName;
				gkCard.IsActive = true;
				Context.SaveChanges();
			}
			else
			{
				gkCard = new GKCard()
				{
					UID = Guid.NewGuid(),
					IpAddress = gkIPAddress,
					GKNo = gkNo,
					CardNo = (int)cardNo,
					FIO = employeeName,
					IsActive = true
				};
				Context.GKCards.Add(gkCard);
				Context.SaveChanges();
			}
		}

		public void Remove(string gkIPAddress, int gkNo, uint cardNo)
		{
			if (string.IsNullOrEmpty(gkIPAddress))
			{
				return;
			}
			var gkCard = Context.GKCards.FirstOrDefault(x => x.IpAddress == gkIPAddress && x.GKNo == gkNo);
			if (gkCard != null)
			{
				gkCard.CardNo = (int)cardNo;
				gkCard.FIO = "Удален";
				gkCard.IsActive = false;
			}
			Context.SaveChanges();
		}

		public void RemoveAll(string gkIPAddress, int cardsCount)
		{
			if (!string.IsNullOrEmpty(gkIPAddress))
			{
				var gkCards = Context.GKCards.Where(x => x.IpAddress == gkIPAddress);
				if (gkCards != null)
				{
					Context.GKCards.RemoveRange(gkCards);
				}
				for (int no = 1; no < cardsCount + 1; no++)
				{
					var gkCard = new GKCard()
					{
						UID = Guid.NewGuid(),
						IpAddress = gkIPAddress,
						GKNo = no,
						CardNo = (int)0,
						FIO = "",
						IsActive = false
					};
					Context.GKCards.Add(gkCard);
				}
				Context.SaveChanges();
			}
		}
	}
}
