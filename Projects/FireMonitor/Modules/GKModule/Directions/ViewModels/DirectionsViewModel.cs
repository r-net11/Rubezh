using System;
using System.Collections.Generic;
using System.Linq;
using RubezhClient;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class DirectionsViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public void Initialize()
		{
			Directions = new List<DirectionViewModel>();
			foreach (var direction in GKManager.Directions.OrderBy(x => x.No))
			{
				var directionViewModel = new DirectionViewModel(direction);
				Directions.Add(directionViewModel);
			}
			SelectedDirection = Directions.FirstOrDefault();
		}

		List<DirectionViewModel> _direction;
		public List<DirectionViewModel> Directions
		{
			get { return _direction; }
			set
			{
				_direction = value;
				OnPropertyChanged(() => Directions);
			}
		}

		DirectionViewModel _selectedDirection;
		public DirectionViewModel SelectedDirection
		{
			get { return _selectedDirection; }
			set
			{
				_selectedDirection = value;
				OnPropertyChanged(() => SelectedDirection);
			}
		}

		public void Select(Guid directionUID)
		{
			if (directionUID != Guid.Empty)
			{
				SelectedDirection = Directions.FirstOrDefault(x => x.Direction.UID == directionUID);
			}
		}
	}
}