using System.Windows.Documents;
using FiresecAPI;
using FiresecAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;
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
			return Get(employeeUID, startDateTime, endDateTime, Context.TimeTrackDocuments, Context.TimeTrackDocumentTypes);
		}

		[Obsolete]
		public OperationResult<List<TimeTrackDocument>> Get(Guid employeeUID, DateTime startDateTime, DateTime endDateTime, IEnumerable<DataAccess.TimeTrackDocument> tableItems, IEnumerable<DataAccess.TimeTrackDocumentType> tableItemsTypes)
		{
			try
			{
				//TODO: Add func to get types of documents
				var tableTimeTrackDocuments = tableItems
					.Where(x => x.EmployeeUID == employeeUID &&
					((x.StartDateTime.Date >= startDateTime && x.StartDateTime.Date <= endDateTime) ||
					 (x.EndDateTime.Date >= startDateTime && x.EndDateTime.Date <= endDateTime) ||
					 (startDateTime >= x.StartDateTime.Date && startDateTime <= x.EndDateTime.Date) ||
					 (endDateTime >= x.StartDateTime.Date && endDateTime <= x.EndDateTime.Date)));

				var timeTrackDocuments = tableTimeTrackDocuments
					.Select(tableTimeTrackDocument => new TimeTrackDocument
					{
						UID = tableTimeTrackDocument.UID,
						EmployeeUID = tableTimeTrackDocument.EmployeeUID,
						StartDateTime = tableTimeTrackDocument.StartDateTime,
						EndDateTime = tableTimeTrackDocument.EndDateTime,
						DocumentCode = tableTimeTrackDocument.DocumentCode,
						Comment = tableTimeTrackDocument.Comment,
						DocumentDateTime = tableTimeTrackDocument.DocumentDateTime,
						DocumentNumber = tableTimeTrackDocument.DocumentNumber,
						FileName = tableTimeTrackDocument.FileName,
						IsOutside = tableTimeTrackDocument.IsOutside
					})
					.ToList();

				var result = timeTrackDocuments.ToList();
				return new OperationResult<List<TimeTrackDocument>>(result);
			}
			catch (Exception e)
			{
				return OperationResult<List<TimeTrackDocument>>.FromError(e.Message);
			}
		}

		public OperationResult<List<TimeTrackDocument>> GetWithTypes(ShortEmployee employee, DateTime startDateTime, DateTime endDateTime, IEnumerable<DataAccess.TimeTrackDocument> tableItems, IEnumerable<DataAccess.TimeTrackDocumentType> tableItemsTypes)
		{
			try
			{
				//TODO: Add func to get types of documents
				var tableTimeTrackDocuments = tableItems
					.Where(x => x.EmployeeUID == employee.UID &&
					((x.StartDateTime.Date >= startDateTime && x.StartDateTime.Date <= endDateTime) ||
					 (x.EndDateTime.Date >= startDateTime && x.EndDateTime.Date <= endDateTime) ||
					 (startDateTime >= x.StartDateTime.Date && startDateTime <= x.EndDateTime.Date) ||
					 (endDateTime >= x.StartDateTime.Date && endDateTime <= x.EndDateTime.Date)));

				var docfactory = new DocumentsFactory();
				var systemTypes = docfactory.SystemDocuments.Select(x => x.TimeTrackDocumentType);
				var timeTrackDocumentTypes = tableItemsTypes
					.Where(x => x.OrganisationUID == employee.OrganisationUID)
					.Select(tableTimeTrackDocumentType => new TimeTrackDocumentType
					{
						UID = tableTimeTrackDocumentType.UID,
						OrganisationUID = tableTimeTrackDocumentType.OrganisationUID,
						Name = tableTimeTrackDocumentType.Name,
						ShortName = tableTimeTrackDocumentType.ShortName,
						Code = tableTimeTrackDocumentType.DocumentCode,
						DocumentType = (DocumentType)tableTimeTrackDocumentType.DocumentType,
					})
					.ToList();

				systemTypes = systemTypes.Union(timeTrackDocumentTypes);

				var docsList = tableTimeTrackDocuments.Select(tableDoc => new TimeTrackDocument
				{
					UID = tableDoc.UID,
					EmployeeUID = tableDoc.EmployeeUID,
					StartDateTime = tableDoc.StartDateTime,
					EndDateTime = tableDoc.EndDateTime,
					DocumentCode = tableDoc.DocumentCode,
					Comment = tableDoc.Comment,
					DocumentDateTime = tableDoc.DocumentDateTime,
					DocumentNumber = tableDoc.DocumentNumber,
					FileName = tableDoc.FileName,
					IsOutside = tableDoc.IsOutside,
					TimeTrackDocumentType = systemTypes.FirstOrDefault(x => x.Code == tableDoc.DocumentCode)
				})
				.ToList();

				return new OperationResult<List<TimeTrackDocument>>(docsList);
			}
			catch (Exception e)
			{
				return OperationResult<List<TimeTrackDocument>>.FromError(e.Message);
			}
		}

		public OperationResult AddTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			try
			{
				var tableItem = new DataAccess.TimeTrackDocument
				{
					UID = timeTrackDocument.UID,
					EmployeeUID = timeTrackDocument.EmployeeUID,
					StartDateTime = timeTrackDocument.StartDateTime,
					EndDateTime = timeTrackDocument.EndDateTime,
					DocumentCode = timeTrackDocument.DocumentCode,
					Comment = timeTrackDocument.Comment,
					DocumentDateTime = timeTrackDocument.DocumentDateTime,
					DocumentNumber = timeTrackDocument.DocumentNumber,
					IsOutside = timeTrackDocument.IsOutside
				};
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
					tableItem.IsOutside = timeTrackDocument.IsOutside;
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