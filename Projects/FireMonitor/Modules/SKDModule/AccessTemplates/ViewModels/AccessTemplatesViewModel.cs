using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Common;

namespace SKDModule.ViewModels
{
	public class AccessTemplatesViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		AccessTemplate _clipboard;

		public AccessTemplatesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
		}

		public void Initialize(AccessTemplateFilter filter)
		{
			var organisations = OrganisationHelper.GetByCurrentUser();
			if (organisations == null)
				return;
			var accessTemplates = AccessTemplateHelper.Get(filter);
			if (accessTemplates == null)
				return;
			AllAccessTemplates = new List<AccessTemplateViewModel>();
			Organisations = new List<AccessTemplateViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new AccessTemplateViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllAccessTemplates.Add(organisationViewModel);
				if (accessTemplates != null)
				{
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
			}
			OnPropertyChanged(() => Organisations);
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
				OnPropertyChanged(() => SelectedAccessTemplate);
			}
		}

		public AccessTemplateViewModel ParentOrganisation
		{
			get
			{
				AccessTemplateViewModel OrganisationViewModel = SelectedAccessTemplate;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedAccessTemplate.Parent;

				if (OrganisationViewModel.Organisation != null)
					return OrganisationViewModel;

				return null;
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

		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			_clipboard = CopyAccessTemplate(SelectedAccessTemplate.AccessTemplate, false);
		}
		private bool CanCopy()
		{
			return SelectedAccessTemplate != null && !SelectedAccessTemplate.IsOrganisation;
		}

		public RelayCommand PasteCommand { get; private set; }
		private void OnPaste()
		{
			var newAccessTemplate = CopyAccessTemplate(_clipboard);
			if (AccessTemplateHelper.Save(newAccessTemplate))
			{
				var accessTemplateViewModel = new AccessTemplateViewModel(SelectedAccessTemplate.Organisation, newAccessTemplate);
				if (ParentOrganisation != null)
				{
					ParentOrganisation.AddChild(accessTemplateViewModel);
					AllAccessTemplates.Add(accessTemplateViewModel);
				}
				SelectedAccessTemplate = accessTemplateViewModel;
			}
		}
		private bool CanPaste()
		{
			return SelectedAccessTemplate != null && _clipboard != null;
		}

		AccessTemplate CopyAccessTemplate(AccessTemplate source, bool newName = true)
		{
			var copy = new AccessTemplate();
			copy.Name = newName ? CopyHelper.CopyName(source.Name, ParentOrganisation.Children.Select(item => item.Name)) : source.Name;
			copy.Description = source.Description;
			copy.OrganisationUID = ParentOrganisation.Organisation.UID;
			return copy;
		}
	}
}