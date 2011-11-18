using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace InstructionsModule.ViewModels
{
    public class InstructionDetailsViewModel : SaveCancelDialogContent
    {
        public Instruction Instruction { get; private set; }

        public InstructionDetailsViewModel()
        {
            InstructionZones = new ObservableCollection<ulong?>();
            InstructionDevices = new ObservableCollection<Guid>();

            SelectZoneCommand = new RelayCommand(OnSelectZoneCommand, CanSelect);
            SelectDeviceCommand = new RelayCommand(OnSelectDeviceCommand, CanSelect);
        }

        public void Initialize()
        {
            Title = "Новая инструкция";

            InstructionNo = 0;
            if (FiresecManager.SystemConfiguration.Instructions.IsNotNullOrEmpty())
                InstructionNo = FiresecManager.SystemConfiguration.Instructions.Select(x => x.No).Max() + 1;

            Instruction = new Instruction();
        }

        public void Initialize(Instruction instruction)
        {
            Title = "Редактирование инструкции";

            Instruction = instruction;
            Text = instruction.Text;
            StateType = instruction.StateType;
            InstructionNo = instruction.No;
            InstructionType = instruction.InstructionType;
            switch (InstructionType)
            {
                case InstructionType.Details:
                    if (Instruction.InstructionZonesList.IsNotNullOrEmpty())
                        InstructionZones = new ObservableCollection<ulong?>(Instruction.InstructionZonesList);
                    if (Instruction.InstructionDevicesList.IsNotNullOrEmpty())
                        InstructionDevices = new ObservableCollection<Guid>(Instruction.InstructionDevicesList);
                    break;

                case InstructionType.General:
                    break;
            }
        }

        ObservableCollection<ulong?> _instructionZones;
        public ObservableCollection<ulong?> InstructionZones
        {
            get { return _instructionZones; }
            set
            {
                _instructionZones = value;
                OnPropertyChanged("InstructionZones");
            }
        }

        ObservableCollection<Guid> _instructionDevices;
        public ObservableCollection<Guid> InstructionDevices
        {
            get { return _instructionDevices; }
            set
            {
                _instructionDevices = value;
                OnPropertyChanged("InstructionDevices");
            }
        }

        public List<StateType> AvailableStates
        {
            get { return new List<StateType>(Enum.GetValues(typeof(StateType)).OfType<StateType>()); }
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
                OnPropertyChanged("StateTypeFilter");
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

        ulong _instructionNo;
        public ulong InstructionNo
        {
            get { return _instructionNo; }
            set
            {
                _instructionNo = value;
                OnPropertyChanged("InstructionNo");
            }
        }

        bool CanSelect()
        {
            return (InstructionType != InstructionType.General);
        }

        public RelayCommand SelectZoneCommand { get; private set; }
        void OnSelectZoneCommand()
        {
            var instructionZonesViewModel = new InstructionZonesViewModel(InstructionZones.ToList());
            if (ServiceFactory.UserDialogs.ShowModalWindow(instructionZonesViewModel))
            {
                InstructionZones = new ObservableCollection<ulong?>(instructionZonesViewModel.InstructionZonesList);
            }
        }

        public RelayCommand SelectDeviceCommand { get; private set; }
        void OnSelectDeviceCommand()
        {
            var instructionDevicesViewModel = new InstructionDevicesViewModel(InstructionDevices.ToList());
            if (ServiceFactory.UserDialogs.ShowModalWindow(instructionDevicesViewModel))
            {
                InstructionDevices = new ObservableCollection<Guid>(instructionDevicesViewModel.InstructionDevicesList);
            }
        }

        protected override bool CanSave()
        {
            if (string.IsNullOrWhiteSpace(Text))
                return false;
            else
                return InstructionType == InstructionType.General ? true : (InstructionDevices.IsNotNullOrEmpty() || InstructionZones.IsNotNullOrEmpty());
        }

        protected override void Save(ref bool cancel)
        {
            Instruction.Text = Text;
            Instruction.StateType = StateType;
            Instruction.InstructionType = InstructionType;
            Instruction.No = InstructionNo;
            if (InstructionType == InstructionType.Details)
            {
                Instruction.InstructionDevicesList = InstructionDevices.ToList();
                Instruction.InstructionZonesList = InstructionZones.ToList();
            }
            else
            {
                Instruction.InstructionDevicesList = new List<Guid>();
                Instruction.InstructionZonesList = new List<ulong?>();
            }
        }
    }
}