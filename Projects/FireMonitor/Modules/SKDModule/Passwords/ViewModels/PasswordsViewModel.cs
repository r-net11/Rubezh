using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using SKDModule.Common;

namespace SKDModule.ViewModels
{
	public class PasswordsViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		ShortPassword _clipboard;

		public PasswordsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
		}

		public void Initialize(PasswordFilter filter)
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter() { UIDs = FiresecManager.CurrentUser.OrganisationUIDs });
			var passwords = PasswordHelper.Get(filter);

			AllPasswords = new List<PasswordViewModel>();
			Organisations = new List<PasswordViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new PasswordViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllPasswords.Add(organisationViewModel);
				foreach (var password in passwords)
				{
					if (password.OrganisationUID == organisation.UID)
					{
						var passwordViewModel = new PasswordViewModel(organisation, password);
						organisationViewModel.AddChild(passwordViewModel);
						AllPasswords.Add(passwordViewModel);
					}
				}
			}
			OnPropertyChanged("Organisations");
			SelectedPassword = Organisations.FirstOrDefault();
		}

		public List<PasswordViewModel> Organisations { get; private set; }
		List<PasswordViewModel> AllPasswords { get; set; }

		public void Select(Guid passwordUID)
		{
			if (passwordUID != Guid.Empty)
			{
				var passwordViewModel = AllPasswords.FirstOrDefault(x => x.Password != null && x.Password.UID == passwordUID);
				if (passwordViewModel != null)
					passwordViewModel.ExpandToThis();
				SelectedPassword = passwordViewModel;
			}
		}

		PasswordViewModel _selectedPassword;
		public PasswordViewModel SelectedPassword
		{
			get { return _selectedPassword; }
			set
			{
				_selectedPassword = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged("SelectedPassword");

				if (value != null)
				{
					PasswordGuardZonesViewModel = new PasswordGuardZonesViewModel(SelectedPassword.Password);
				}
				else
				{
					PasswordGuardZonesViewModel = null;
				}
			}
		}

		PasswordGuardZonesViewModel _passwordGuardZonesViewModel;
		public PasswordGuardZonesViewModel PasswordGuardZonesViewModel
		{
			get { return _passwordGuardZonesViewModel; }
			set
			{
				_passwordGuardZonesViewModel = value;
				OnPropertyChanged("PasswordGuardZonesViewModel");
			}
		}

		public PasswordViewModel ParentOrganisation
		{
			get
			{
				PasswordViewModel OrganisationViewModel = SelectedPassword;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedPassword.Parent;

				if (OrganisationViewModel.Organisation != null)
					return OrganisationViewModel;

				return null;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var passwordDetailsViewModel = new PasswordDetailsViewModel(SelectedPassword.Organisation.UID);
			if (DialogService.ShowModalWindow(passwordDetailsViewModel))
			{
				var passwordViewModel = new PasswordViewModel(SelectedPassword.Organisation, passwordDetailsViewModel.ShortPassword);

				PasswordViewModel OrganisationViewModel = SelectedPassword;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedPassword.Parent;

				if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
					return;

				OrganisationViewModel.AddChild(passwordViewModel);
				SelectedPassword = passwordViewModel;
			}
		}
		bool CanAdd()
		{
			return SelectedPassword != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			PasswordViewModel OrganisationViewModel = SelectedPassword;
			if (!OrganisationViewModel.IsOrganisation)
				OrganisationViewModel = SelectedPassword.Parent;

			if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
				return;

			var index = OrganisationViewModel.Children.ToList().IndexOf(SelectedPassword);
			var password = SelectedPassword.Password;
			bool removeResult = PasswordHelper.MarkDeleted(password.UID);
			if (!removeResult)
				return;
			OrganisationViewModel.RemoveChild(SelectedPassword);
			index = Math.Min(index, OrganisationViewModel.Children.Count() - 1);
			if (index > -1)
				SelectedPassword = OrganisationViewModel.Children.ToList()[index];
			else
				SelectedPassword = OrganisationViewModel;
		}
		bool CanRemove()
		{
			return SelectedPassword != null && !SelectedPassword.IsOrganisation;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var passwordDetailsViewModel = new PasswordDetailsViewModel(SelectedPassword.Organisation.UID, SelectedPassword.Password.UID);
			if (DialogService.ShowModalWindow(passwordDetailsViewModel))
			{
				SelectedPassword.Update(passwordDetailsViewModel.ShortPassword);
			}
		}
		bool CanEdit()
		{
			return SelectedPassword != null && SelectedPassword.Parent != null && !SelectedPassword.IsOrganisation;
		}

		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			_clipboard = CopyPassword(SelectedPassword.Password, false);
		}
		private bool CanCopy()
		{
			return SelectedPassword != null;
		}

		public RelayCommand PasteCommand { get; private set; }
		private void OnPaste()
		{
			var newShortPassword = CopyPassword(_clipboard);
			var password = new Password()
			{
				UID = newShortPassword.UID,
				Name = newShortPassword.Name,
				Description = newShortPassword.Description
			};
			if (PasswordHelper.Save(password))
			{
				var passwordViewModel = new PasswordViewModel(SelectedPassword.Organisation, newShortPassword);
				if (ParentOrganisation != null)
				{
					ParentOrganisation.AddChild(passwordViewModel);
					AllPasswords.Add(passwordViewModel);
				}
				SelectedPassword = passwordViewModel;
			}
		}
		private bool CanPaste()
		{
			return _clipboard != null;
		}

		ShortPassword CopyPassword(ShortPassword source, bool newName = true)
		{
			var copy = new ShortPassword();
			copy.Name = newName ? CopyHelper.CopyName(source.Name, ParentOrganisation.Children.Select(item => item.Name)) : source.Name;
			copy.Description = source.Description;
			copy.OrganisationUID = ParentOrganisation.Organisation.UID;
			return copy;
		}
	}
}