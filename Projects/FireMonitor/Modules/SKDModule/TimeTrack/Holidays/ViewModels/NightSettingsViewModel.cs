using System;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class NightSettingsViewModel : SaveCancelDialogViewModel
	{
		NightSettings NightSettings;
		
		public NightSettingsViewModel(Guid organisationUID)
		{
			Title = "Настройки ночных интервалов";
			NightSettings = NightSettingsHelper.GetByOrganisation(organisationUID);
			if (NightSettings == null)
				NightSettings = new NightSettings { OrganisationUID = organisationUID };
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
			return NightSettingsHelper.Save(NightSettings);
		}
	}
}