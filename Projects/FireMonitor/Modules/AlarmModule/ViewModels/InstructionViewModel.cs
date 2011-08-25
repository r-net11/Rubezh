using Infrastructure.Common;
using FiresecClient;
using System;
using System.Linq;
using System.Collections.Generic;
using Common;
using FiresecAPI.Models;

namespace AlarmModule.ViewModels
{
    public class InstructionViewModel : DialogContent
    {
        private InstructionViewModel() { }
        public InstructionViewModel(string deviceId, AlarmType alarmType)
        {
            Title = "Инструкции";
            DeviceId = deviceId;
            StateType = AlarmTypeToStateType(alarmType);
            InicializeInstruction();

            CloseCommand = new RelayCommand(OnCloseInstruction);
        }

        void InicializeInstruction()
        {
            if (FindInstruction(AvailableInstructionsDevices, DeviceId))
            {
                return;
            }
            if (FindInstruction(AvailableInstructionsZones, ZoneNo))
            {
                return;
            }
            Instruction = InstructionGeneral;
        }

        public string DeviceId { get; private set; }
        public StateType StateType { get; private set; }
        public Instruction Instruction { get; private set; }

        public string ZoneNo
        {
            get 
            {
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == DeviceId);
                if (device != null)
                {
                    return device.ZoneNo;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        List<Instruction> AvailableInstructions
        {
            get
            {
                if (FiresecClient.FiresecManager.SystemConfiguration.Instructions.IsNotNullOrEmpty())
                {
                    return FiresecClient.FiresecManager.SystemConfiguration.Instructions.FindAll(x => x.StateType == StateType);
                }
                else
                {
                    return new List<Instruction>();
                }
            }
        }
        List<Instruction> AvailableInstructionsDevices
        {
            get
            {
                if (AvailableInstructions.IsNotNullOrEmpty())
                {
                    return AvailableInstructions.FindAll(x => x.InstructionType == InstructionType.Device);
                }
                else
                {
                    return AvailableInstructions;
                }
            }
        }
        List<Instruction> AvailableInstructionsZones
        {
            get
            {
                if (AvailableInstructions.IsNotNullOrEmpty())
                {
                    return AvailableInstructions.FindAll(x => x.InstructionType == InstructionType.Zone);
                }
                else
                {
                    return AvailableInstructions;
                }
            }
        }
        Instruction InstructionGeneral
        {
            get
            {
                if (AvailableInstructions.IsNotNullOrEmpty())
                {
                    return AvailableInstructions.FirstOrDefault(x => x.InstructionType == InstructionType.General);
                }
                else
                {
                    return new Instruction();
                }
            }
        }

        StateType AlarmTypeToStateType(AlarmType alarmType)
        {
            switch (alarmType)
            {
                case AlarmType.Fire:
                    return (StateType)0;

                case AlarmType.Attention:
                    return (StateType)1;

                case AlarmType.Failure:
                    return (StateType)2;

                case AlarmType.Off:
                    return (StateType)4;

                case AlarmType.Info:
                    return (StateType)6;

                case AlarmType.Service:
                    return (StateType)3;

                default:
                    return (StateType)8;
            }
        }

        bool FindInstruction(List<Instruction> instructionsList, string number)
        {
            if (instructionsList.IsNotNullOrEmpty())
            {
                foreach (var instruction in instructionsList)
                {
                    if (instruction.InstructionDetailsList.Contains(number))
                    {
                        Instruction = instruction;
                        return true;
                    }
                }
            }

            return false;
        }

        public RelayCommand CloseCommand { get; private set; }
        void OnCloseInstruction()
        {
            Close(true);
        }
    }
}