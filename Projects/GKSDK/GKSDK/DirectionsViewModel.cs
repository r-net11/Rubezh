using System.Collections.ObjectModel;
using RubezhClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI;

namespace GKSDK
{
	public class DirectionsViewModel : BaseViewModel
	{
		public DirectionsViewModel()
		{
			Directions = new ObservableCollection<DirectionViewModel>();
            foreach (var direction in GKManager.Directions)
			{
				var deviceViewModel = new DirectionViewModel(direction);
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
				OnPropertyChanged(()=>SelectedDirection);
			}
		}
	}
}