using System;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;


namespace SKDDriver
{
	public class HolidaySettingsTranslator : TranslatorBase<DataAccess.HolidaySetting, HolidaySettings>
	{
		public HolidaySettingsTranslator(DataAccess.SKDDataContext context)
			: base(context)
		{

		}

		protected override HolidaySettings Translate(DataAccess.HolidaySetting tableItem)
		{
			var apiItem = base.Translate(tableItem);
			apiItem.OrganisationUID = tableItem.OrganisationUID != null ? tableItem.OrganisationUID.Value : Guid.Empty;
			apiItem.NightStartTime = TimeSpan.FromTicks(tableItem.NightStartTime);
			apiItem.NightEndTime = TimeSpan.FromTicks(tableItem.NightEndTime);
			apiItem.EveningStartTime = TimeSpan.FromTicks(tableItem.EveningStartTime);
			apiItem.EveningEndTime = TimeSpan.FromTicks(tableItem.EveningEndTime);
			return apiItem;
		}

		protected override void TranslateBack(DataAccess.HolidaySetting tableItem, HolidaySettings apiItem)
		{
			tableItem.OrganisationUID = apiItem.OrganisationUID;
			tableItem.NightStartTime = apiItem.NightStartTime.Ticks;
			tableItem.NightEndTime = apiItem.NightEndTime.Ticks;
			tableItem.EveningStartTime = apiItem.EveningStartTime.Ticks;
			tableItem.EveningEndTime = apiItem.EveningEndTime.Ticks;
		}

		public OperationResult<HolidaySettings> GetByOrganisation(Guid uid)
		{
			try
			{
				var tableItem = Table.FirstOrDefault(x => x.OrganisationUID.Equals(uid));
				if (tableItem == null)
					return new OperationResult<HolidaySettings>("Настройки праздничных дней для данной организации не найдены");
				return new OperationResult<HolidaySettings> { Result = Translate(tableItem) };
			}
			catch (Exception e)
			{
				return new OperationResult<HolidaySettings>(e.Message);
			}
		}

	}
}
