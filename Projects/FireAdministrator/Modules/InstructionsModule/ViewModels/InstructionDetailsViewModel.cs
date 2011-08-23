using System.Collections.Generic;
using FiresecAPI.Models;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using System;

namespace InstructionsModule.ViewModels
{
    public class InstructionDetailsViewModel : DialogContent
    {
        public InstructionDetailsViewModel()
        {
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
            SelectZoneCommand = new RelayCommand(OnSelectZoneCommand, CanSelectZone);
            SelectDeviceCommand = new RelayCommand(OnSelectDeviceCommand, CanSelectDevice);
        }

        bool _isNew;
        public Instruction Instruction { get; private set; }

        public void Initialize()
        {
            _isNew = false;
            Title = "Новая инструкция";
            Instruction = new Instruction();
        }

        public void Initialize(Instruction instruction)
        {
            _isNew = false;
            Title = "Редактирование инструкции";
            Instruction = instruction;
            Name = instruction.Name;
            Text = instruction.Text;
            StateType = instruction.StateType;
            InstructionType = instruction.InstructionType;
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

        public List<InstructionType> AvailableInstructionsType
        {
            get { return new List<InstructionType>(Enum.GetValues(typeof(InstructionType)).OfType<InstructionType>()); }
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

        InstructionType _instructionType;
        public InstructionType InstructionType
        {
            get { return _instructionType; }
            set
            {
                _instructionType = value;
                OnPropertyChanged("InstructionType");
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
            Instruction.InstructionType = InstructionType;
            if (_isNew)
            {
                FiresecManager.SystemConfiguration.Instructions.Add(Instruction);
            }
        }

        bool CanSelectZone(object obj)
        {
            if (InstructionType == InstructionType.Zone)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool CanSelectDevice(object obj)
        {
            if (InstructionType == InstructionType.Device)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public RelayCommand SelectZoneCommand { get; private set; }
        void OnSelectZoneCommand()
        {
            var instructionZonesViewModel = new InstructionZonesViewModel();
            instructionZonesViewModel.Inicialize(Instruction);
            ServiceFactory.UserDialogs.ShowModalWindow(instructionZonesViewModel);
        }

        public RelayCommand SelectDeviceCommand { get; private set; }
        void OnSelectDeviceCommand()
        {
            
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
