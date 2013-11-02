using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class OPCDirectionsViewModel : ViewPartViewModel
	{
		public void Initialize()
		{
			Directions = new ObservableCollection<DirectionViewModel>();
			foreach (var direction in XManager.Directions)
			{
				var zoneViewModel = new DirectionViewModel(direction);
				Directions.Add(zoneViewModel);
			}
			SelectedDirection = Directions.FirstOrDefault();
		}

		ObservableCollection<DirectionViewModel> _directions;
		public ObservableCollection<DirectionViewModel> Directions
		{
			get { return _directions; }
			set
			{
				_directions = value;
				OnPropertyChanged("Directions");
			}
		}

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
	}
}