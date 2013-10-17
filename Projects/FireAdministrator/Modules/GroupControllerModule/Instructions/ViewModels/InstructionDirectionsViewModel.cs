using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
    public class InstructionDirectionsViewModel : SaveCancelDialogViewModel
    {
        public InstructionDirectionsViewModel(List<Guid> instructionDirectionsList)
        {
            AddOneCommand = new RelayCommand(OnAddOne, CanAddOne);
            RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemoveOne);
            AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);

            Title = "Выбор направлений";

            InstructionDirectionsList = new List<Guid>(instructionDirectionsList);
            InstructionDirections = new ObservableCollection<DirectionViewModel>();
            AvailableDirections = new ObservableCollection<DirectionViewModel>();

            InitializeDirections();
            if (InstructionDirections.IsNotNullOrEmpty())
                SelectedInstructionDirection = InstructionDirections[0];
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
                        InstructionDirections.Add(directionViewModel);
                    else
                        AvailableDirections.Add(directionViewModel);
                }
                else
                {
                    AvailableDirections.Add(directionViewModel);
                }
            }

            if (InstructionDirections.IsNotNullOrEmpty())
                SelectedInstructionDirection = InstructionDirections[0];
            if (AvailableDirections.IsNotNullOrEmpty())
                SelectedAvailableDirection = AvailableDirections[0];
        }

        public List<Guid> InstructionDirectionsList { get; set; }
        public ObservableCollection<DirectionViewModel> AvailableDirections { get; set; }
        public ObservableCollection<DirectionViewModel> InstructionDirections { get; set; }

        DirectionViewModel _selectedAvailableDirection;
        public DirectionViewModel SelectedAvailableDirection
        {
            get { return _selectedAvailableDirection; }
            set
            {
                _selectedAvailableDirection = value;
                OnPropertyChanged("SelectedAvailableDirection");
            }
        }

        DirectionViewModel _selectedInstructionDirection;
        public DirectionViewModel SelectedInstructionDirection
        {
            get { return _selectedInstructionDirection; }
            set
            {
                _selectedInstructionDirection = value;
                OnPropertyChanged("SelectedInstructionDirection");
            }
        }

        public bool CanAddOne()
        {
            return SelectedAvailableDirection != null;
        }

        public bool CanAddAll()
        {
            return AvailableDirections.IsNotNullOrEmpty();
        }

        public bool CanRemoveOne()
        {
            return SelectedInstructionDirection != null;
        }

        public bool CanRemoveAll()
        {
            return InstructionDirections.IsNotNullOrEmpty();
        }

        public RelayCommand AddOneCommand { get; private set; }
        void OnAddOne()
        {
            InstructionDirections.Add(SelectedAvailableDirection);
            AvailableDirections.Remove(SelectedAvailableDirection);
            if (AvailableDirections.IsNotNullOrEmpty())
                SelectedAvailableDirection = AvailableDirections[0];
            if (InstructionDirections.IsNotNullOrEmpty())
                SelectedInstructionDirection = InstructionDirections[0];
        }

        public RelayCommand AddAllCommand { get; private set; }
        void OnAddAll()
        {
            foreach (var availableZone in AvailableDirections)
            {
                InstructionDirections.Add(availableZone);
            }

            AvailableDirections.Clear();
            SelectedAvailableDirection = null;
            if (InstructionDirections.IsNotNullOrEmpty())
                SelectedInstructionDirection = InstructionDirections[0];
        }

        public RelayCommand RemoveAllCommand { get; private set; }
        void OnRemoveAll()
        {
            foreach (var instructionZone in InstructionDirections)
            {
                AvailableDirections.Add(instructionZone);
            }

            InstructionDirections.Clear();
            SelectedInstructionDirection = null;
            if (AvailableDirections.IsNotNullOrEmpty())
                SelectedAvailableDirection = AvailableDirections[0];
        }

        public RelayCommand RemoveOneCommand { get; private set; }
        void OnRemoveOne()
        {
            AvailableDirections.Add(SelectedInstructionDirection);
            InstructionDirections.Remove(SelectedInstructionDirection);
            if (AvailableDirections.IsNotNullOrEmpty())
                SelectedAvailableDirection = AvailableDirections[0];
            if (InstructionDirections.IsNotNullOrEmpty())
                SelectedInstructionDirection = InstructionDirections[0];
        }

        protected override bool Save()
        {
            InstructionDirectionsList = new List<Guid>(from direction in InstructionDirections select direction.Direction.UID);
            return base.Save();
        }
    }
}