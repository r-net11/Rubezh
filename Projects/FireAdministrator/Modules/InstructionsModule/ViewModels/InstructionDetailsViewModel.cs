using System;
using System.Collections.Generic;
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
        bool _isNew;
        public Instruction Instruction { get; private set; }
        public List<ulong?> InstructionZonesList { get; set; }
        public List<Guid> InstructionDevicesList { get; set; }

        public InstructionDetailsViewModel()
        {
            InstructionZonesList = new List<ulong?>();
            InstructionDevicesList = new List<Guid>();
            SelectZoneCommand = new RelayCommand(OnSelectZoneCommand, CanSelect);
            SelectDeviceCommand = new RelayCommand(OnSelectDeviceCommand, CanSelect);
        }

        public void Initialize(Instruction instruction = null)
        {
            if (instruction == null)
            {
                _isNew = true;
                Title = "Новая инструкция";

                InstructionNo = 0;
                if (FiresecManager.SystemConfiguration.Instructions.IsNotNullOrEmpty())
                    InstructionNo = FiresecManager.SystemConfiguration.Instructions.Select(x => x.No).Max() + 1;

                Instruction = new Instruction();
            }
            else
            {
                _isNew = false;
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
                            InstructionZonesList = new List<ulong?>(Instruction.InstructionZonesList);
                        if (Instruction.InstructionDevicesList.IsNotNullOrEmpty())
                            InstructionDevicesList = new List<Guid>(Instruction.InstructionDevicesList);
                        break;

                    case InstructionType.General:
                        break;
                }
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

        public string SelectZones
        {
            get
            {
                string selectZones = "";
                if (InstructionZonesList.IsNotNullOrEmpty())
                    selectZones = InstructionZonesList[0].ToString();

                if (InstructionZonesList.Count > 1)
                {
                    for (int i = 1; i < InstructionZonesList.Count; ++i)
                        selectZones += ", " + InstructionZonesList[i];
                }
                return selectZones;
            }
        }

        public string SelectDevices
        {
            get
            {
                string selectDevices = "";
                if (InstructionDevicesList.IsNotNullOrEmpty())
                {
                    Device device;
                    if (FiresecManager.DeviceConfiguration.Devices.IsNotNullOrEmpty())
                        device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == InstructionDevicesList[0]);
                    else
                        device = null;

                    if (device != null)
                        selectDevices = device.Driver.ShortName + " (" + device.PresentationAddress + ")";
                }

                if (InstructionDevicesList.Count > 1)
                {
                    Device device;
                    for (int i = 1; i < InstructionDevicesList.Count; ++i)
                    {
                        if (FiresecManager.DeviceConfiguration.Devices.IsNotNullOrEmpty())
                            device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == InstructionDevicesList[i]);
                        else
                            device = null;

                        if (device != null)
                            selectDevices += ", " + device.Driver.ShortName + " (" + device.PresentationAddress + ")";
                    }
                }
                return selectDevices;
            }
        }

        bool CanSelect()
        {
            return (InstructionType != InstructionType.General);
        }

        public RelayCommand SelectZoneCommand { get; private set; }
        void OnSelectZoneCommand()
        {
            var instructionZonesViewModel = new InstructionZonesViewModel(InstructionZonesList);
            if (ServiceFactory.UserDialogs.ShowModalWindow(instructionZonesViewModel))
            {
                InstructionZonesList = instructionZonesViewModel.InstructionZonesList;
                OnPropertyChanged("SelectZones");
            }
        }

        public RelayCommand SelectDeviceCommand { get; private set; }
        void OnSelectDeviceCommand()
        {
            var instructionDevicesViewModel = new InstructionDevicesViewModel(InstructionDevicesList);
            if (ServiceFactory.UserDialogs.ShowModalWindow(instructionDevicesViewModel))
            {
                InstructionDevicesList = instructionDevicesViewModel.InstructionDevicesList;
                OnPropertyChanged("SelectDevices");
            }
        }

        protected override bool CanSave()
        {
            if (string.IsNullOrWhiteSpace(Text))
                return false;
            else
                return InstructionType == InstructionType.General ? true : (InstructionDevicesList.IsNotNullOrEmpty() || InstructionZonesList.IsNotNullOrEmpty());
        }

        protected override void Save(ref bool cancel)
        {
            Instruction.Text = Text;
            Instruction.StateType = StateType;
            Instruction.InstructionType = InstructionType;
            Instruction.No = InstructionNo;
            if (InstructionType == InstructionType.General)
            {
                InstructionDevicesList.Clear();
                InstructionZonesList.Clear();
            }
            Instruction.InstructionDevicesList = InstructionDevicesList;
            Instruction.InstructionZonesList = InstructionZonesList;
            if (_isNew)
            {
                FiresecManager.SystemConfiguration.Instructions.Add(Instruction);
            }
        }
    }
}