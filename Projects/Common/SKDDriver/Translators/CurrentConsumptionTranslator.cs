using FiresecAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using API = FiresecAPI.GK;

namespace SKDDriver.DataClasses
{   
    public class CurrentConsumptionTranslator
    {
        public DbService DbService { get; private set; }
		public DatabaseContext Context { get { return DbService.Context; } }

        public CurrentConsumptionTranslator(DbService context)
		{
			DbService = context;
		}

        public OperationResult Save(API.CurrentConsumption item)
        {
            try
            {
                SaveItem(item, Context.CurrentConsumptions);
                Context.SaveChanges();
                return new OperationResult();
            }
            catch (Exception e)
            {
                return new OperationResult(e.Message);
            }
        }

        public OperationResult SaveMany(IEnumerable<API.CurrentConsumption> items)
        {
            try
            {
                var uids = items.Select(x => x.UID).ToList();
                var tableItems = Context.CurrentConsumptions.Where(x => uids.Contains(x.UID)).ToList();//. new List<DataAccess.CurrentConsumption>();
                foreach (var item in items)
                {
                    SaveItem(item, tableItems);
                }
                Context.SaveChanges();
                return new OperationResult();
            }
            catch (Exception e)
            {
                return new OperationResult(e.Message);
            }
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
            try
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
                return new OperationResult<List<API.CurrentConsumption>>(result.ToList());
            }
            catch (Exception e)
            {
                return OperationResult<List<API.CurrentConsumption>>.FromError(e.Message);
            }
        }
    }
}
