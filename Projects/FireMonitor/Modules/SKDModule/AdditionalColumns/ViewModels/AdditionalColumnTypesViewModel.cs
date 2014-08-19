using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class AdditionalColumnTypesViewModel : ViewPartViewModel
	{
		public AdditionalColumnTypesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			ServiceFactory.Events.GetEvent<EditOrganisationEvent>().Unsubscribe(OnEditOrganisation);
			ServiceFactory.Events.GetEvent<EditOrganisationEvent>().Subscribe(OnEditOrganisation);
			ServiceFactory.Events.GetEvent<OrganisationUsersChangedEvent>().Unsubscribe(OnOrganisationUsersChanged);
			ServiceFactory.Events.GetEvent<OrganisationUsersChangedEvent>().Subscribe(OnOrganisationUsersChanged);
		}

		public void Initialize(AdditionalColumnTypeFilter filter)
		{
			var organisations = OrganisationHelper.GetByCurrentUser();
			if (organisations == null)
				return;
			var additionalColumnTypes = AdditionalColumnTypeHelper.Get(filter);
			if (additionalColumnTypes == null)
				return;
			AllAdditionalColumnTypes = new ObservableCollection<AdditionalColumnTypeViewModel>();
			Organisations = new ObservableCollection<AdditionalColumnTypeViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new AdditionalColumnTypeViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllAdditionalColumnTypes.Add(organisationViewModel);
				foreach (var additionalColumnType in additionalColumnTypes)
				{
					if (additionalColumnType.OrganisationUID == organisation.UID)
					{
						var additionalColumnTypeViewModel = new AdditionalColumnTypeViewModel(organisation, additionalColumnType);
						organisationViewModel.AddChild(additionalColumnTypeViewModel);
						AllAdditionalColumnTypes.Add(additionalColumnTypeViewModel);
					}
				}
			}
			OnPropertyChanged(() => Organisations);
			SelectedAdditionalColumnType = Organisations.FirstOrDefault();
		}

		void OnEditOrganisation(Organisation newOrganisation)
		{
			var organisation = Organisations.FirstOrDefault(x => x.Organisation.UID == newOrganisation.UID);
			if (organisation != null)
			{
				organisation.Update(newOrganisation);
			}
			OnPropertyChanged(() => Organisations);
		}

		void OnOrganisationUsersChanged(Organisation newOrganisation)
		{
			if (newOrganisation.UserUIDs.Any(x => x == FiresecManager.CurrentUser.UID))
			{
				var organisationViewModel = new AdditionalColumnTypeViewModel(newOrganisation);
				Organisations.Add(organisationViewModel);
				AllAdditionalColumnTypes.Add(organisationViewModel);
				var employees = AdditionalColumnTypeHelper.GetShortByOrganisation(newOrganisation.UID);
				if (employees == null)
					return;
				foreach (var additionalColumnType in employees)
				{
					var employeeViewModel = new AdditionalColumnTypeViewModel(newOrganisation, additionalColumnType);
					organisationViewModel.AddChild(employeeViewModel);
					AllAdditionalColumnTypes.Add(employeeViewModel);
				}
				OnPropertyChanged(() => Organisations);
			}
			else
			{
				var organisationViewModel = Organisations.FirstOrDefault(x => x.Organisation.UID == newOrganisation.UID);
				if (organisationViewModel != null)
				{
					Organisations.Remove(organisationViewModel);
					AllAdditionalColumnTypes.Remove(organisationViewModel);
					OnPropertyChanged(() => Organisations);
				}
			}
		}

		public ObservableCollection<AdditionalColumnTypeViewModel> Organisations { get; private set; }
		ObservableCollection<AdditionalColumnTypeViewModel> AllAdditionalColumnTypes { get; set; }

		AdditionalColumnTypeViewModel _selectedAdditionalColumnType;
		public AdditionalColumnTypeViewModel SelectedAdditionalColumnType
		{
			get { return _selectedAdditionalColumnType; }
			set
			{
				_selectedAdditionalColumnType = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged(() => SelectedAdditionalColumnType);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var additionalColumnTypeDetailsViewModel = new AdditionalColumnTypeDetailsViewModel(SelectedAdditionalColumnType.Organisation);
			if (DialogService.ShowModalWindow(additionalColumnTypeDetailsViewModel))
			{
				var additionalColumnTypeViewModel = new AdditionalColumnTypeViewModel(SelectedAdditionalColumnType.Organisation, additionalColumnTypeDetailsViewModel.ShortAdditionalColumnType);

				AdditionalColumnTypeViewModel OrganisationViewModel = SelectedAdditionalColumnType;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedAdditionalColumnType.Parent;

				if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
					return;

				OrganisationViewModel.AddChild(additionalColumnTypeViewModel);
				SelectedAdditionalColumnType = additionalColumnTypeViewModel;
			}
		}
		bool CanAdd()
		{
			return SelectedAdditionalColumnType != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			AdditionalColumnTypeViewModel OrganisationViewModel = SelectedAdditionalColumnType;
			if (!OrganisationViewModel.IsOrganisation)
				OrganisationViewModel = SelectedAdditionalColumnType.Parent;

			if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
				return;

			var index = OrganisationViewModel.Children.ToList().IndexOf(SelectedAdditionalColumnType);
			var additionalColumnType = SelectedAdditionalColumnType.AdditionalColumnType;
			bool removeResult = AdditionalColumnTypeHelper.MarkDeleted(additionalColumnType.UID);
			if (!removeResult)
				return;
			OrganisationViewModel.RemoveChild(SelectedAdditionalColumnType);
			index = Math.Min(index, OrganisationViewModel.Children.Count() - 1);
			if (index > -1)
				SelectedAdditionalColumnType = OrganisationViewModel.Children.ToList()[index];
			else
				SelectedAdditionalColumnType = OrganisationViewModel;
		}
		bool CanRemove()
		{
			return SelectedAdditionalColumnType != null && !SelectedAdditionalColumnType.IsOrganisation;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var additionalColumnTypeDetailsViewModel = new AdditionalColumnTypeDetailsViewModel(SelectedAdditionalColumnType.Organisation, SelectedAdditionalColumnType.AdditionalColumnType.UID);
			if (DialogService.ShowModalWindow(additionalColumnTypeDetailsViewModel))
			{
				SelectedAdditionalColumnType.Update(additionalColumnTypeDetailsViewModel.ShortAdditionalColumnType);
			}
		}
		bool CanEdit()
		{
			return SelectedAdditionalColumnType != null && SelectedAdditionalColumnType.Parent != null && !SelectedAdditionalColumnType.IsOrganisation;
		}
	}
}