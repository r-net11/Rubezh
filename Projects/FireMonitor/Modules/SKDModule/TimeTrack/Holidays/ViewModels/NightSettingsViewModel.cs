using System;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class NightSettingsViewModel : SaveCancelDialogViewModel
	{
		NightSettings NightSettings;
		
		public NightSettingsViewModel(Guid organisationUID)
		{
			Title = "Настройки вечерних и ночных интервалов";
			NightSettings = NightSettingsHelper.GetByOrganisation(organisationUID);
			if (NightSettings == null)
				NightSettings = new NightSettings { OrganisationUID = organisationUID };
		}

		public TimeSpan EveningStartTime
		{
			get { return NightSettings.EveningStartTime; }
			set
			{
				NightSettings.EveningStartTime = value;
				OnPropertyChanged(() => EveningStartTime);
			}
		}

		public TimeSpan EveningEndTime
		{
			get { return NightSettings.EveningEndTime; }
			set
			{
				NightSettings.EveningEndTime = value;
				OnPropertyChanged(() => EveningEndTime);
			}
		}

		public TimeSpan NightStartTime
		{
			get { return NightSettings.NightStartTime; }
			set
			{
				NightSettings.NightStartTime = value;
				OnPropertyChanged(() => NightStartTime);
			}
		}

		public TimeSpan NightEndTime
		{
			get { return NightSettings.NightEndTime; }
			set
			{
				NightSettings.NightEndTime = value;
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
			if (EveningEndTime > NightStartTime && NightEndTime > TimeSpan.Zero)
			{
				MessageBoxService.ShowWarning("Начало ночного времени должно быть больше конца вечернего");
				return false;
			}
			return NightSettingsHelper.Save(NightSettings);
		}
	}
}