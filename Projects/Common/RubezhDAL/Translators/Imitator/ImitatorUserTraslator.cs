using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace RubezhDAL.DataClasses
{
	public class ImitatorUserTraslator
	{
		public DbService DbService { get; private set; }
		public DatabaseContext Context { get { return DbService.Context; } }

		public ImitatorUserTraslator(DbService dbService)
		{
			DbService = dbService;
		}

		public List<ImitatorUser> Get()
		{
			try
			{
				var result = Context.ImitatorUsers.Include(x => x.ImitatorUserDevices).ToList();
				return result;

			}
			catch
			{
				return null;
			}
		}

		public void Add(ImitatorUser imitatorUser)
		{
			try
			{
				imitatorUser.EndDateTime = imitatorUser.EndDateTime.CheckDate();
				Context.ImitatorUsers.Add(imitatorUser);
				Context.SaveChanges();
			}
			catch
			{

			}
		}

		public void Edit(ImitatorUser imitatorUser)
		{
			try
			{
				var existingUser = Context.ImitatorUsers.Include(x => x.ImitatorUserDevices).FirstOrDefault(x => x.GKNo == imitatorUser.GKNo);
				if (existingUser != null)
				{

					Context.ImitatorUserDevices.RemoveRange(existingUser.ImitatorUserDevices);
					existingUser.GKNo = imitatorUser.GKNo;
					existingUser.Number = imitatorUser.Number;
					existingUser.Name = imitatorUser.Name;
					existingUser.EndDateTime = imitatorUser.EndDateTime.CheckDate();
					existingUser.TotalSeconds = imitatorUser.TotalSeconds;
					existingUser.CardType = imitatorUser.CardType;
					existingUser.Level = imitatorUser.Level;
					existingUser.ScheduleNo = imitatorUser.ScheduleNo;
					existingUser.IsBlocked = imitatorUser.IsBlocked;
					existingUser.ImitatorUserDevices = imitatorUser.ImitatorUserDevices;
					Context.SaveChanges();
				}
			}
			catch
			{

			}
		}

		public ImitatorUser GetByGKNo(int gkNo)
		{
			try
			{
				return Context.ImitatorUsers.FirstOrDefault(x => x.GKNo == gkNo);
			}
			catch
			{
				return null;
			}
		}

		public ImitatorUser GetByNo(uint no)
		{
			try
			{
				return Context.ImitatorUsers.Include(x => x.ImitatorUserDevices).FirstOrDefault(x => x.Number == no);
			}
			catch
			{
				return null;
			}
		}
	}
}