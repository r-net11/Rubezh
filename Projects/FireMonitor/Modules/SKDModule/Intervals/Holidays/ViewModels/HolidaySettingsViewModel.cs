using System;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class HolidaySettingsViewModel : SaveCancelDialogViewModel
	{
		HolidaySettings HolidaySettings;
		
		public HolidaySettingsViewModel(Guid organisationUID)
		{
			Title = "Настройки праздничных дней";
			HolidaySettings = HolidaySettingsHelper.GetByOrganisation(organisationUID);
			if (HolidaySettings == null)
				HolidaySettings = new HolidaySettings { OrganisationUID = organisationUID };
		}

		public TimeSpan NightStartTime
		{
			get { return HolidaySettings.NightStartTime; }
			set
			{
				HolidaySettings.NightStartTime = value;
				OnPropertyChanged(() => NightStartTime);
			}
		}

		public TimeSpan NightEndTime
		{
			get { return HolidaySettings.NightEndTime; }
			set
			{
				HolidaySettings.NightEndTime = value;
				OnPropertyChanged(() => NightEndTime);
			}
		}

		public TimeSpan EveningStartTime
		{
			get { return HolidaySettings.EveningStartTime; }
			set
			{
				HolidaySettings.EveningStartTime = value;
				OnPropertyChanged(() => EveningStartTime);
			}
		}

		public TimeSpan EveningEndTime
		{
			get { return HolidaySettings.EveningEndTime; }
			set
			{
				HolidaySettings.EveningEndTime = value;
				OnPropertyChanged(() => EveningEndTime);
			}
		}

		protected override bool Save()
		{
			return HolidaySettingsHelper.Save(HolidaySettings);
		}
	}
}