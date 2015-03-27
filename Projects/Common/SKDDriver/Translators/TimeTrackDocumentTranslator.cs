using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	public class TimeTrackDocumentTranslator
	{
		protected SKDDatabaseService DatabaseService;
		protected DataAccess.SKDDataContext Context;

		public TimeTrackDocumentTranslator(SKDDatabaseService databaseService)
		{
			DatabaseService = databaseService;
			Context = databaseService.Context;
		}

		public OperationResult<List<TimeTrackDocument>> Get(Guid employeeUID, DateTime startDateTime, DateTime endDateTime)
		{
			try
			{
				var tableTimeTrackDocuments = Context.TimeTrackDocuments.Where(x => x.EmployeeUID == employeeUID && 
					((x.StartDateTime.Date >= startDateTime && x.StartDateTime.Date <= endDateTime) || 
					 (x.EndDateTime.Date >= startDateTime && x.EndDateTime.Date <= endDateTime) || 
					 (startDateTime >= x.StartDateTime.Date && startDateTime <= x.EndDateTime.Date) ||
					 (endDateTime >= x.StartDateTime.Date && endDateTime <= x.EndDateTime.Date)));
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
							Comment = tableTimeTrackDocument.Comment,
							DocumentDateTime = tableTimeTrackDocument.DocumentDateTime,
							DocumentNumber = tableTimeTrackDocument.DocumentNumber,
							FileName = tableTimeTrackDocument.FileName
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
				tableItem.DocumentDateTime = timeTrackDocument.DocumentDateTime;
				tableItem.DocumentNumber = timeTrackDocument.DocumentNumber;
				Context.TimeTrackDocuments.InsertOnSubmit(tableItem);
				tableItem.FileName = timeTrackDocument.FileName;
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
					tableItem.DocumentDateTime = timeTrackDocument.DocumentDateTime;
					tableItem.DocumentNumber = timeTrackDocument.DocumentNumber;
					tableItem.FileName = timeTrackDocument.FileName;
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