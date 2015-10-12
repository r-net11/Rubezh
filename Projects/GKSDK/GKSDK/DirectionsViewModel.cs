﻿using System.Collections.ObjectModel;
using RubezhClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

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