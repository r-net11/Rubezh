using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	[SaveSizeAttribute]
	public class OrganisationsSelectionViewModel : SaveCancelDialogViewModel
	{
		public List<Organisation> Organisations { get; private set; }

		public OrganisationsSelectionViewModel(List<Organisation> organisations)
		{
			Title = "Выбор организаций";
			AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
			AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
			RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);

			Organisations = organisations;
			TargetOrganisations = new ObservableCollection<Organisation>();
			SourceOrganisations = new ObservableCollection<Organisation>();

			foreach (var organisation in OrganisationHelper.GetByCurrentUser())
			{
				if (Organisations.Any(x => x.UID == organisation.UID))
					TargetOrganisations.Add(organisation);
				else
					SourceOrganisations.Add(organisation);
			}

			SelectedTargetOrganisation = TargetOrganisations.FirstOrDefault();
			SelectedSourceOrganisation = SourceOrganisations.FirstOrDefault();
		}

		public ObservableCollection<Organisation> SourceOrganisations { get; private set; }

		Organisation _selectedSourceOrganisation;
		public Organisation SelectedSourceOrganisation
		{
			get { return _selectedSourceOrganisation; }
			set
			{
				_selectedSourceOrganisation = value;
				OnPropertyChanged(() => SelectedSourceOrganisation);
			}
		}

		public ObservableCollection<Organisation> TargetOrganisations { get; private set; }

		Organisation _selectedTargetOrganisation;
		public Organisation SelectedTargetOrganisation
		{
			get { return _selectedTargetOrganisation; }
			set
			{
				_selectedTargetOrganisation = value;
				OnPropertyChanged(() => SelectedTargetOrganisation);
			}
		}

		public RelayCommand<object> AddCommand { get; private set; }
		public IList SelectedSourceOrganisations;
		void OnAdd(object parameter)
		{
			var index = SourceOrganisations.IndexOf(SelectedSourceOrganisation);

			SelectedSourceOrganisations = (IList)parameter;
			var organisationViewModels = new List<Organisation>();
			foreach (var selectedOrganisation in SelectedSourceOrganisations)
			{
				var organisationViewModel = selectedOrganisation as Organisation;
				if (organisationViewModel != null)
					organisationViewModels.Add(organisationViewModel);
			}
			foreach (var organisationViewModel in organisationViewModels)
			{
				TargetOrganisations.Add(organisationViewModel);
				SourceOrganisations.Remove(organisationViewModel);
			}
			SelectedTargetOrganisation = TargetOrganisations.LastOrDefault();
			OnPropertyChanged("SourceOrganisations");

			index = Math.Min(index, SourceOrganisations.Count - 1);
			if (index > -1)
				SelectedSourceOrganisation = SourceOrganisations[index];
		}

		public RelayCommand<object> RemoveCommand { get; private set; }
		public IList SelectedTargetOrganisations;
		void OnRemove(object parameter)
		{
			var index = TargetOrganisations.IndexOf(SelectedTargetOrganisation);

			SelectedTargetOrganisations = (IList)parameter;
			var organisationViewModels = new List<Organisation>();
			foreach (var selectedOrganisation in SelectedTargetOrganisations)
			{
				var organisationViewModel = selectedOrganisation as Organisation;
				if (organisationViewModel != null)
					organisationViewModels.Add(organisationViewModel);
			}
			foreach (var organisationViewModel in organisationViewModels)
			{
				SourceOrganisations.Add(organisationViewModel);
				TargetOrganisations.Remove(organisationViewModel);
			}
			SelectedSourceOrganisation = SourceOrganisations.LastOrDefault();
			OnPropertyChanged(() => TargetOrganisations);

			index = Math.Min(index, TargetOrganisations.Count - 1);
			if (index > -1)
				SelectedTargetOrganisation = TargetOrganisations[index];
		}

		public RelayCommand AddAllCommand { get; private set; }
		void OnAddAll()
		{
			foreach (var organisationViewModel in SourceOrganisations)
			{
				TargetOrganisations.Add(organisationViewModel);
			}
			SourceOrganisations.Clear();
			SelectedTargetOrganisation = TargetOrganisations.FirstOrDefault();
		}

		public RelayCommand RemoveAllCommand { get; private set; }
		void OnRemoveAll()
		{
			foreach (var organisationViewModel in TargetOrganisations)
			{
				SourceOrganisations.Add(organisationViewModel);
			}
			TargetOrganisations.Clear();
			SelectedSourceOrganisation = SourceOrganisations.FirstOrDefault();
		}

		public bool CanAdd(object parameter)
		{
			return SelectedSourceOrganisation != null;
		}

		public bool CanRemove(object parameter)
		{
			return SelectedTargetOrganisation != null;
		}

		public bool CanAddAll()
		{
			return (SourceOrganisations.Count > 0);
		}

		public bool CanRemoveAll()
		{
			return (TargetOrganisations.Count > 0);
		}

		protected override bool Save()
		{
			Organisations = new List<Organisation>(TargetOrganisations);
			return base.Save();
		}
	}
}