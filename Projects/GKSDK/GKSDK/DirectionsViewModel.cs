using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKSDK
{
	public class DirectionsViewModel : BaseViewModel
	{
		public DirectionsViewModel()
		{
			SetZoneGuardCommand = new RelayCommand(OnSetZoneGuard, CanSetZoneGuard);
			UnSetZoneGuardCommand = new RelayCommand(OnUnSetZoneGuard, CanUnSetZoneGuard);

			Directions = new ObservableCollection<DirectionViewModel>();
			foreach (var direction in XManager.Directions)
			{
				var deviceViewModel = new DirectionViewModel(direction.DirectionState);
				Directions.Add(deviceViewModel);
			}
		}

		public ObservableCollection<DirectionViewModel> Directions { get; set; }

		DirectionViewModel _selectedDirection;
		public DirectionViewModel SelectedDirection
		{
			get { return _selectedDirection; }
			set
			{
				_selectedDirection = value;
				OnPropertyChanged("SelectedDirection");
			}
		}

		bool CanSetZoneGuard()
		{
			return SelectedDirection != null;
		}
		public RelayCommand SetZoneGuardCommand { get; private set; }
		void OnSetZoneGuard()
		{
		}

		bool CanUnSetZoneGuard()
		{
			return SelectedDirection != null;
		}
		public RelayCommand UnSetZoneGuardCommand { get; private set; }
		void OnUnSetZoneGuard()
		{
		}
	}
}