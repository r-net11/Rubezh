using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecAPI.Models;
using FiresecClient;
using FiresecAPI;

namespace InstructionsModule.ViewModels
{
    public class InstructionDetailsViewModel : DialogContent
    {
        public InstructionDetailsViewModel()
        {
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        bool _isNew;
        public Instruction Instruction { get; private set; }

        public void Initialize()
        {
            _isNew = false;
            Instruction = new Instruction();
            Title = "Новая инструкция";
        }

        public void Initialize(Instruction instruction)
        {
            _isNew = false;
            Instruction = instruction;
            Name = instruction.Name;
            Text = instruction.Text;
            State = instruction.StateType;
            Title = "Редактирование инструкции";
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

        StateType _state;
        public StateType State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged("State");
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
            Instruction.StateType = State;
            if (_isNew)
            {
                FiresecManager.SystemConfiguration.Instructions.Add(Instruction);
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
