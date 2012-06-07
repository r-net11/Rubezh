using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace InstructionsModule.ViewModels
{
    public class InstructionDetailsViewModel : SaveCancelDialogViewModel
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
                CopyProperties();
            }
            else
            {
                Title = "Новая инструкция";
                Instruction = new Instruction();
                CopyProperties();
                if (FiresecManager.SystemConfiguration.Instructions.IsNotNullOrEmpty())
                    InstructionNo = FiresecManager.SystemConfiguration.Instructions.Select(x => x.No).Max() + 1;
            }
        }

        void CopyProperties()
        {
            InstructionZones = new ObservableCollection<ulong>();
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
                        InstructionZones = new ObservableCollection<ulong>(Instruction.Zones);
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
        public List<StateType> AvailableStates
        {
            get { return new List<StateType>(Enum.GetValues(typeof(StateType)).OfType<StateType>()); }
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
        public List<InstructionType> AvailableInstructionsType
        {
            get { return new List<InstructionType>(Enum.GetValues(typeof(InstructionType)).OfType<InstructionType>()); }
        }

        ObservableCollection<ulong> _instructionZones;
        public ObservableCollection<ulong> InstructionZones
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
            if (DialogService.ShowModalWindow(instructionZonesViewModel))
            {
                InstructionZones = new ObservableCollection<ulong>(instructionZonesViewModel.InstructionZonesList);
            }
        }

        public RelayCommand SelectDeviceCommand { get; private set; }
        void OnSelectDeviceCommand()
        {
            var instructionDevicesViewModel = new InstructionDevicesViewModel(InstructionDevices.ToList());
			if (DialogService.ShowModalWindow(instructionDevicesViewModel))
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

		protected override bool Save()
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
                Instruction.Zones = new List<ulong>();
            }
			return base.Save();
		}
	}
}