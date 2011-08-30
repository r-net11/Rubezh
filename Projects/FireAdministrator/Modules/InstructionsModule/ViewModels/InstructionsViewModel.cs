using System.Collections.ObjectModel;
using FiresecClient;
using Common;
using System.Linq;
using Infrastructure;
using Infrastructure.Common;
using System.Collections.Generic;
using FiresecAPI.Models;
using System;
using System.ComponentModel;
using System.Windows.Data;

namespace InstructionsModule.ViewModels
{
    public class InstructionsViewModel : RegionViewModel
    {
        public InstructionsViewModel()
        {
            AddCommand = new RelayCommand(OnAdd);
            RemoveCommand = new RelayCommand(OnRemove, CanEditRemove);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);
            EditCommand = new RelayCommand(OnEdit, CanEditRemove);

            Instructions = new ObservableCollection<InstructionViewModel>();
            _filterView = CollectionViewSource.GetDefaultView(this.Instructions);
            _filterView.Filter = CustomerFilter;
            StateTypeFilter = "All";
            InstructionTypeFilter = "All";
        }

        public void Initialize()
        {
            if (FiresecManager.SystemConfiguration.Instructions.IsNotNullOrEmpty())
            {
                foreach (var instruction in FiresecManager.SystemConfiguration.Instructions)
                {
                    var instructionViewModel = new InstructionViewModel(instruction);
                    Instructions.Add(instructionViewModel);
                }
            }
        }

        public ICollectionView _filterView;

        public ObservableCollection<InstructionViewModel> Instructions { get; set; }

        InstructionViewModel _selectedInstruction;
        public InstructionViewModel SelectedInstruction
        {
            get { return _selectedInstruction; }
            set
            {
                _selectedInstruction = value;
                OnPropertyChanged("SelectedInstruction");
            }
        }

        public List<string> StateTypeFilterList
        {
            get
            {
                var filterList = new List<string>() { "All" };
                foreach (var stateTypeName in Enum.GetNames(typeof(StateType)))
                {
                    filterList.Add(stateTypeName);
                }
                return filterList;
            }
        }
        public List<string> InstructionTypeFilterList
        {
            get
            {
                var filterList = new List<string>() { "All" };
                foreach (var instructionTypeName in Enum.GetNames(typeof(InstructionType)))
                {
                    filterList.Add(instructionTypeName);
                }
                return filterList;
            }
        }

        string _stateTypeFilter;
        public string StateTypeFilter
        {
            get { return _stateTypeFilter; }
            set
            {
                _stateTypeFilter = value;
                OnPropertyChanged("StateTypeFilter");
                _filterView.Refresh();
            }
        }
        string _instructionTypeFilter;
        public string InstructionTypeFilter
        {
            get { return _instructionTypeFilter; }
            set
            {
                _instructionTypeFilter = value;
                OnPropertyChanged("InstructionTypeFilter");
                _filterView.Refresh();
            }
        }

        public StateType? StateType
        {
            get
            {
                if (StateTypeFilter == "All")
                {
                    return null;
                }
                else
                {
                    StateType stateType = new StateType();
                    foreach (StateType stType in Enum.GetValues(typeof(StateType)))
                    {
                        if ((Enum.GetName(typeof(StateType), stType) == StateTypeFilter))
                        {
                            stateType = stType;
                        }
                    }
                    return stateType;
                }
            }
        }
        public InstructionType? InstructionType
        {
            get
            {
                if (InstructionTypeFilter == "All")
                {
                    return null;
                }
                else
                {
                    InstructionType instructionType = new InstructionType();
                    foreach (InstructionType instrType in Enum.GetValues(typeof(InstructionType)))
                    {
                        if ((Enum.GetName(typeof(InstructionType), instrType) == InstructionTypeFilter))
                        {
                            instructionType = instrType;
                        }
                    }
                    return instructionType;
                }
            }
        }

        bool CustomerFilter(object item)
        {
            InstructionViewModel instructionViewModel = (InstructionViewModel)item;
            if ((StateType == null) && (InstructionType == null))
            {
                return true;
            }
            else
            {
                if (StateType == null)
                {
                    return (instructionViewModel.InstructionType == InstructionType);
                }
                else
                {
                    if (InstructionType == null)
                    {
                        return (instructionViewModel.StateType == StateType);
                    }
                    else
                    {
                        return ((instructionViewModel.InstructionType == InstructionType) &&
                            (instructionViewModel.StateType == StateType));
                    }
                }
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var instructionDetailsViewModel = new InstructionDetailsViewModel();
            instructionDetailsViewModel.Initialize();
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(instructionDetailsViewModel);
            if (result)
            {
                var instructionViewModel = new InstructionViewModel(instructionDetailsViewModel.Instruction);
                Instructions.Add(instructionViewModel);
                InstructionsModule.HasChanges = true;
            }
        }

        bool CanEditRemove()
        {
            return SelectedInstruction != null;
        }

        bool CanRemoveAll()
        {
            return (Instructions.IsNotNullOrEmpty());
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            FiresecManager.SystemConfiguration.Instructions.Remove(SelectedInstruction.Instruction);
            Instructions.Remove(SelectedInstruction);
            if (Instructions.IsNotNullOrEmpty())
            {
                SelectedInstruction = Instructions[0];
            }
            InstructionsModule.HasChanges = true;
        }

        public RelayCommand RemoveAllCommand { get; private set; }
        void OnRemoveAll()
        {
            SelectedInstruction = null;
            Instructions.Clear();
            FiresecManager.SystemConfiguration.Instructions.Clear();
            InstructionsModule.HasChanges = true;
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var instructionDetailsViewModel = new InstructionDetailsViewModel();
            instructionDetailsViewModel.Initialize(SelectedInstruction.Instruction);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(instructionDetailsViewModel);
            if (result)
            {
                SelectedInstruction.Update();
                InstructionsModule.HasChanges = true;
            }
        }

        public override void OnShow()
        {
            var instructionsMenuViewModel = new InstructionsMenuViewModel(AddCommand, EditCommand, RemoveCommand, RemoveAllCommand);
            ServiceFactory.Layout.ShowMenu(instructionsMenuViewModel);

        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}
