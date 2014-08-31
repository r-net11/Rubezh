using System;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;

namespace SKDDriver
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
			apiItem.EveningStartTime = TimeSpan.FromTicks(tableItem.EveningStartTime);
			apiItem.EveningEndTime = TimeSpan.FromTicks(tableItem.EveningEndTime);
			return apiItem;
		}

		protected override void TranslateBack(DataAccess.NightSetting tableItem, NightSettings apiItem)
		{
			tableItem.OrganisationUID = apiItem.OrganisationUID;
			tableItem.NightStartTime = apiItem.NightStartTime.Ticks;
			tableItem.NightEndTime = apiItem.NightEndTime.Ticks;
			tableItem.EveningStartTime = apiItem.EveningStartTime.Ticks;
			tableItem.EveningEndTime = apiItem.EveningEndTime.Ticks;
		}

		public OperationResult<NightSettings> GetByOrganisation(Guid uid)
		{
			try
			{
				var tableItem = Table.FirstOrDefault(x => x.OrganisationUID.Equals(uid));
				if (tableItem == null)
					return new OperationResult<NightSettings>("Настройки ночных и вечерних интервалов для данной организации не найдены");
				return new OperationResult<NightSettings> { Result = Translate(tableItem) };
			}
			catch (Exception e)
			{
				return new OperationResult<NightSettings>(e.Message);
			}
		}
	}
}