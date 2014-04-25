using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;
using System;
using System.Collections.Generic;
using FiresecClient.SKDHelpers;

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
						var accessTemplateViewModel = new AccessTemplateViewModel(accessTemplate);
						organisationViewModel.AddChild(accessTemplateViewModel);
						AllAccessTemplates.Add(accessTemplateViewModel);
					}
				}
			}
			SelectedAccessTemplate = Organisations.FirstOrDefault();
			OnPropertyChanged("RootAccessTemplates");
		}

		public List<AccessTemplateViewModel> AllAccessTemplates;

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

		List<AccessTemplateViewModel> _organisations;
		public List<AccessTemplateViewModel> Organisations
		{
			get { return _organisations; }
			private set
			{
				_organisations = value;
				OnPropertyChanged("Organisations");
			}
		}

		public Organisation Organisation
		{
			get
			{
				AccessTemplateViewModel OrganisationViewModel = SelectedAccessTemplate;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedAccessTemplate.Parent;

				if (OrganisationViewModel != null)
					return OrganisationViewModel.Organisation;

				return null;
			}
		}

		public AccessTemplateViewModel[] RootAccessTemplates
		{
			get { return Organisations.ToArray(); }
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
			var accessTemplateDetailsViewModel = new AccessTemplateDetailsViewModel(this, Organisation);
			if (DialogService.ShowModalWindow(accessTemplateDetailsViewModel))
			{
				var accessTemplateViewModel = new AccessTemplateViewModel(accessTemplateDetailsViewModel.AccessTemplate);

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
			var accessTemplateDetailsViewModel = new AccessTemplateDetailsViewModel(this, Organisation, SelectedAccessTemplate.AccessTemplate);
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