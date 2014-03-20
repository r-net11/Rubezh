using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using SKDModule.ViewModels;
using FiresecAPI;
using Infrastructure.Common.Windows;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class OrganisationAccessTemplatesViewModel : BaseViewModel
	{
		public Organization Organization { get; private set; } 

		public OrganisationAccessTemplatesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
		}

		public void Initialize(Organization organization, List<AccessTemplate> accessTemplates)
		{
			Organization = organization;

			AccessTemplates = new ObservableCollection<AccessTemplateViewModel>();
			foreach (var accessTemplate in accessTemplates)
			{
				var accessTemplateViewModel = new AccessTemplateViewModel(accessTemplate);
				AccessTemplates.Add(accessTemplateViewModel);
			}
			SelectedAccessTemplate = AccessTemplates.FirstOrDefault();
		}

		ObservableCollection<AccessTemplateViewModel> _accessTemplates;
		public ObservableCollection<AccessTemplateViewModel> AccessTemplates
		{
			get { return _accessTemplates; }
			set
			{
				_accessTemplates = value;
				OnPropertyChanged("AccessTemplates");
			}
		}

		AccessTemplateViewModel _selectedAccessTemplate;
		public AccessTemplateViewModel SelectedAccessTemplate
		{
			get { return _selectedAccessTemplate; }
			set
			{
				_selectedAccessTemplate = value;
				OnPropertyChanged("SelectedAccessTemplate");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var accessTemplateDetailsViewModel = new AccessTemplateDetailsViewModel(this);
			if (DialogService.ShowModalWindow(accessTemplateDetailsViewModel))
			{
				var accessTemplate = accessTemplateDetailsViewModel.AccessTemplate;
				var saveResult = AccessTemplateHelper.Save(accessTemplate);
				if (saveResult == false)
					return;
				var accessTemplateViewModel = new AccessTemplateViewModel(accessTemplate);
				AccessTemplates.Add(accessTemplateViewModel);
				SelectedAccessTemplate = accessTemplateViewModel;
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var accessTemplate = SelectedAccessTemplate.AccessTemplate;
			var removeResult = AccessTemplateHelper.MarkDeleted(accessTemplate);
			if (removeResult == false)
				return;
			var index = AccessTemplates.IndexOf(SelectedAccessTemplate);
			AccessTemplates.Remove(SelectedAccessTemplate);
			index = Math.Min(index, AccessTemplates.Count - 1);
			if (index > -1)
				SelectedAccessTemplate = AccessTemplates[index];
		}
		bool CanRemove()
		{
			return SelectedAccessTemplate != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var accessTemplateDetailsViewModel = new AccessTemplateDetailsViewModel(this, SelectedAccessTemplate.AccessTemplate);
			if (DialogService.ShowModalWindow(accessTemplateDetailsViewModel))
			{
				var accessTemplate = accessTemplateDetailsViewModel.AccessTemplate;
				var saveResult = AccessTemplateHelper.Save(accessTemplate);
				if (saveResult == false)
					return;
				SelectedAccessTemplate.Update(accessTemplate);
			}
		}
		bool CanEdit()
		{
			return SelectedAccessTemplate != null;
		}
	}
}