using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;

namespace PlansModule.ViewModels
{
    public class ChildrensViewModel : RegionViewModel
    {/*
        public ChildrensViewModel()
        {
            AddCommand = new RelayCommand(OnAdd);
            RemoveCommand = new RelayCommand(OnRemove, CanEditRemove);
            EditCommand = new RelayCommand(OnEdit, CanEditRemove);
            Instructions = new ObservableCollection<PlanViewModel>();
        }
        
        public void Initialize()
        {
            if (FiresecManager.SystemConfiguration.Plans != null)
            {
                foreach (var plan in FiresecManager.SystemConfiguration.Plans)
                {
                    PlanViewModel planViewModel = new PlanViewModel(plan);
                    Instructions.Add(instructionViewModel);
                }
            }
        }

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

        public RelayCommand AddCommand { get; private set; }

        void OnAdd()
        {
            InstructionDetailsViewModel instructionDetailsViewModel = new InstructionDetailsViewModel();
            instructionDetailsViewModel.Initialize();
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(instructionDetailsViewModel);
            if (result)
            {
                InstructionViewModel instructionViewModel = new InstructionViewModel(instructionDetailsViewModel.Instruction);
                Instructions.Add(instructionViewModel);
            }
        }

        bool CanEditRemove(object obj)
        {
            return SelectedInstruction != null;
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            FiresecManager.SystemConfiguration.Instructions.Remove(SelectedInstruction.Instruction);
            Instructions.Remove(SelectedInstruction);
        }

        public RelayCommand EditCommand { get; private set; }

        void OnEdit()
        {
            InstructionDetailsViewModel instructionDetailsViewModel = new InstructionDetailsViewModel();
            instructionDetailsViewModel.Initialize(SelectedInstruction.Instruction);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(instructionDetailsViewModel);
            if (result)
            {
                SelectedInstruction.Update();
            }
        }

        public override void OnShow()
        {
            InstructionsMenuViewModel instructionsMenuViewModel = new InstructionsMenuViewModel(AddCommand, EditCommand, RemoveCommand);
            ServiceFactory.Layout.ShowMenu(instructionsMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }*/
    }
}
