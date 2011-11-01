using System.Collections.ObjectModel;
using Common;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

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
        }

        public void Initialize()
        {
            if (FiresecManager.SystemConfiguration.Instructions.IsNotNullOrEmpty())
            {
                foreach (var instruction in FiresecManager.SystemConfiguration.Instructions)
                {
                    Instructions.Add(new InstructionViewModel(instruction));
                }
            }

            if (Instructions.Count > 0)
                SelectedInstruction = Instructions[0];
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
            var instructionDetailsViewModel = new InstructionDetailsViewModel();
            instructionDetailsViewModel.Initialize();
            if (ServiceFactory.UserDialogs.ShowModalWindow(instructionDetailsViewModel))
            {
                Instructions.Add(new InstructionViewModel(instructionDetailsViewModel.Instruction));
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
                SelectedInstruction = Instructions[0];
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
            if (ServiceFactory.UserDialogs.ShowModalWindow(instructionDetailsViewModel))
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