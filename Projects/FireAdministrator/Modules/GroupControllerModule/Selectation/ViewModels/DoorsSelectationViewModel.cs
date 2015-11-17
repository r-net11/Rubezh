﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class DoorsSelectationViewModel : SaveCancelDialogViewModel
	{
		public List<GKDoor> Doors { get; private set; }

		public DoorsSelectationViewModel(List<GKDoor> doors)
		{
			Title = "Выбор точек доступа";
			AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
			AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
			RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);

			Doors = doors;
			TargetDoors = new ObservableCollection<GKDoor>();
			SourceDoors = new ObservableCollection<GKDoor>();

			foreach (var door in GKManager.DeviceConfiguration.Doors)
			{
				if (Doors.Contains(door))
					TargetDoors.Add(door);
				else
					SourceDoors.Add(door);
			}

			SelectedTargetDoor = TargetDoors.FirstOrDefault();
			SelectedSourceDoor = SourceDoors.FirstOrDefault();
		}

		public ObservableCollection<GKDoor> SourceDoors { get; private set; }

		GKDoor _selectedSourceDoor;
		public GKDoor SelectedSourceDoor
		{
			get { return _selectedSourceDoor; }
			set
			{
				_selectedSourceDoor = value;
				OnPropertyChanged(() => SelectedSourceDoor);
			}
		}

		public ObservableCollection<GKDoor> TargetDoors { get; private set; }

		GKDoor _selectedTargetDoor;
		public GKDoor SelectedTargetDoor
		{
			get { return _selectedTargetDoor; }
			set
			{
				_selectedTargetDoor = value;
				OnPropertyChanged(() => SelectedTargetDoor);
			}
		}

		public RelayCommand<object> AddCommand { get; private set; }
		public IList SelectedSourceDoors;
		void OnAdd(object parameter)
		{
			var index = SourceDoors.IndexOf(SelectedSourceDoor);

			SelectedSourceDoors = (IList)parameter;
			var doorViewModels = new List<GKDoor>();
			foreach (var selectedDoor in SelectedSourceDoors)
			{
				var doorViewModel = selectedDoor as GKDoor;
				if (doorViewModel != null)
					doorViewModels.Add(doorViewModel);
			}
			foreach (var doorViewModel in doorViewModels)
			{
				TargetDoors.Add(doorViewModel);
				SourceDoors.Remove(doorViewModel);
			}
			SelectedTargetDoor = TargetDoors.LastOrDefault();
			OnPropertyChanged(() => SourceDoors);

			index = Math.Min(index, SourceDoors.Count - 1);
			if (index > -1)
				SelectedSourceDoor = SourceDoors[index];
		}

		public RelayCommand<object> RemoveCommand { get; private set; }
		public IList SelectedTargetDoors;
		void OnRemove(object parameter)
		{
			var index = TargetDoors.IndexOf(SelectedTargetDoor);

			SelectedTargetDoors = (IList)parameter;
			var doorViewModels = new List<GKDoor>();
			foreach (var selectedDoor in SelectedTargetDoors)
			{
				var doorViewModel = selectedDoor as GKDoor;
				if (doorViewModel != null)
					doorViewModels.Add(doorViewModel);
			}
			foreach (var doorViewModel in doorViewModels)
			{
				SourceDoors.Add(doorViewModel);
				TargetDoors.Remove(doorViewModel);
			}
			SelectedSourceDoor = SourceDoors.LastOrDefault();
			OnPropertyChanged(() => TargetDoors);

			index = Math.Min(index, TargetDoors.Count - 1);
			if (index > -1)
				SelectedTargetDoor = TargetDoors[index];
		}

		public RelayCommand AddAllCommand { get; private set; }
		void OnAddAll()
		{
			foreach (var doorViewModel in SourceDoors)
			{
				TargetDoors.Add(doorViewModel);
			}
			SourceDoors.Clear();
			SelectedTargetDoor = TargetDoors.FirstOrDefault();
		}

		public RelayCommand RemoveAllCommand { get; private set; }
		void OnRemoveAll()
		{
			foreach (var doorViewModel in TargetDoors)
			{
				SourceDoors.Add(doorViewModel);
			}
			TargetDoors.Clear();
			SelectedSourceDoor = SourceDoors.FirstOrDefault();
		}

		public bool CanAdd(object parameter)
		{
			return SelectedSourceDoor != null;
		}

		public bool CanRemove(object parameter)
		{
			return SelectedTargetDoor != null;
		}

		public bool CanAddAll()
		{
			return (SourceDoors.Count > 0);
		}

		public bool CanRemoveAll()
		{
			return (TargetDoors.Count > 0);
		}

		protected override bool Save()
		{
			Doors = new List<GKDoor>(TargetDoors);
			return base.Save();
		}
	}
}