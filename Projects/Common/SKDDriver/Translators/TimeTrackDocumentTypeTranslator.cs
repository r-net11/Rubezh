using FiresecAPI;
using FiresecAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;
using OperationResult = FiresecAPI.OperationResult;
using Organisation = FiresecAPI.SKD.Organisation;
using TimeTrackDocumentType = FiresecAPI.SKD.TimeTrackDocumentType;

namespace SKDDriver.Translators
{
	public class TimeTrackDocumentTypeTranslator
	{
		protected SKDDatabaseService DatabaseService;
		protected DataAccess.SKDDataContext Context;

		protected List<TimeTrackDocumentType> GetAllSystemDocuments()
		{
			return new List<TimeTrackDocumentType>
			{
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String1, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String1_Code, 1, DocumentType.Presence),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String2, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String2_Code, 2, DocumentType.Presence),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String3, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String3_Code, 3, DocumentType.Overtime),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String4, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String4_Code, 4, DocumentType.Overtime),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String5, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String5_Code, 5, DocumentType.Presence),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String6, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String6_Code, 6, DocumentType.Presence),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String7, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String7_Code, 7, DocumentType.Presence),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String8, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String8_Code, 8, DocumentType.Presence),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String9, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String9_Code, 9, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String10, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String10_Code, 10, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String11, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String11_Code, 11, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String12, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String12_Code, 12, DocumentType.Presence),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String13, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String13_Code, 13, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String14, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String14_Code, 14, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String15, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String15_Code, 15, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String16, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String16_Code, 16, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String17, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String17_Code, 17, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String18, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String18_Code, 18, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String19, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String19_Code, 19, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String20, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String20_Code, 20, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String21, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String21_Code, 21, DocumentType.Presence),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String22, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String22_Code, 22, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String23, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String23_Code, 23, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String24, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String24_Code, 24, DocumentType.Absence),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String25, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String25_Code, 25, DocumentType.Presence),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String26, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String26_Code, 26, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String27, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String27_Code, 27, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String28, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String28_Code, 28, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String29, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String29_Code, 29, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String30, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String30_Code, 30, DocumentType.Absence),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String31, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String31_Code, 31, DocumentType.Presence),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String32, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String32_Code, 32, DocumentType.Presence),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String33, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String33_Code, 33, DocumentType.Absence),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String34, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String34_Code, 34, DocumentType.AbsenceReasonable),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String35, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String35_Code, 35, DocumentType.Absence),
				new TimeTrackDocumentType(Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String36, Resources.Language.Translators.TimeTrackDocumentTypeTranslator.String36_Code, 36, DocumentType.AbsenceReasonable)
			};
		}

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
						IsSystem = tableTimeTrackDocumentType.IsSystem
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

		public OperationResult AddSystemDocumentTypesForOrganisation(Guid organisationGuid)
		{
			try
			{
				var systemDocuments = GetAllSystemDocuments().Select(x => new DataAccess.TimeTrackDocumentType
				{
					UID = Guid.NewGuid(),
					DocumentCode = x.Code,
					DocumentType = (int) x.DocumentType,
					Name = x.Name,
					ShortName = x.ShortName,
					OrganisationUID = organisationGuid,
					IsSystem = true
				});
				Context.TimeTrackDocumentTypes.InsertAllOnSubmit(systemDocuments);
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
				var tableItem = Context.TimeTrackDocumentTypes.FirstOrDefault(x => x.UID.Equals(timeTrackDocumentType.UID));
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

		public TimeTrackDocumentType Get(int code)
		{
			var tableItem = Context.TimeTrackDocumentTypes.FirstOrDefault(x => x.DocumentCode == code);
			return tableItem == null ? null : Translate(tableItem);
		}

		private TimeTrackDocumentType Translate(DataAccess.TimeTrackDocumentType tableItem)
		{
			return new TimeTrackDocumentType
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				ShortName = tableItem.ShortName,
				Code = tableItem.DocumentCode,
				DocumentType = (DocumentType)tableItem.DocumentType,
				OrganisationUID = tableItem.OrganisationUID,
				IsSystem = tableItem.IsSystem
			};
		}
	}
}