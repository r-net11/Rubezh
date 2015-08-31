using System;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class NightSettingsViewModel : SaveCancelDialogViewModel
	{
		NightSettings NightSettings
		{
			get 
			{
				if (!HasSelected) return new NightSettings();
				var settings = NightSettingsHelper.GetByOrganisation(SelectedOrganisation.Organisation.UID);
				if (settings == null)
					settings = new NightSettings { OrganisationUID = SelectedOrganisation.Organisation.UID };
				return settings; 
			}
		}
		public List<FilterOrganisationViewModel> Organisations { get; private set; }
		
		public NightSettingsViewModel()
		{
			Title = "Настройки ночных интервалов";
			Organisations = new List<FilterOrganisationViewModel>();
			var filter = new OrganisationFilter() { UserUID = FiresecManager.CurrentUser.UID };
			var organisations = OrganisationHelper.Get(filter);
			if (organisations != null)
			{
				Organisations = new List<FilterOrganisationViewModel>();
				foreach (var organisation in organisations)
				{
					Organisations.Add(new FilterOrganisationViewModel(organisation));
				}
			}
		}

		FilterOrganisationViewModel _selectedOrganisation;
		public FilterOrganisationViewModel SelectedOrganisation
		{
			get { return _selectedOrganisation; }
			set
			{
				_selectedOrganisation = value;
				OnPropertyChanged(() => SelectedOrganisation);
				OnPropertyChanged(() => NightStartTime);
				OnPropertyChanged(() => NightEndTime);
				OnPropertyChanged(() => HasSelected);
			}
		}
		public bool HasSelected
		{
			get { return SelectedOrganisation != null; }
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