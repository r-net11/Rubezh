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
	public class PositionsViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		PositionFilter Filter;

		public PositionsViewModel()
		{
			EditFilterCommand = new RelayCommand(OnEditFilter);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Filter = new PositionFilter() { OrganisationUIDs = FiresecManager.CurrentUser.OrganisationUIDs };
			Initialize();
		}

		public void Initialize()
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter() { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
			var positions = PositionHelper.Get(Filter);

			AllPositions = new List<PositionViewModel>();
			Organisations = new List<PositionViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new PositionViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllPositions.Add(organisationViewModel);
				foreach (var position in positions)
				{
					if (position.OrganisationUID == organisation.UID)
					{
						var positionViewModel = new PositionViewModel(position);
						organisationViewModel.AddChild(positionViewModel);
						AllPositions.Add(positionViewModel);
					}
				}
			}

			foreach (var organisation in Organisations)
			{
				organisation.ExpandToThis();
			}
			OnPropertyChanged("RootPositions");
		}

		#region PositionSelection
		public List<PositionViewModel> AllPositions;

		public void Select(Guid positionUID)
		{
			if (positionUID != Guid.Empty)
			{
				var positionViewModel = AllPositions.FirstOrDefault(x => x.Position != null && x.Position.UID == positionUID);
				if (positionViewModel != null)
					positionViewModel.ExpandToThis();
				SelectedPosition = positionViewModel;
			}
		}
		#endregion

		PositionViewModel _selectedPosition;
		public PositionViewModel SelectedPosition
		{
			get { return _selectedPosition; }
			set
			{
				_selectedPosition = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged("SelectedPosition");
			}
		}

		List<PositionViewModel> _organisations;
		public List<PositionViewModel> Organisations
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
				PositionViewModel OrganisationViewModel = SelectedPosition;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedPosition.Parent;

				if (OrganisationViewModel != null)
					return OrganisationViewModel.Organisation;

				return null;
			}
		}

		public PositionViewModel[] RootPositions
		{
			get { return Organisations.ToArray(); }
		}

		public RelayCommand EditFilterCommand { get; private set; }
		void OnEditFilter()
		{
			var filterViewModel = new PositionFilterViewModel(Filter);
			if (DialogService.ShowModalWindow(filterViewModel))
			{
				Filter = filterViewModel.Filter;
				Initialize();
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var positionDetailsViewModel = new PositionDetailsViewModel(this, Organisation);
			if (DialogService.ShowModalWindow(positionDetailsViewModel))
			{
				var positionViewModel = new PositionViewModel(positionDetailsViewModel.ShortPosition);

				PositionViewModel OrganisationViewModel = SelectedPosition;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedPosition.Parent;

				if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
					return;

				OrganisationViewModel.AddChild(positionViewModel);
				SelectedPosition = positionViewModel;
			}
		}
		bool CanAdd()
		{
			return SelectedPosition != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			PositionViewModel OrganisationViewModel = SelectedPosition;
			if (!OrganisationViewModel.IsOrganisation)
				OrganisationViewModel = SelectedPosition.Parent;

			if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
				return;

			var index = OrganisationViewModel.Children.ToList().IndexOf(SelectedPosition);
			var position = SelectedPosition.Position;
			bool removeResult = PositionHelper.MarkDeleted(position.UID);
			if (!removeResult)
				return;
			OrganisationViewModel.RemoveChild(SelectedPosition);
			index = Math.Min(index, OrganisationViewModel.Children.Count() - 1);
			if (index > -1)
				SelectedPosition = OrganisationViewModel.Children.ToList()[index];
			else
				SelectedPosition = OrganisationViewModel;
		}
		bool CanRemove()
		{
			return SelectedPosition != null && !SelectedPosition.IsOrganisation;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var positionDetailsViewModel = new PositionDetailsViewModel(this, Organisation, SelectedPosition.Position.UID);
			if (DialogService.ShowModalWindow(positionDetailsViewModel))
			{
				SelectedPosition.Update(positionDetailsViewModel.ShortPosition);
			}
		}
		bool CanEdit()
		{
			return SelectedPosition != null && SelectedPosition.Parent != null && !SelectedPosition.IsOrganisation;
		}
	}
}