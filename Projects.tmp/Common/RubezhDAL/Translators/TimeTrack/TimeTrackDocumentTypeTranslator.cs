using RubezhAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using API = RubezhAPI.SKD;

namespace RubezhDAL.DataClasses
{
	public class TimeTrackDocumentTypeTranslator
	{
		DatabaseContext Context;

		public TimeTrackDocumentTypeTranslator(DbService dbService)
		{
			Context = dbService.Context;
		}

		public OperationResult<List<API.TimeTrackDocumentType>> Get(Guid organisationUID)
		{
			return Get(organisationUID, Context.TimeTrackDocumentTypes);
		}

		public OperationResult<List<API.TimeTrackDocumentType>> Get(Guid organisationUID, IEnumerable<TimeTrackDocumentType> tableItems)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableTimeTrackDocumentTypes = tableItems.Where(x => x.OrganisationUID == organisationUID);
				if (tableTimeTrackDocumentTypes != null)
				{
					var timeTrackDocumentTypes = new List<API.TimeTrackDocumentType>();
					foreach (var tableItem in tableTimeTrackDocumentTypes)
					{
						timeTrackDocumentTypes.Add(Translate(tableItem));
					}
					return timeTrackDocumentTypes;
				}
				return new List<API.TimeTrackDocumentType>();
			});
		}

		public API.TimeTrackDocumentType Translate(TimeTrackDocumentType tableItem)
		{
			return new API.TimeTrackDocumentType()
			{
				UID = tableItem.UID,
				OrganisationUID = tableItem.OrganisationUID.Value,
				Name = tableItem.Name,
				ShortName = tableItem.ShortName,
				Code = tableItem.DocumentCode,
				DocumentType = (API.DocumentType)tableItem.DocumentType,
			};
		}

		public OperationResult<bool> AddTimeTrackDocumentType(API.TimeTrackDocumentType timeTrackDocumentType)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItem = new TimeTrackDocumentType();
				tableItem.UID = timeTrackDocumentType.UID;
				tableItem.OrganisationUID = timeTrackDocumentType.OrganisationUID;
				tableItem.Name = timeTrackDocumentType.Name;
				tableItem.ShortName = timeTrackDocumentType.ShortName;
				tableItem.DocumentCode = timeTrackDocumentType.Code;
				tableItem.DocumentType = (int)timeTrackDocumentType.DocumentType;
				Context.TimeTrackDocumentTypes.Add(tableItem);
				Context.SaveChanges();
				return true;
			});
		}
		public OperationResult<bool> EditTimeTrackDocumentType(API.TimeTrackDocumentType timeTrackDocumentType)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItem = (from x in Context.TimeTrackDocumentTypes where x.UID.Equals(timeTrackDocumentType.UID) select x).FirstOrDefault();
				if (tableItem != null)
				{
					tableItem.Name = timeTrackDocumentType.Name;
					tableItem.ShortName = timeTrackDocumentType.ShortName;
					tableItem.DocumentCode = timeTrackDocumentType.Code;
					tableItem.DocumentType = (int)timeTrackDocumentType.DocumentType;
					Context.SaveChanges();
				}
				return true;
			});
		}
		public OperationResult<bool> RemoveTimeTrackDocumentType(Guid uid)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItem = Context.TimeTrackDocumentTypes.Where(x => x.UID.Equals(uid)).Single();
				Context.TimeTrackDocumentTypes.Remove(tableItem);
				Context.SaveChanges();
				return true;
			});
		}
	}
}