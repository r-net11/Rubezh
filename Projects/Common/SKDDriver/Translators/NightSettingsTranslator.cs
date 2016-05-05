using StrazhAPI;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StrazhDAL
{
	public class NightSettingsTranslator : TranslatorBase<DataAccess.NightSetting, NightSettings>
	{
		public NightSettingsTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

		protected override NightSettings Translate(DataAccess.NightSetting tableItem)
		{
			var apiItem = base.Translate(tableItem);
			apiItem.OrganisationUID = tableItem.OrganisationUID != null ? tableItem.OrganisationUID.Value : Guid.Empty;
			apiItem.NightStartTime = TimeSpan.FromTicks(tableItem.NightStartTime);
			apiItem.NightEndTime = TimeSpan.FromTicks(tableItem.NightEndTime);
			apiItem.IsNightSettingsEnabled = tableItem.IsNightSettingsEnabled;
			return apiItem;
		}

		protected override void TranslateBack(DataAccess.NightSetting tableItem, NightSettings apiItem)
		{
			tableItem.OrganisationUID = apiItem.OrganisationUID;
			tableItem.NightStartTime = apiItem.NightStartTime.Ticks;
			tableItem.NightEndTime = apiItem.NightEndTime.Ticks;
			tableItem.IsNightSettingsEnabled = apiItem.IsNightSettingsEnabled;
		}

		public OperationResult<NightSettings> GetByOrganisation(Guid uid)
		{
			return GetByOrganisation(uid, Table);
		}

		public OperationResult<NightSettings> GetByOrganisation(Guid uid, IEnumerable<DataAccess.NightSetting> tableItems)
		{
			try
			{
				var tableItem = tableItems.FirstOrDefault(x => x.OrganisationUID.Equals(uid));
				if (tableItem == null)
					return OperationResult<NightSettings>.FromError("Настройки ночных и вечерних интервалов для данной организации не найдены");
				return new OperationResult<NightSettings>(Translate(tableItem));
			}
			catch (Exception e)
			{
				return OperationResult<NightSettings>.FromError(e.Message);
			}
		}
	}
}