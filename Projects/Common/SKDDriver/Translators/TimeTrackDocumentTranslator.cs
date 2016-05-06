using Common;
using StrazhAPI;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;
using OperationResult = StrazhAPI.OperationResult;

namespace StrazhDAL
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

		public OperationResult<List<TimeTrackDocument>> GetWithTypes(ShortEmployee employee, DateTime startDateTime, DateTime endDateTime, IEnumerable<DataAccess.TimeTrackDocument> tableItems, IEnumerable<TimeTrackDocumentType> documentTypes)
		{
			try
			{
				var tableTimeTrackDocuments = tableItems
					.Where(x => x.EmployeeUID == employee.UID &&
					            ((x.StartDateTime.Date >= startDateTime && x.StartDateTime.Date <= endDateTime) ||
					             (x.EndDateTime.Date >= startDateTime && x.EndDateTime.Date <= endDateTime) ||
					             (startDateTime >= x.StartDateTime.Date && startDateTime <= x.EndDateTime.Date) ||
					             (endDateTime >= x.StartDateTime.Date && endDateTime <= x.EndDateTime.Date)));

				var docsList = tableTimeTrackDocuments.Select(tableDoc =>
				{
					var result = new TimeTrackDocument
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
						TimeTrackDocumentType = documentTypes.FirstOrDefault(x => x.Code == tableDoc.DocumentCode)
					};

					var attachment = Context.Attachments.FirstOrDefault(x => x.UID.ToString() == result.FileName);
					if (attachment != null)
						result.OriginalFileName = attachment.Name;
		
					return result;
				}).ToList();

				return new OperationResult<List<TimeTrackDocument>>(docsList);
			}
			catch (Exception e)
			{
				return OperationResult<List<TimeTrackDocument>>.FromError(e.Message);
			}
		}

		public OperationResult CheckDocumentType(TimeTrackDocumentType timeTrackDocumentType, Guid organisationUID)
		{
			var emplOfCurrentOrganisation = Context.Employees.Where(x => x.OrganisationUID == organisationUID);
			var joinDocumentsEmployeeResult = emplOfCurrentOrganisation
												.Join(
												Context.TimeTrackDocuments,
												empl => empl.UID,
												doc => doc.EmployeeUID,
												(empl, doc) => new {empl.UID, doc.DocumentCode}
												);
			var result = joinDocumentsEmployeeResult.Where(x => x.DocumentCode == timeTrackDocumentType.Code);
			return result.IsEmpty() ? new OperationResult() : new OperationResult("Error");
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
				var tableItem = Context.TimeTrackDocuments.Single(x => x.UID.Equals(uid));
				Context.TimeTrackDocuments.DeleteOnSubmit(tableItem);
				Context.TimeTrackDocuments.Context.SubmitChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}

		private TimeTrackDocument Translate(DataAccess.TimeTrackDocument tableItem)
		{
			var result = new TimeTrackDocument
			{
				UID = tableItem.UID,
				EmployeeUID = tableItem.EmployeeUID,
				StartDateTime = tableItem.StartDateTime,
				EndDateTime = tableItem.EndDateTime,
				DocumentCode = tableItem.DocumentCode,
				Comment = tableItem.Comment,
				FileName = tableItem.FileName,
				DocumentDateTime = tableItem.DocumentDateTime,
				IsOutside = tableItem.IsOutside,
				DocumentNumber = tableItem.DocumentNumber,
				TimeTrackDocumentType = DatabaseService.TimeTrackDocumentTypeTranslator.Get(tableItem.DocumentCode)
			};

			var attachment = Context.Attachments.FirstOrDefault(x => x.UID.ToString() == result.FileName);
			if (attachment != null)
				result.OriginalFileName = attachment.Name;

			return result;
		}

		public TimeTrackDocument Get(Guid timeTrackDocumentUID)
		{
			var tableItem = Context.TimeTrackDocuments.FirstOrDefault(x => x.UID == timeTrackDocumentUID);
			return tableItem == null ? null : Translate(tableItem);
		}
	}
}