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
		public static Deposit GetDeposit(Guid uid)
		{
			using (var context = DatabaseContext.Initialize())
			{
				return context.Deposits.FirstOrDefault(x => x.UID == uid);
			}
		}

		public static IEnumerable<Deposit> GetDeposits(Guid consumerUID)
		{
			using (var context = DatabaseContext.Initialize())
			{
				return context.Deposits.Where(x => x.ConsumerUID == consumerUID).ToList();
			}
		}

		public static IEnumerable<Deposit> GetAllDeposits()
		{
			using (var context = DatabaseContext.Initialize())
			{
				return context.Deposits.ToList();
			}
		}
				
		public static void SaveDeposit(Deposit deposit)
		{
			using (var context = DatabaseContext.Initialize())
			{
				var dbDeposit = context.Deposits.FirstOrDefault(x => x.UID == deposit.UID);

				if (dbDeposit == null)
				{
					dbDeposit = context.Deposits.Add(deposit);
				}
				else
				{
					dbDeposit.ConsumerUID = deposit.ConsumerUID;
					dbDeposit.Moment = deposit.Moment;
					dbDeposit.Amount = deposit.Amount;
					dbDeposit.Description = deposit.Description;
				}
				context.SaveChanges();
			}
		}
		
		public static void DeleteDeposit(Deposit deposit)
		{
			using (var context = DatabaseContext.Initialize())
			{
				var dbDeposit = context.Deposits.FirstOrDefault(x => x.UID == deposit.UID);
				if (dbDeposit != null)
					context.Deposits.Remove(dbDeposit);
				context.SaveChanges();
			}
		}
	}
}
