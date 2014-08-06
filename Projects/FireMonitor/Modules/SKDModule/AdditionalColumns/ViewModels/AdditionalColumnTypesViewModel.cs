using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class AdditionalColumnTypesViewModel : ViewPartViewModel
	{
		public AdditionalColumnTypesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
		}

		public void Initialize(AdditionalColumnTypeFilter filter)
		{
			var organisations = OrganisationHelper.GetByCurrentUser();
			if (organisations == null)
				return;
			var additionalColumnTypes = AdditionalColumnTypeHelper.Get(filter);
			if (additionalColumnTypes == null)
				return;
			var types = AdditionalColumnTypeHelper.Get(new AdditionalColumnTypeFilter { UserUID = FiresecManager.CurrentUser.UID, PersonType = PersonType.Guest });
			if (types == null)
				return;

			AllAdditionalColumnTypes = new List<AdditionalColumnTypeViewModel>();
			Organisations = new List<AdditionalColumnTypeViewModel>();
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

		public List<AdditionalColumnTypeViewModel> Organisations { get; private set; }
		List<AdditionalColumnTypeViewModel> AllAdditionalColumnTypes { get; set; }

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