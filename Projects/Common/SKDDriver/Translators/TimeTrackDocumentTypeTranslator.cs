using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	public class TimeTrackDocumentTypeTranslator
	{
		protected SKDDatabaseService DatabaseService;
		protected DataAccess.SKDDataContext Context;

		public TimeTrackDocumentTypeTranslator(SKDDatabaseService databaseService)
		{
			DatabaseService = databaseService;
			Context = databaseService.Context;
		}

		public OperationResult<List<TimeTrackDocumentType>> Get(Guid organisationUID)
		{
			return Get(organisationUID, Context.TimeTrackDocumentTypes);
		}

		public OperationResult<List<TimeTrackDocumentType>> Get(Guid organisationUID, IEnumerable<DataAccess.TimeTrackDocumentType> tableItems)
		{
			try
			{
				var tableTimeTrackDocumentTypes = tableItems.Where(x => x.OrganisationUID == organisationUID);
				if (tableTimeTrackDocumentTypes != null)
				{
					var timeTrackDocumentTypes = new List<TimeTrackDocumentType>();
					foreach (var tableTimeTrackDocumentType in tableTimeTrackDocumentTypes)
					{
						var timeTrackDocumentType = new TimeTrackDocumentType()
						{
							UID = tableTimeTrackDocumentType.UID,
							OrganisationUID = tableTimeTrackDocumentType.OrganisationUID,
							Name = tableTimeTrackDocumentType.Name,
							ShortName = tableTimeTrackDocumentType.ShortName,
							Code = tableTimeTrackDocumentType.DocumentCode,
							DocumentType = (DocumentType)tableTimeTrackDocumentType.DocumentType,
						};
						timeTrackDocumentTypes.Add(timeTrackDocumentType);
					}
					return new OperationResult<List<TimeTrackDocumentType>>(timeTrackDocumentTypes);
				}
				return new OperationResult<List<TimeTrackDocumentType>>(new List<TimeTrackDocumentType>());
			}
			catch (Exception e)
			{
				return OperationResult<List<TimeTrackDocumentType>>.FromError(e.Message);
			}
		}

		public OperationResult AddTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			try
			{
				var tableItem = new DataAccess.TimeTrackDocumentType();
				tableItem.UID = timeTrackDocumentType.UID;
				tableItem.OrganisationUID = timeTrackDocumentType.OrganisationUID;
				tableItem.Name = timeTrackDocumentType.Name;
				tableItem.ShortName = timeTrackDocumentType.ShortName;
				tableItem.DocumentCode = timeTrackDocumentType.Code;
				tableItem.DocumentType = (int)timeTrackDocumentType.DocumentType;
				Context.TimeTrackDocumentTypes.InsertOnSubmit(tableItem);
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}
		public OperationResult EditTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			try
			{
				var tableItem = (from x in Context.TimeTrackDocumentTypes where x.UID.Equals(timeTrackDocumentType.UID) select x).FirstOrDefault();
				if (tableItem != null)
				{
					tableItem.Name = timeTrackDocumentType.Name;
					tableItem.ShortName = timeTrackDocumentType.ShortName;
					tableItem.DocumentCode = timeTrackDocumentType.Code;
					tableItem.DocumentType = (int)timeTrackDocumentType.DocumentType;
					Context.SubmitChanges();
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}
		public OperationResult RemoveTimeTrackDocumentType(Guid uid)
		{
			try
			{
				var tableItem = Context.TimeTrackDocumentTypes.Where(x => x.UID.Equals(uid)).Single();
				Context.TimeTrackDocumentTypes.DeleteOnSubmit(tableItem);
				Context.TimeTrackDocumentTypes.Context.SubmitChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}
	}
}