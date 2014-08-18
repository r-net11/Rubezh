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
			try
			{
				var tableItem = new DataAccess.TimeTrackDocument();
				tableItem.UID = timeTrackDocument.UID;
				tableItem.EmployeeUID = timeTrackDocument.EmployeeUID;
				tableItem.StartDateTime = timeTrackDocument.StartDateTime;
				tableItem.EndDateTime = timeTrackDocument.EndDateTime;
				tableItem.DocumentCode = timeTrackDocument.DocumentCode;
				tableItem.Comment = timeTrackDocument.Comment;
				Context.TimeTrackDocuments.InsertOnSubmit(tableItem);
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}
		public OperationResult EditTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			try
			{
				var tableItem = (from x in Context.TimeTrackDocuments where x.UID.Equals(timeTrackDocument.UID) select x).FirstOrDefault();
				if (tableItem != null)
				{
					tableItem.EmployeeUID = timeTrackDocument.EmployeeUID;
					tableItem.StartDateTime = timeTrackDocument.StartDateTime;
					tableItem.EndDateTime = timeTrackDocument.EndDateTime;
					tableItem.DocumentCode = timeTrackDocument.DocumentCode;
					tableItem.Comment = timeTrackDocument.Comment;
					Context.SubmitChanges();
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}
		public OperationResult RemoveTimeTrackDocument(Guid uid)
		{
			try
			{
				var tableItem = Context.TimeTrackDocuments.Where(x => x.UID.Equals(uid)).Single();
				Context.TimeTrackDocuments.DeleteOnSubmit(tableItem);
				Context.TimeTrackDocuments.Context.SubmitChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}
	}
}