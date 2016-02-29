using RubezhAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using API = RubezhAPI.GK;

namespace RubezhDAL.DataClasses
{   
	public class CurrentConsumptionTranslator
	{
		public DbService DbService { get; private set; }
		public DatabaseContext Context { get { return DbService.Context; } }

		public CurrentConsumptionTranslator(DbService dbService)
		{
			DbService = dbService;
		}

		public OperationResult<bool> Save(API.CurrentConsumption item)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				SaveItem(item, Context.CurrentConsumptions);
				Context.SaveChanges();
				return true;
			});
		}

		public OperationResult<bool> SaveMany(IEnumerable<API.CurrentConsumption> items)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var uids = items.Select(x => x.UID).ToList();
				var tableItems = Context.CurrentConsumptions.Where(x => uids.Contains(x.UID)).ToList();//. new List<DataAccess.CurrentConsumption>();
				foreach (var item in items)
				{
					SaveItem(item, tableItems);
				}
				Context.SaveChanges();
				return true;
			});
		}

		void SaveItem(API.CurrentConsumption item, IEnumerable<CurrentConsumption> tableItems)
		{
			bool isNew = false;
			var tableItem = tableItems.FirstOrDefault(x => x.UID == item.UID);
			if (tableItem == null)
			{
				tableItem = new CurrentConsumption { UID = item.UID };
				isNew = true;
			}
			tableItem.AlsUID = item.AlsUID;
			tableItem.Current = item.Current;
			tableItem.DateTime = item.DateTime.CheckDate();
			if (isNew)
				Context.CurrentConsumptions.Add(tableItem);
		}

		public OperationResult<List<API.CurrentConsumption>> Get(API.CurrentConsumptionFilter filter)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var result = from tableItem in Context.CurrentConsumptions
							 where tableItem.AlsUID == filter.AlsUID && tableItem.DateTime >= filter.StartDateTime && tableItem.DateTime <= filter.EndDateTime
							 select new API.CurrentConsumption
							 {
								 UID = tableItem.UID,
								 AlsUID = tableItem.AlsUID,
								 Current = tableItem.Current,
								 DateTime = tableItem.DateTime
							 };
				return result.ToList();
			});
		}
	}
}
