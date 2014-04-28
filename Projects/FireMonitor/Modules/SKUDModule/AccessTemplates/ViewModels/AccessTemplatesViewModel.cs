using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class AccessTemplatesViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		AccessTemplateFilter Filter;

		public AccessTemplatesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			EditFilterCommand = new RelayCommand(OnEditFilter);
			Filter = new AccessTemplateFilter() { OrganisationUIDs = FiresecManager.CurrentUser.OrganisationUIDs };
			Initialize(Filter);
		}

		public void Initialize(AccessTemplateFilter filter)
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter() { UIDs = FiresecManager.CurrentUser.OrganisationUIDs });
			var accessTemplates = AccessTemplateHelper.Get(filter);

			AllAccessTemplates = new List<AccessTemplateViewModel>();
			Organisations = new List<AccessTemplateViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new AccessTemplateViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllAccessTemplates.Add(organisationViewModel);
				foreach (var accessTemplate in accessTemplates)
				{
					if (accessTemplate.OrganisationUID == organisation.UID)
					{
						var accessTemplateViewModel = new AccessTemplateViewModel(organisation, accessTemplate);
						organisationViewModel.AddChild(accessTemplateViewModel);
						AllAccessTemplates.Add(accessTemplateViewModel);
					}
				}
			}
			OnPropertyChanged("Organisations");
			SelectedAccessTemplate = Organisations.FirstOrDefault();
		}

		public List<AccessTemplateViewModel> Organisations { get; private set; }
		List<AccessTemplateViewModel> AllAccessTemplates { get; set; }

		public void Select(Guid accessTemplateUID)
		{
			if (accessTemplateUID != Guid.Empty)
			{
				var AccessTemplateViewModel = AllAccessTemplates.FirstOrDefault(x => x.AccessTemplate != null && x.AccessTemplate.UID == accessTemplateUID);
				if (AccessTemplateViewModel != null)
					AccessTemplateViewModel.ExpandToThis();
				SelectedAccessTemplate = AccessTemplateViewModel;
			}
		}

		AccessTemplateViewModel _selectedAccessTemplate;
		public AccessTemplateViewModel SelectedAccessTemplate
		{
			get { return _selectedAccessTemplate; }
			set
			{
				_selectedAccessTemplate = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged("SelectedAccessTemplate");
			}
		}

		public RelayCommand EditFilterCommand { get; private set; }
		void OnEditFilter()
		{
			var filterViewModel = new AccessTemplateFilterViewModel(Filter);
			if (DialogService.ShowModalWindow(filterViewModel))
			{
				Filter = filterViewModel.Filter;
				Initialize(Filter);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var accessTemplateDetailsViewModel = new AccessTemplateDetailsViewModel(SelectedAccessTemplate.Organisation);
			if (DialogService.ShowModalWindow(accessTemplateDetailsViewModel))
			{
				var accessTemplateViewModel = new AccessTemplateViewModel(SelectedAccessTemplate.Organisation, accessTemplateDetailsViewModel.AccessTemplate);

				AccessTemplateViewModel OrganisationViewModel = SelectedAccessTemplate;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedAccessTemplate.Parent;

				if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
					return;

				OrganisationViewModel.AddChild(accessTemplateViewModel);
				SelectedAccessTemplate = accessTemplateViewModel;
			}
		}
		bool CanAdd()
		{
			return SelectedAccessTemplate != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			AccessTemplateViewModel OrganisationViewModel = SelectedAccessTemplate;
			if (!OrganisationViewModel.IsOrganisation)
				OrganisationViewModel = SelectedAccessTemplate.Parent;

			if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
				return;

			var index = OrganisationViewModel.Children.ToList().IndexOf(SelectedAccessTemplate);
			var accessTemplate = SelectedAccessTemplate.AccessTemplate;
			bool removeResult = AccessTemplateHelper.MarkDeleted(accessTemplate);
			if (!removeResult)
				return;
			OrganisationViewModel.RemoveChild(SelectedAccessTemplate);
			index = Math.Min(index, OrganisationViewModel.Children.Count() - 1);
			if (index > -1)
				SelectedAccessTemplate = OrganisationViewModel.Children.ToList()[index];
			else
				SelectedAccessTemplate = OrganisationViewModel;
		}
		bool CanRemove()
		{
			return SelectedAccessTemplate != null && !SelectedAccessTemplate.IsOrganisation;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var accessTemplateDetailsViewModel = new AccessTemplateDetailsViewModel(SelectedAccessTemplate.Organisation, SelectedAccessTemplate.AccessTemplate);
			if (DialogService.ShowModalWindow(accessTemplateDetailsViewModel))
			{
				SelectedAccessTemplate.Update(accessTemplateDetailsViewModel.AccessTemplate);
			}
		}
		bool CanEdit()
		{
			return SelectedAccessTemplate != null && SelectedAccessTemplate.Parent != null && !SelectedAccessTemplate.IsOrganisation;
		}
	}
}