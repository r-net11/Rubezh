using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class InstructionDirectionsViewModel : SaveCancelDialogViewModel
    {
        public InstructionDirectionsViewModel(List<Guid> instructionDirectionsList)
        {
            AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
            AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);

            Title = "Выбор направлений";

            InstructionDirectionsList = new List<Guid>(instructionDirectionsList);
            TargetDirections = new ObservableCollection<DirectionViewModel>();
            SourceDirections = new ObservableCollection<DirectionViewModel>();

            InitializeDirections();
            if (TargetDirections.IsNotNullOrEmpty())
                SelectedTargetDirection = TargetDirections[0];
        }

        void InitializeDirections()
        {
            foreach (var direction in XManager.Directions)
            {
                var directionViewModel = new DirectionViewModel(direction);
                if (InstructionDirectionsList.IsNotNullOrEmpty())
                {
                    var instructionZone = InstructionDirectionsList.FirstOrDefault(x => x == directionViewModel.Direction.UID);
                    if (instructionZone != Guid.Empty)
                        TargetDirections.Add(directionViewModel);
                    else
                        SourceDirections.Add(directionViewModel);
                }
                else
                {
                    SourceDirections.Add(directionViewModel);
                }
            }

            if (TargetDirections.IsNotNullOrEmpty())
                SelectedTargetDirection = TargetDirections[0];
            if (SourceDirections.IsNotNullOrEmpty())
                SelectedSourceDirection = SourceDirections[0];
        }

        public List<Guid> InstructionDirectionsList { get; set; }
        public ObservableCollection<DirectionViewModel> SourceDirections { get; set; }
        public ObservableCollection<DirectionViewModel> TargetDirections { get; set; }

        DirectionViewModel _selectedAvailableDirection;
        public DirectionViewModel SelectedSourceDirection
        {
            get { return _selectedAvailableDirection; }
            set
            {
                _selectedAvailableDirection = value;
				OnPropertyChanged("SelectedSourceDirection");
            }
        }

        DirectionViewModel _selectedInstructionDirection;
        public DirectionViewModel SelectedTargetDirection
        {
            get { return _selectedInstructionDirection; }
            set
            {
                _selectedInstructionDirection = value;
				OnPropertyChanged("SelectedTargetDirection");
            }
        }

        public bool CanAddAll()
        {
            return SourceDirections.IsNotNullOrEmpty();
        }

        public bool CanRemoveAll()
        {
            return TargetDirections.IsNotNullOrEmpty();
        }

        public RelayCommand<object> AddCommand { get; private set; }
		public IList SelectedSourceDirections;
		void OnAdd(object parameter)
		{
			var index = SourceDirections.IndexOf(SelectedSourceDirection);

			SelectedSourceDirections = (IList)parameter;
			var SourceDirectionViewModels = new List<DirectionViewModel>();
			foreach (var SourceDirection in SelectedSourceDirections)
			{
				var SourceDirectionViewModel = SourceDirection as DirectionViewModel;
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
		public bool CanAdd(object parameter)
		{
			return SelectedSourceDirection != null;
		}

		public RelayCommand<object> RemoveCommand { get; private set; }
		public IList SelectedTargetDirections;
		void OnRemove(object parameter)
		{
			var index = TargetDirections.IndexOf(SelectedTargetDirection);

			SelectedTargetDirections = (IList)parameter;
			var TargetDirectionViewModels = new List<DirectionViewModel>();
			foreach (var TargetDirection in SelectedTargetDirections)
			{
				var TargetDirectionViewModel = TargetDirection as DirectionViewModel;
				if (TargetDirectionViewModel != null)
					TargetDirectionViewModels.Add(TargetDirectionViewModel);
			}
			foreach (var TargetDirectionViewModel in TargetDirectionViewModels)
			{
				SourceDirections.Add(TargetDirectionViewModel);
				TargetDirections.Remove(TargetDirectionViewModel);
			}
			SelectedSourceDirection = SourceDirections.LastOrDefault();
			OnPropertyChanged("TargetDirections");

			index = Math.Min(index, TargetDirections.Count - 1);
			if (index > -1)
				SelectedTargetDirection = TargetDirections[index];
		}
		public bool CanRemove(object parameter)
		{
			return SelectedTargetDirection != null;
		}

        public RelayCommand AddAllCommand { get; private set; }
        void OnAddAll()
        {
            foreach (var availableZone in SourceDirections)
            {
                TargetDirections.Add(availableZone);
            }

            SourceDirections.Clear();
            SelectedSourceDirection = null;
            if (TargetDirections.IsNotNullOrEmpty())
                SelectedTargetDirection = TargetDirections[0];
        }

        public RelayCommand RemoveAllCommand { get; private set; }
        void OnRemoveAll()
        {
            foreach (var instructionZone in TargetDirections)
            {
                SourceDirections.Add(instructionZone);
            }

            TargetDirections.Clear();
            SelectedTargetDirection = null;
            if (SourceDirections.IsNotNullOrEmpty())
                SelectedSourceDirection = SourceDirections[0];
        }

		protected override bool Save()
        {
            InstructionDirectionsList = new List<Guid>(from direction in TargetDirections select direction.Direction.UID);
            return base.Save();
        }
    }
}