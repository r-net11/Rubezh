using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationPositionsViewModel : BaseViewModel
	{
		public Organisation Organization { get; private set; }

		public OrganisationPositionsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
		}

		public void Initialize(Organisation organization, IEnumerable<ShortPosition> positions)
		{
			Organization = organization;

			Positions = new ObservableCollection<PositionViewModel>();
			foreach (var position in positions)
			{
				var positionViewModel = new PositionViewModel(position);
				Positions.Add(positionViewModel);
			}
			SelectedPosition = Positions.FirstOrDefault();
		}

		ObservableCollection<PositionViewModel> _positions;
		public ObservableCollection<PositionViewModel> Positions
		{
			get { return _positions; }
			set
			{
				_positions = value;
				OnPropertyChanged("Positions");
			}
		}

		PositionViewModel _selectedPosition;
		public PositionViewModel SelectedPosition
		{
			get { return _selectedPosition; }
			set
			{
				_selectedPosition = value;
				OnPropertyChanged("SelectedPosition");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var positionDetailsViewModel = new PositionDetailsViewModel(this, Organization);
			if (DialogService.ShowModalWindow(positionDetailsViewModel))
			{
				var positionViewModel = new PositionViewModel(positionDetailsViewModel.ShortPosition);
				Positions.Add(positionViewModel);
				SelectedPosition = positionViewModel;
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var index = Positions.IndexOf(SelectedPosition);
			var position = SelectedPosition.Position;
			bool removeResult = PositionHelper.MarkDeleted(position.UID);
			if (!removeResult)
				return;
			Positions.Remove(SelectedPosition);
			index = Math.Min(index, Positions.Count - 1);
			if (index > -1)
				SelectedPosition = Positions[index];
		}
		bool CanRemove()
		{
			return SelectedPosition != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var positionDetailsViewModel = new PositionDetailsViewModel(this, Organization, SelectedPosition.Position.UID);
			if (DialogService.ShowModalWindow(positionDetailsViewModel))
			{
				SelectedPosition.Update(positionDetailsViewModel.ShortPosition);
			}
		}
		bool CanEdit()
		{
			return SelectedPosition != null;
		}
	}
}