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
			try
			{
				var tableTimeTrackDocumentTypes = tableItems.Where(x => x.OrganisationUID == organisationUID);
				if (tableTimeTrackDocumentTypes != null)
				{
					var timeTrackDocumentTypes = new List<API.TimeTrackDocumentType>();
					foreach (var tableItem in tableTimeTrackDocumentTypes)
					{
						timeTrackDocumentTypes.Add(Translate(tableItem));
					}
					return new OperationResult<List<API.TimeTrackDocumentType>>(timeTrackDocumentTypes);
				}
				return new OperationResult<List<API.TimeTrackDocumentType>>(new List<API.TimeTrackDocumentType>());
			}
			catch (Exception e)
			{
				return OperationResult<List<API.TimeTrackDocumentType>>.FromError(e.Message);
			}
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

		public OperationResult AddTimeTrackDocumentType(API.TimeTrackDocumentType timeTrackDocumentType)
		{
			try
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
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}
		public OperationResult EditTimeTrackDocumentType(API.TimeTrackDocumentType timeTrackDocumentType)
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
					Context.SaveChanges();
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
				Context.TimeTrackDocumentTypes.Remove(tableItem);
				Context.SaveChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}
	}
}