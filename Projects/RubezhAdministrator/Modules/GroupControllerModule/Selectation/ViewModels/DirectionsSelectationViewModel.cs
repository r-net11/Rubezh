using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using Common;
using Infrastructure;
using GKModule.Events;

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
			CreateNewCommand = new RelayCommand(OnCreateNew);

			Directions = directions;
			TargetDirections = new SortableObservableCollection<GKDirection>();
			SourceDirections = new SortableObservableCollection<GKDirection>();

			foreach (var direction in GKManager.Directions)
			{
				if (Directions.Contains(direction))
					TargetDirections.Add(direction);
				else
					SourceDirections.Add(direction);
			}
			TargetDirections.Sort(x => x.No);
			SourceDirections.Sort(x => x.No);

			SelectedTargetDirection = TargetDirections.FirstOrDefault();
			SelectedSourceDirection = SourceDirections.FirstOrDefault();
		}

		public SortableObservableCollection<GKDirection> SourceDirections { get; private set; }

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

		public SortableObservableCollection<GKDirection> TargetDirections { get; private set; }

		GKDirection _selectedTargetDirection;
		public GKDirection SelectedTargetDirection
		{
			get { return _selectedTargetDirection; }
			set
			{
				_selectedTargetDirection = value;
				OnPropertyChanged(() => SelectedTargetDirection);
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
			TargetDirections.Sort(x => x.No);
			SelectedTargetDirection = TargetDirections.LastOrDefault();
			OnPropertyChanged(() => SourceDirections);

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
			SourceDirections.Sort(x => x.No);
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

		public RelayCommand AddAllCommand { get; private set; }
		void OnAddAll()
		{
			foreach (var zoneViewModel in SourceDirections)
			{
				TargetDirections.Add(zoneViewModel);
			}
			TargetDirections.Sort(x => x.No);
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
			SourceDirections.Sort(x => x.No);
			TargetDirections.Clear();
			SelectedSourceDirection = SourceDirections.FirstOrDefault();
		}
		public RelayCommand CreateNewCommand { get; private set; }
		void OnCreateNew()
		{
			var createGKDirectionEventArg = new CreateGKDirectionEventArg();
			ServiceFactory.Events.GetEvent<CreateGKDirectionEvent>().Publish(createGKDirectionEventArg);
			if (createGKDirectionEventArg.Direction != null)
			{
				SourceDirections.Add(createGKDirectionEventArg.Direction);
			}
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