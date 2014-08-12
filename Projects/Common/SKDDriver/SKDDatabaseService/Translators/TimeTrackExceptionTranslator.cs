using System;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI.EmployeeTimeIntervals;
using LinqKit;
using OperationResult = FiresecAPI.OperationResult;
using FiresecAPI.SKD;
using FiresecAPI;

namespace SKDDriver.Translators
{
	public class TimeTrackExceptionTranslator
	{
		DataAccess.SKDDataContext Context;

		public TimeTrackExceptionTranslator(DataAccess.SKDDataContext context)
		{
			Context = context;
		}

		public OperationResult Save(TimeTrackException api)
		{
			try
			{
				var timeTrackException = Context.TimeTrackExceptions.FirstOrDefault(x => x.EmployeeUID == api.EmployeeUID && x.StartDateTime.Date == api.StartDateTime.Date);
				if (timeTrackException != null)
				{
					timeTrackException.DocumentType = (int)api.TimeTrackExceptionType;
					timeTrackException.Comment = api.Comment;
				}
				else
				{
					timeTrackException = new DataAccess.TimeTrackException()
					{
						UID = Guid.NewGuid(),
						StartDateTime = api.StartDateTime.Date,
						EndDateTime = api.EndDateTime.Date,
						EmployeeUID = api.EmployeeUID,
						DocumentType = (int)api.TimeTrackExceptionType,
						Comment = api.Comment
					};
					Context.TimeTrackExceptions.InsertOnSubmit(timeTrackException);
				}
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult<TimeTrackException> Get(DateTime dateTime, Guid employeeUID)
		{
			try
			{
				var timeTrackException = Context.TimeTrackExceptions.FirstOrDefault(x => x.EmployeeUID == employeeUID && x.StartDateTime.Date == dateTime.Date);
				if (timeTrackException != null)
				{
					var api = new TimeTrackException()
					{
						UID = timeTrackException.UID,
						EmployeeUID = timeTrackException.EmployeeUID,
						StartDateTime = timeTrackException.StartDateTime,
						EndDateTime = timeTrackException.EndDateTime,
						TimeTrackExceptionType = (TimeTrackExceptionType)timeTrackException.DocumentType,
						Comment = timeTrackException.Comment
					};
					return new OperationResult<TimeTrackException>() { Result = api };
				}
				return new OperationResult<TimeTrackException>() { Result = new TimeTrackException() };
			}
			catch (Exception e)
			{
				return new OperationResult<TimeTrackException>(e.Message);
			}
		}
	}
}