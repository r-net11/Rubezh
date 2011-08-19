using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecAPI.Models;
using FiresecClient;
using FiresecAPI;
using InstructionsModule.Views;
using Infrastructure;

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
            Instruction = new Instruction();
            Title = "Новая инструкция";
            InstructionZonesViewModel = new InstructionZonesViewModel();
        }

        public void Initialize(Instruction instruction)
        {
            _isNew = false;
            Instruction = instruction;
            Name = instruction.Name;
            Text = instruction.Text;
            StateType = instruction.StateType;
            Title = "Редактирование инструкции";
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
            get { return null; }
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
            Instruction.StateType = StateType.Fire;
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
