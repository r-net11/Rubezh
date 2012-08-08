using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using InstructionsModule.Views;

namespace InstructionsModule.ViewModels
{
    public class InstructionsViewModel : ViewPartViewModel, IEditingViewModel
    {
        public InstructionsViewModel()
        {
            AddCommand = new RelayCommand(OnAdd);
            DeleteCommand = new RelayCommand(OnDelete, CanEditRemove);
            DeleteAllCommand = new RelayCommand(OnDeleteAll, CanRemoveAll);
            EditCommand = new RelayCommand(OnEdit, CanEditRemove);
        }

        public void Initialize()
        {
			Instructions = new ObservableCollection<InstructionViewModel>();

            if (FiresecManager.SystemConfiguration.Instructions.IsNotNullOrEmpty())
            {
                foreach (var instruction in FiresecManager.SystemConfiguration.Instructions)
                {
                    if (instruction.InstructionType == InstructionType.Details)
                    {
                        instruction.Devices = new List<System.Guid>(
                            instruction.Devices.Where(deviceGuid => FiresecManager.Devices.Any(x => x.UID == deviceGuid))
                        );
						instruction.Zones = new List<int>(
                            instruction.Zones.Where(zoneNumber => FiresecManager.Zones.Any(x => x.No == zoneNumber))
                        );
                    }
                    Instructions.Add(new InstructionViewModel(instruction));
                }
            }

			SelectedInstruction = Instructions.FirstOrDefault();
        }

		ObservableCollection<InstructionViewModel> _instructions;
		public ObservableCollection<InstructionViewModel> Instructions
		{
			get { return _instructions; }
			set
			{
				_instructions = value;
				OnPropertyChanged("Instructions");
			}
		}

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
            if (DialogService.ShowModalWindow(instructionDetailsViewModel))
            {
                Instructions.Add(new InstructionViewModel(instructionDetailsViewModel.Instruction));
                FiresecManager.SystemConfiguration.Instructions.Add(instructionDetailsViewModel.Instruction);
                ServiceFactory.SaveService.InstructionsChanged = true;
            }
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var instructionDetailsViewModel = new InstructionDetailsViewModel(SelectedInstruction.Instruction);
			if (DialogService.ShowModalWindow(instructionDetailsViewModel))
            {
                SelectedInstruction.Update();
                ServiceFactory.SaveService.InstructionsChanged = true;
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

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            FiresecManager.SystemConfiguration.Instructions.Remove(SelectedInstruction.Instruction);
            Instructions.Remove(SelectedInstruction);
            if (Instructions.IsNotNullOrEmpty())
                SelectedInstruction = Instructions[0];
            ServiceFactory.SaveService.InstructionsChanged = true;
        }

        public RelayCommand DeleteAllCommand { get; private set; }
        void OnDeleteAll()
        {
            SelectedInstruction = null;
            Instructions.Clear();
            FiresecManager.SystemConfiguration.Instructions.Clear();
            ServiceFactory.SaveService.InstructionsChanged = true;
        }

        public override void OnShow()
        {
            ServiceFactory.Layout.ShowMenu(new InstructionsMenuViewModel(this));

			if (InstructionsMenuView.Current != null)
				InstructionsMenuView.Current.AcceptKeyboard = true;
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);

			if (InstructionsMenuView.Current != null)
				InstructionsMenuView.Current.AcceptKeyboard = false;
        }
    }
}