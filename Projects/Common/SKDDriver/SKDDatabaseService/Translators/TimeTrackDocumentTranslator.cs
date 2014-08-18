using System;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using OperationResult = FiresecAPI.OperationResult;
using FiresecAPI.SKD;
using FiresecAPI;
using System.Collections.Generic;

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

		public OperationResult<List<TimeTrackDocument>> Get(Guid employeeUID, DateTime startDateTime, DateTime endDateTime)
		{
			try
			{
				var tableTimeTrackDocuments = Context.TimeTrackDocuments.Where(x => x.EmployeeUID == employeeUID && ((x.StartDateTime.Date >= startDateTime && x.StartDateTime.Date <= endDateTime) || (x.EndDateTime.Date >= startDateTime && x.EndDateTime.Date <= endDateTime)));
				if (tableTimeTrackDocuments != null)
				{
					var timeTrackDocuments = new List<TimeTrackDocument>();
					foreach (var tableTimeTrackDocument in tableTimeTrackDocuments)
					{
						var timeTrackDocument = new TimeTrackDocument()
						{
							UID = tableTimeTrackDocument.UID,
							EmployeeUID = tableTimeTrackDocument.EmployeeUID,
							StartDateTime = tableTimeTrackDocument.StartDateTime,
							EndDateTime = tableTimeTrackDocument.EndDateTime,
							DocumentCode = tableTimeTrackDocument.DocumentCode,
							Comment = tableTimeTrackDocument.Comment
						};
						timeTrackDocuments.Add(timeTrackDocument);
					}
					return new OperationResult<List<TimeTrackDocument>>() { Result = timeTrackDocuments };
				}
				return new OperationResult<List<TimeTrackDocument>>() { Result = new List<TimeTrackDocument>() };
			}
			catch (Exception e)
			{
				return new OperationResult<List<TimeTrackDocument>>(e.Message);
			}
		}

		public OperationResult AddTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			return new OperationResult();
		}
		public OperationResult EditTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			return new OperationResult();
		}
		public OperationResult RemoveTimeTrackDocument(Guid timeTrackDocumentUID)
		{
			return new OperationResult();
		}
	}
}