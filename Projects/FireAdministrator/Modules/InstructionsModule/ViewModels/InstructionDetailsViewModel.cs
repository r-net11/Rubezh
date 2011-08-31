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
    public class InstructionDetailsViewModel : DialogContent
    {
        public InstructionDetailsViewModel()
        {
            SaveCommand = new RelayCommand(OnSave, CanSave);
            CancelCommand = new RelayCommand(OnCancel);
            SelectZoneCommand = new RelayCommand(OnSelectZoneCommand, CanSelect);
            SelectDeviceCommand = new RelayCommand(OnSelectDeviceCommand, CanSelect);
            InstructionZonesList = new List<string>();
            InstructionDevicesList = new List<string>();
        }

        bool _isNew;
        public Instruction Instruction { get; private set; }
        public List<string> InstructionZonesList { get; set; }
        public List<string> InstructionDevicesList { get; set; }

        public void Initialize()
        {
            _isNew = true;
            Title = "Новая инструкция";
            var maxNo = 0;
            if (FiresecManager.SystemConfiguration.Instructions.IsNotNullOrEmpty())
            {
                maxNo = (from instruction in FiresecManager.SystemConfiguration.Instructions select int.Parse(instruction.No)).Max();
            }
            Instruction = new Instruction();
            InstructionNo = (maxNo + 1).ToString();
        }

        public void Initialize(Instruction instruction)
        {
            _isNew = false;
            Title = "Редактирование инструкции";
            Instruction = instruction;
            //Name = instruction.Name;
            Text = instruction.Text;
            StateType = instruction.StateType;
            InstructionNo = instruction.No;
            InstructionType = instruction.InstructionType;
            switch (InstructionType)
            {
                case InstructionType.General:
                    break;
                default:
                    if (Instruction.InstructionZonesList.IsNotNullOrEmpty())
                    {
                        InstructionZonesList = new List<string>(Instruction.InstructionZonesList);
                    }
                    if (Instruction.InstructionDevicesList.IsNotNullOrEmpty())
                    {
                        InstructionDevicesList = new List<string>(Instruction.InstructionDevicesList);
                    }
                    break;
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

        string _instructionNo;
        public string InstructionNo
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
                {
                    selectZones = InstructionZonesList[0];
                }

                if (InstructionZonesList.Count > 1)
                {
                    for (int i = 1; i < InstructionZonesList.Count; i++)
                    {
                        selectZones += ", " + InstructionZonesList[i];
                    }
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
                    {
                        device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == InstructionDevicesList[0]);
                    }
                    else
                    {
                        device = null;
                    }
                    if (device != null)
                    {
                        selectDevices = device.Driver.ShortName + " (" + device.AddressFullPath + ")";
                    }
                }

                if (InstructionDevicesList.Count > 1)
                {
                    Device device;
                    for (int i = 1; i < InstructionDevicesList.Count; i++)
                    {
                        if (FiresecManager.DeviceConfiguration.Devices.IsNotNullOrEmpty())
                        {
                            device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == InstructionDevicesList[i]);
                        }
                        else
                        {
                            device = null;
                        }
                        if (device != null)
                        {
                            selectDevices += ", " + device.Driver.ShortName + " (" + device.AddressFullPath + ")";
                        }
                    }
                }
                return selectDevices;
            }
        }

        bool CanSelect()
        {
            return (InstructionType != InstructionType.General);
        }

        bool CanSave()
        {
            if (string.IsNullOrWhiteSpace(Text))
            {
                return false;
            }
            else
            {
                if (InstructionType == InstructionType.General)
                {
                    return true;
                }
                else
                {
                    return ((InstructionDevicesList.IsNotNullOrEmpty()) || (InstructionZonesList.IsNotNullOrEmpty()));
                }
            }
        }

        public RelayCommand SelectZoneCommand { get; private set; }
        void OnSelectZoneCommand()
        {
            var instructionZonesViewModel = new InstructionZonesViewModel();
            instructionZonesViewModel.Inicialize(InstructionZonesList);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(instructionZonesViewModel);
            if (result)
            {
                InstructionZonesList = instructionZonesViewModel.InstructionZonesList;
                OnPropertyChanged("SelectZones");
            }
        }

        public RelayCommand SelectDeviceCommand { get; private set; }
        void OnSelectDeviceCommand()
        {
            var instructionDevicesViewModel = new InstructionDevicesViewModel();
            instructionDevicesViewModel.Inicialize(InstructionDevicesList);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(instructionDevicesViewModel);
            if (result)
            {
                InstructionDevicesList = instructionDevicesViewModel.InstructionDevicesList;
                OnPropertyChanged("SelectDevices");
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

        void Save()
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
                //InstructionsModule.HasChanges = true;
            }
        }
    }
}
