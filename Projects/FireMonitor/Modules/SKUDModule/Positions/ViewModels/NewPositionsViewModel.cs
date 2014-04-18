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
	public class NewPositionsViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		PositionFilter Filter;

		public NewPositionsViewModel()
		{
			EditFilterCommand = new RelayCommand(OnEditFilter);
			RefreshCommand = new RelayCommand(OnRefresh);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Filter = new PositionFilter();
			Initialize();
		}

		public void Initialize()
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter() { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
			var positions = PositionHelper.Get(Filter);

			AllPositions = new List<NewPositionViewModel>();
			Organisations = new List<NewPositionViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new NewPositionViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllPositions.Add(organisationViewModel);
				foreach (var position in positions)
				{
					if (position.OrganisationUID == organisation.UID)
					{
						var positionViewModel = new NewPositionViewModel(position);
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
		public List<NewPositionViewModel> AllPositions;

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

		NewPositionViewModel _selectedPosition;
		public NewPositionViewModel SelectedPosition
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

		List<NewPositionViewModel> _organisations;
		public List<NewPositionViewModel> Organisations
		{
			get { return _organisations; }
			private set
			{
				_organisations = value;
				OnPropertyChanged("Organisations");
			}
		}

		public NewPositionViewModel[] RootPositions
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

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			//NewPositionViewModel OrganisationViewModel = SelectedPosition;
			//if (!OrganisationViewModel.IsOrganisation)
			//    OrganisationViewModel = SelectedPosition.Parent;

			//if (OrganisationViewModel == null || OrganisationViewModel.Organization == null)
			//    return;

			//var positionDetailsViewModel = new PositionDetailsViewModel(this, OrganisationViewModel.Organization);
			//if (DialogService.ShowModalWindow(positionDetailsViewModel))
			//{
			//    var position = positionDetailsViewModel.Position;
			//    bool saveResult = PositionHelper.Save(position);
			//    if (!saveResult)
			//        return;
			//    var positionViewModel = new NewPositionViewModel(positionDetailsViewModel.ShortPosition);
			//    OrganisationViewModel.AddChild(positionViewModel);
			//    SelectedPosition = positionViewModel;
			//}
		}
		bool CanAdd()
		{
			return SelectedPosition != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			NewPositionViewModel OrganisationViewModel = SelectedPosition;
			if (!OrganisationViewModel.IsOrganisation)
				OrganisationViewModel = SelectedPosition.Parent;

			if (OrganisationViewModel == null || OrganisationViewModel.Organization == null)
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
		}
		bool CanRemove()
		{
			return SelectedPosition != null && !SelectedPosition.IsOrganisation;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			//var organisation = SelectedPosition.Parent.Organization;
			//var positionDetailsViewModel = new PositionDetailsViewModel(this, organisation, SelectedPosition.Position.UID);
			//if (DialogService.ShowModalWindow(positionDetailsViewModel))
			//{
			//    var position = positionDetailsViewModel.Position;
			//    bool saveResult = PositionHelper.Save(position);
			//    if (!saveResult)
			//        return;
			//    SelectedPosition.Update(positionDetailsViewModel.ShortPosition);
			//}
		}
		bool CanEdit()
		{
			return SelectedPosition != null && SelectedPosition.Parent != null && !SelectedPosition.IsOrganisation;
		}
	}
}