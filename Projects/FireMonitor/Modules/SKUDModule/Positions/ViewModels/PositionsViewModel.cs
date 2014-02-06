using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class PositionsViewModel : ViewPartViewModel
	{
		public PositionsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);

			Positions = new ObservableCollection<PositionViewModel>();
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
			var positionDetailsViewModel = new PositionDetailsViewModel();
			if (DialogService.ShowModalWindow(positionDetailsViewModel))
			{
				var position = positionDetailsViewModel.Position;
				var positionViewModel = new PositionViewModel(position);
				Positions.Add(positionViewModel);
				SelectedPosition = positionViewModel;
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			Positions.Remove(SelectedPosition);
		}
		bool CanRemove()
		{
			return SelectedPosition != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var positionDetailsViewModel = new PositionDetailsViewModel(SelectedPosition.Position);
			if (DialogService.ShowModalWindow(positionDetailsViewModel))
			{
				SelectedPosition.Update(positionDetailsViewModel.Position);
			}
		}
		bool CanEdit()
		{
			return SelectedPosition != null;
		}
	}
}