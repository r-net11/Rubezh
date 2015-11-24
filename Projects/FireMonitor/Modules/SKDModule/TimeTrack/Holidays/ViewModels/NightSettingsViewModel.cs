﻿using System;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class NightSettingsViewModel : SaveCancelDialogViewModel
	{
		private NightSettings _nightSettings;

		public NightSettingsViewModel(Guid organisationUID)
		{
			Title = "Настройка интервала ночного времени";
			_nightSettings = NightSettingsHelper.GetByOrganisation(organisationUID) ??
			                new NightSettings { OrganisationUID = organisationUID };
		}

		public TimeSpan NightStartTime
		{
			get { return _nightSettings.NightStartTime; }
			set
			{
				_nightSettings.NightStartTime = value;
				OnPropertyChanged(() => NightStartTime);
			}
		}

		public TimeSpan NightEndTime
		{
			get { return _nightSettings.NightEndTime; }
			set
			{
				_nightSettings.NightEndTime = value;
				OnPropertyChanged(() => NightEndTime);
			}
		}

		public bool IsNightSettingsEnabled
		{
			get { return _nightSettings.IsNightSettingsEnabled; }
			set
			{
				_nightSettings.IsNightSettingsEnabled = value;
				OnPropertyChanged(() => IsNightSettingsEnabled);
			}
		}

		protected override bool Save()
		{
			return NightSettingsHelper.Save(_nightSettings);
		}
	}
}