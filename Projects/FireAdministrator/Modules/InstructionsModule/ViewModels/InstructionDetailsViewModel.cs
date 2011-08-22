using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace InstructionsModule.ViewModels
{
    public class InstructionDetailsViewModel : DialogContent
    {
        public InstructionDetailsViewModel()
        {
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
            SetZoneCommand = new RelayCommand(OnSetZoneCommand);
        }

        bool _isNew;
        public Instruction Instruction { get; private set; }
        public InstructionZonesViewModel InstructionZonesViewModel { get; private set; }

        public void Initialize()
        {
            _isNew = false;
            Title = "Новая инструкция";
            Instruction = new Instruction();
            InstructionZonesViewModel = new InstructionZonesViewModel();
        }

        public void Initialize(Instruction instruction)
        {
            _isNew = false;
            Title = "Редактирование инструкции";
            Instruction = instruction;
            Name = instruction.Name;
            Text = instruction.Text;
            StateType = instruction.StateType;
            InstructionZonesViewModel = new InstructionZonesViewModel();
            InstructionZonesViewModel.Inicialize(Instruction);
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public List<StateType> AvailableStates
        {
            get { return new List<StateType>(Enum.GetValues(typeof(StateType)).OfType<StateType>());  }
        }

        StateType _stateType;
        public StateType StateType
        {
            get { return _stateType; }
            set
            {
                _stateType = value;
                OnPropertyChanged("StateType");
            }
        }

        string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        void Save()
        {
            Instruction.Name = Name;
            Instruction.Text = Text;
            Instruction.StateType = StateType;
            if (_isNew)
            {
                FiresecManager.SystemConfiguration.Instructions.Add(Instruction);
            }
        }

        public RelayCommand SetZoneCommand { get; private set; }
        void OnSetZoneCommand()
        {
            Instruction.Name = Name;
            Instruction.Text = Text;
            Instruction.StateType = StateType;
            InstructionZonesViewModel.Inicialize(Instruction);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(InstructionZonesViewModel);
            if (result)
            {
                
            }
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            Save();
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}
