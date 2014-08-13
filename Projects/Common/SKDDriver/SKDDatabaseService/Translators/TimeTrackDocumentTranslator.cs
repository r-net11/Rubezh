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
	public class TimeTrackDocumentTranslator
	{
		DataAccess.SKDDataContext Context;

		public TimeTrackDocumentTranslator(DataAccess.SKDDataContext context)
		{
			Context = context;
		}

		public OperationResult Save(TimeTrackDocument api)
		{
			try
			{
				var timeTrackException = Context.TimeTrackExceptions.FirstOrDefault(x => x.EmployeeUID == api.EmployeeUID && x.StartDateTime.Date == api.StartDateTime.Date);
				if (timeTrackException != null)
				{
					timeTrackException.DocumentType = api.DocumentCode;
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
						DocumentType = api.DocumentCode,
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

		public OperationResult<TimeTrackDocument> Get(DateTime dateTime, Guid employeeUID)
		{
			try
			{
				var timeTrackException = Context.TimeTrackExceptions.FirstOrDefault(x => x.EmployeeUID == employeeUID && x.StartDateTime.Date == dateTime.Date);
				if (timeTrackException != null)
				{
					var api = new TimeTrackDocument()
					{
						UID = timeTrackException.UID,
						EmployeeUID = timeTrackException.EmployeeUID,
						StartDateTime = timeTrackException.StartDateTime,
						EndDateTime = timeTrackException.EndDateTime,
						DocumentCode = timeTrackException.DocumentType,
						Comment = timeTrackException.Comment
					};
					return new OperationResult<TimeTrackDocument>() { Result = api };
				}
				return new OperationResult<TimeTrackDocument>() { Result = new TimeTrackDocument() };
			}
			catch (Exception e)
			{
				return new OperationResult<TimeTrackDocument>(e.Message);
			}
		}
	}
}