using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;

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