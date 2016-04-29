using System;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using RubezhClient;

namespace SKDModule.ViewModels
{
	public class NightSettingsViewModel : SaveCancelDialogViewModel
	{
		public NightSettings NightSettings { get; set; }
		public List<FilterOrganisationViewModel> Organisations { get; private set; }
		
		public NightSettingsViewModel()
		{
			Title = "Настройки ночных интервалов";
			Organisations = new List<FilterOrganisationViewModel>();
			var filter = new OrganisationFilter() { User = ClientManager.CurrentUser };
			var organisations = OrganisationHelper.Get(filter);
			if (organisations != null)
			{
				Organisations = new List<FilterOrganisationViewModel>();
				foreach (var organisation in organisations)
				{
					Organisations.Add(new FilterOrganisationViewModel(organisation));
				}
			}
			NightSettings = new NightSettings();
		}

		FilterOrganisationViewModel _selectedOrganisation;
		public FilterOrganisationViewModel SelectedOrganisation
		{
			get { return _selectedOrganisation; }
			set
			{
				_selectedOrganisation = value;
				OnPropertyChanged(() => SelectedOrganisation);
				NightSettings = NightSettingsHelper.GetByOrganisation(SelectedOrganisation.Organisation.UID);
				if (NightSettings == null)
					NightSettings = new NightSettings { OrganisationUID = SelectedOrganisation.Organisation.UID };
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