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
				var timeTrackDocument = Context.TimeTrackDocuments.FirstOrDefault(x => x.EmployeeUID == api.EmployeeUID && x.StartDateTime.Date == api.StartDateTime.Date);
				if (timeTrackDocument != null)
				{
					timeTrackDocument.DocumentCode = api.DocumentCode;
					timeTrackDocument.Comment = api.Comment;
				}
				else
				{
					timeTrackDocument = new DataAccess.TimeTrackDocument()
					{
						UID = Guid.NewGuid(),
						StartDateTime = api.StartDateTime.Date,
						EndDateTime = api.EndDateTime.Date,
						EmployeeUID = api.EmployeeUID,
						DocumentCode = api.DocumentCode,
						Comment = api.Comment
					};
					Context.TimeTrackDocuments.InsertOnSubmit(timeTrackDocument);
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
				var timeTrackDocument = Context.TimeTrackDocuments.FirstOrDefault(x => x.EmployeeUID == employeeUID && x.StartDateTime.Date == dateTime.Date);
				if (timeTrackDocument != null)
				{
					var api = new TimeTrackDocument()
					{
						UID = timeTrackDocument.UID,
						EmployeeUID = timeTrackDocument.EmployeeUID,
						StartDateTime = timeTrackDocument.StartDateTime,
						EndDateTime = timeTrackDocument.EndDateTime,
						DocumentCode = timeTrackDocument.DocumentCode,
						Comment = timeTrackDocument.Comment
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