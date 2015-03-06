﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class DirectionsSelectationViewModel : SaveCancelDialogViewModel
	{
		public List<GKDirection> Directions { get; private set; }

		public DirectionsSelectationViewModel(List<GKDirection> directions)
		{
			Title = "Выбор направлений";
			AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
			AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
			RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);

			Directions = directions;
			TargetDirections = new ObservableCollection<GKDirection>();
			SourceDirections = new ObservableCollection<GKDirection>();

			foreach (var direction in GKManager.Directions)
			{
				if (Directions.Contains(direction))
					TargetDirections.Add(direction);
				else
					SourceDirections.Add(direction);
			}

			SelectedTargetDirection = TargetDirections.FirstOrDefault();
			SelectedSourceDirection = SourceDirections.FirstOrDefault();
		}

		public ObservableCollection<GKDirection> SourceDirections { get; private set; }

		GKDirection _selectedSourceDirection;
		public GKDirection SelectedSourceDirection
		{
			get { return _selectedSourceDirection; }
			set
			{
				_selectedSourceDirection = value;
				OnPropertyChanged(() => SelectedSourceDirection);
			}
		}

		public ObservableCollection<GKDirection> TargetDirections { get; private set; }

		GKDirection _selectedTargetDirection;
		public GKDirection SelectedTargetDirection
		{
			get { return _selectedTargetDirection; }
			set
			{
				_selectedTargetDirection = value;
				OnPropertyChanged("SelectedTargetDirection");
			}
		}

		public RelayCommand<object> AddCommand { get; private set; }
		public IList SelectedSourceDirections;
		void OnAdd(object parameter)
		{
			var index = SourceDirections.IndexOf(SelectedSourceDirection);

			SelectedSourceDirections = (IList)parameter;
			var SourceDirectionViewModels = new List<GKDirection>();
			foreach (var SourceDirection in SelectedSourceDirections)
			{
				var SourceDirectionViewModel = SourceDirection as GKDirection;
				if (SourceDirectionViewModel != null)
					SourceDirectionViewModels.Add(SourceDirectionViewModel);

			}
			foreach (var SourceDirectionViewModel in SourceDirectionViewModels)
			{
				TargetDirections.Add(SourceDirectionViewModel);
				SourceDirections.Remove(SourceDirectionViewModel);
			}
			SelectedTargetDirection = TargetDirections.LastOrDefault();
			OnPropertyChanged("SourceDirections");

			index = Math.Min(index, SourceDirections.Count - 1);
			if (index > -1)
				SelectedSourceDirection = SourceDirections[index];
		}

		public RelayCommand<object> RemoveCommand { get; private set; }
		public IList SelectedTargetDirections;
		void OnRemove(object parameter)
		{
			var index = TargetDirections.IndexOf(SelectedTargetDirection);

			SelectedTargetDirections = (IList)parameter;
			var TargetDirectionViewModels = new List<GKDirection>();
			foreach (var TargetDirection in SelectedTargetDirections)
			{
				var TargetDirectionViewModel = TargetDirection as GKDirection;
				if (TargetDirectionViewModel != null)
					TargetDirectionViewModels.Add(TargetDirectionViewModel);
			}
			foreach (var TargetDirectionViewModel in TargetDirectionViewModels)
			{
				SourceDirections.Add(TargetDirectionViewModel);
				TargetDirections.Remove(TargetDirectionViewModel);
			}
			SelectedSourceDirection = SourceDirections.LastOrDefault();
			OnPropertyChanged(() => TargetDirections);

			index = Math.Min(index, TargetDirections.Count - 1);
			if (index > -1)
				SelectedTargetDirection = TargetDirections[index];
		}

		public bool CanAdd(object parameter)
		{
			return SelectedSourceDirection != null;
		}

		public bool CanRemove(object parameter)
		{
			return SelectedTargetDirection != null;
		}

		public RelayCommand AddOneCommand { get; private set; }
		void OnAddOne()
		{
			TargetDirections.Add(SelectedSourceDirection);
			SelectedTargetDirection = SelectedSourceDirection;
			SourceDirections.Remove(SelectedSourceDirection);
			SelectedSourceDirection = SourceDirections.FirstOrDefault();
		}

		public RelayCommand RemoveOneCommand { get; private set; }
		void OnRemoveOne()
		{
			SourceDirections.Add(SelectedTargetDirection);
			SelectedSourceDirection = SelectedTargetDirection;
			TargetDirections.Remove(SelectedTargetDirection);
			SelectedTargetDirection = TargetDirections.FirstOrDefault();
		}

		public RelayCommand AddAllCommand { get; private set; }
		void OnAddAll()
		{
			foreach (var zoneViewModel in SourceDirections)
			{
				TargetDirections.Add(zoneViewModel);
			}
			SourceDirections.Clear();
			SelectedTargetDirection = TargetDirections.FirstOrDefault();
		}

		public RelayCommand RemoveAllCommand { get; private set; }
		void OnRemoveAll()
		{
			foreach (var zoneViewModel in TargetDirections)
			{
				SourceDirections.Add(zoneViewModel);
			}
			TargetDirections.Clear();
			SelectedSourceDirection = SourceDirections.FirstOrDefault();
		}
		public bool CanAddAll()
		{
			return (SourceDirections.Count > 0);
		}

		public bool CanRemoveAll()
		{
			return (TargetDirections.Count > 0);
		}
		protected override bool Save()
		{
			Directions = new List<GKDirection>(TargetDirections);
			return base.Save();
		}
	}
}