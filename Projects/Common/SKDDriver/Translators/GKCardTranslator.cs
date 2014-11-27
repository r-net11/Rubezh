using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SKDDriver.DataAccess;

namespace SKDDriver
{
	public class GKCardTranslator
	{
		DataAccess.SKDDataContext Context;

		public GKCardTranslator(SKDDatabaseService databaseService)
		{
			Context = databaseService.Context;
		}

		public int GetFreeNo(string gkIPAddress, out bool isNew)
		{
			var gkCard = Context.GKCards.FirstOrDefault(x => x.IPAddress == gkIPAddress && x.Action == 0);
			if (gkCard != null)
			{
				isNew = false;
				return gkCard.No;
			}
			if (Context.GKCards.Where(x => x.IPAddress == gkIPAddress).Count() > 0)
			{
				isNew = true;
				return Context.GKCards.Where(x => x.IPAddress == gkIPAddress).Max(x => x.No) + 1;
			}
			else
			{
				isNew = true;
				return 1;
			}
		}

		public void AddOrEdit(string gkIPAddress, int no, int cardNo)
		{
			if (string.IsNullOrEmpty(gkIPAddress))
			{
				return;
			}
			var gkCard = Context.GKCards.FirstOrDefault(x => x.IPAddress == gkIPAddress && x.No == no);
			if (gkCard != null)
			{
				gkCard.CardNo = cardNo;
				gkCard.Action = 1;
				Context.SubmitChanges();
			}
			else
			{
				gkCard = new GKCard()
				{
					UID = Guid.NewGuid(),
					IPAddress = gkIPAddress,
					No = no,
					CardNo = cardNo,
					Action = 1
				};
				Context.GKCards.InsertOnSubmit(gkCard);
				Context.SubmitChanges();
			}
		}

		public void Remove(string gkIPAddress, int no)
		{
			if (string.IsNullOrEmpty(gkIPAddress))
			{
				return;
			}
			var gkCard = Context.GKCards.FirstOrDefault(x => x.IPAddress == gkIPAddress && x.No == no);
			if (gkCard != null)
			{
				gkCard.Action = 0;
			}
			Context.SubmitChanges();
		}
	}
}