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
	public class TimeTrackDocumentTypeTranslator
	{
		DataAccess.SKDDataContext Context;

		public TimeTrackDocumentTypeTranslator(DataAccess.SKDDataContext context)
		{
			Context = context;
		}

		public OperationResult<List<TimeTrackDocumentType>> Get(Guid organisationUID)
		{
			try
			{
				var tableTimeTrackDocumentTypes = Context.TimeTrackDocumentTypes.Where(x => x.OrganisationUID == organisationUID);
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
					return new OperationResult<List<TimeTrackDocumentType>>() { Result = timeTrackDocumentTypes };
				}
				return new OperationResult<List<TimeTrackDocumentType>>() { Result = new List<TimeTrackDocumentType>() };
			}
			catch (Exception e)
			{
				return new OperationResult<List<TimeTrackDocumentType>>(e.Message);
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