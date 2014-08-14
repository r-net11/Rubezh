using System;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

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

		protected override bool Save()
		{
			if (EveningStartTime > EveningEndTime)
			{
				MessageBoxService.ShowWarning("Начало вечернего времени должно быть раньше конца");
				return false;
			}
			if (NightStartTime > NightEndTime)
			{
				MessageBoxService.ShowWarning("Начало ночного времени должно быть раньше конца");
				return false;
			}
			if (EveningEndTime > NightStartTime && NightEndTime > TimeSpan.Zero)
			{
				MessageBoxService.ShowWarning("Начало ночного времени должно быть больше конца вечернего");
				return false;
			}
			return HolidaySettingsHelper.Save(HolidaySettings);
		}
	}
}