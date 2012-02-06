using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace InstructionsModule.ViewModels
{
    public class InstructionDetailsViewModel : SaveCancelDialogContent
    {
        public Instruction Instruction { get; private set; }

        public InstructionDetailsViewModel(Instruction instruction = null)
        {
            SelectZoneCommand = new RelayCommand(OnSelectZoneCommand, CanSelect);
            SelectDeviceCommand = new RelayCommand(OnSelectDeviceCommand, CanSelect);

            if (instruction != null)
            {
                Title = "Редактирование инструкции";
                Instruction = instruction;
            }
            else
            {
                Title = "Новая инструкция";

                InstructionNo = 1;
                if (FiresecManager.SystemConfiguration.Instructions.IsNotNullOrEmpty())
                    InstructionNo = FiresecManager.SystemConfiguration.Instructions.Select(x => x.No).Max() + 1;

                Instruction = new Instruction();
            }

            CopyProperties();
        }

        void CopyProperties()
        {
            InstructionZones = new ObservableCollection<ulong?>();
            InstructionDevices = new ObservableCollection<Guid>();

            InstructionNo = Instruction.No;
            Name = Instruction.Name;
            Text = Instruction.Text;
            StateType = Instruction.StateType;
            InstructionType = Instruction.InstructionType;
            switch (InstructionType)
            {
                case InstructionType.Details:
                    if (Instruction.Zones.IsNotNullOrEmpty())
                        InstructionZones = new ObservableCollection<ulong?>(Instruction.Zones);
                    if (Instruction.Devices.IsNotNullOrEmpty())
                        InstructionDevices = new ObservableCollection<Guid>(Instruction.Devices);
                    break;

                case InstructionType.General:
                    break;
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

        public bool IsDetails
        {
            get { return InstructionType == InstructionType.Details; }
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
                OnPropertyChanged("IsDetails");
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
            Instruction.No = InstructionNo;
            Instruction.Name = Name;
            Instruction.Text = Text;
            Instruction.StateType = StateType;
            Instruction.InstructionType = InstructionType;
            if (InstructionType == InstructionType.Details)
            {
                Instruction.Devices = InstructionDevices.ToList();
                Instruction.Zones = InstructionZones.ToList();
            }
            else
            {
                Instruction.Devices = new List<Guid>();
                Instruction.Zones = new List<ulong?>();
            }
        }
    }
}