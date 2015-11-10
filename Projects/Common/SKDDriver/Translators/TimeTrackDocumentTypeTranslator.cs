using System.Diagnostics;
using System.Threading.Tasks;
using FiresecAPI;
using FiresecAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;
using SKDDriver.DataAccess;
using OperationResult = FiresecAPI.OperationResult;
using TimeTrackDocumentType = FiresecAPI.SKD.TimeTrackDocumentType;

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
				var timeTrackDocumentTypes = tableItems
					.Where(x => x.OrganisationUID == organisationUID)
					.Select(tableTimeTrackDocumentType => new TimeTrackDocumentType
					{
						UID = tableTimeTrackDocumentType.UID,
						OrganisationUID = tableTimeTrackDocumentType.OrganisationUID,
						Name = tableTimeTrackDocumentType.Name,
						ShortName = tableTimeTrackDocumentType.ShortName,
						Code = tableTimeTrackDocumentType.DocumentCode,
						DocumentType = (DocumentType) tableTimeTrackDocumentType.DocumentType,
					})
					.ToList();

				return new OperationResult<List<TimeTrackDocumentType>>(timeTrackDocumentTypes);
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
				var tableItem = new DataAccess.TimeTrackDocumentType
				{
					UID = timeTrackDocumentType.UID,
					OrganisationUID = timeTrackDocumentType.OrganisationUID,
					Name = timeTrackDocumentType.Name,
					ShortName = timeTrackDocumentType.ShortName,
					DocumentCode = timeTrackDocumentType.Code,
					DocumentType = (int) timeTrackDocumentType.DocumentType
				};
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
				var tableItem = Context.TimeTrackDocumentTypes.Single(x => x.UID.Equals(uid));
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