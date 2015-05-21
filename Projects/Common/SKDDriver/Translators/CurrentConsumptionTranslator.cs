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
					KauUID = item.KauUID, 
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

		public OperationResult<IEnumerable<CurrentConsumption>> Get(CurrentConsumptionFilter filter)
		{
			try
			{
				var result = from tableItem in _Context.CurrentConsumptions
							 where tableItem.KauUID == filter.KauUID && tableItem.DateTime >= filter.StartDateTime && tableItem.DateTime <= filter.EndDateTime
							 select new CurrentConsumption
							 {
								 UID = tableItem.UID,
								 KauUID = tableItem.KauUID,
								 Current = tableItem.Current,
								 DateTime = tableItem.DateTime
							 };
				return new OperationResult<IEnumerable<CurrentConsumption>>(result);
			}
			catch (Exception e)
			{
				return OperationResult<IEnumerable<CurrentConsumption>>.FromError(e.Message);
			}
		}
	}

	
}
