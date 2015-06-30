using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;


namespace SKDDriver
{
	public class CurrentConsumptionTranslator
	{
		SKDDriver.DataAccess.SKDDataContext _Context;

		public CurrentConsumptionTranslator(SKDDatabaseService databaseService)
		{
			_Context = databaseService.Context;
		}

		public OperationResult Save(CurrentConsumption item)
		{
			try
			{
				var tableItem = new DataAccess.CurrentConsumption 
				{ 
					UID = item.UID,
					AlsUID = item.AlsUID, 
					Current = item.Current, 
					DateTime = item.DateTime
				};
				_Context.CurrentConsumptions.InsertOnSubmit(tableItem);
				_Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult SaveMany(IEnumerable<CurrentConsumption> items)
		{
			try
			{
				var tableItems = new List<DataAccess.CurrentConsumption>();
				foreach (var item in items)
				{
					tableItems.Add(new DataAccess.CurrentConsumption
					{
						UID = item.UID,
						AlsUID = item.AlsUID,
						Current = item.Current,
						DateTime = item.DateTime
					});
				}
				_Context.CurrentConsumptions.InsertAllOnSubmit(tableItems);	
				_Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult<IEnumerable<CurrentConsumption>> Get(CurrentConsumptionFilter filter)
		{
			try
			{
				var result = from tableItem in _Context.CurrentConsumptions
							 where tableItem.AlsUID == filter.AlsUID && tableItem.DateTime >= filter.StartDateTime && tableItem.DateTime <= filter.EndDateTime
							 select new CurrentConsumption
							 {
								 UID = tableItem.UID,
								 AlsUID = tableItem.AlsUID,
								 Current = tableItem.Current,
								 DateTime = tableItem.DateTime
							 };
				return new OperationResult<IEnumerable<CurrentConsumption>>(result.ToList());
			}
			catch (Exception e)
			{
				return OperationResult<IEnumerable<CurrentConsumption>>.FromError(e.Message);
			}
		}
	}

	
}
