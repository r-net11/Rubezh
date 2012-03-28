using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XFiresecAPI
{
    public class DeviceBinaryFormatter : BinaryFormatterBase
    {
        XDevice Device;

        public void Initialize(XDevice device)
        {
            Device = device;

            DeviceType = ToBytes(device.Driver.DriverTypeNo);

            short address = 0;
            if (device.Driver.IsDeviceOnShleif)
                address = (short)(device.ShleifNo * 256 + device.IntAddress);
            Address = ToBytes(address);

            SetObjectOutDependencesBytes();
            SetFormulaBytes();            
            SetPropertiesBytes();

            OutDependensesCount = ToBytes((short)(OutDependenses.Count() / 2));
            Offset = ToBytes((short)(8 + OutDependenses.Count() + Formula.Count()));
            ParametersCount = ToBytes((short)(Parameters.Count() / 4));

            InitializeAllBytes();
        }

        void SetObjectOutDependencesBytes()
        {
            OutDependenses = new List<byte>();
            for (int i = 0; i < 10; i++)
            {
                short objectNo = (short)i;
                OutDependenses.AddRange(BitConverter.GetBytes(objectNo));
            }
        }

        void SetFormulaBytes()
        {
            Formula = new List<byte>();

            var formulaOperations = new List<FormulaOperation>();

            if (Device.Driver.HasLogic)
            {
                foreach (var stateLogic in Device.DeviceLogic.StateLogics)
                {
                    foreach (var clause in stateLogic.Clauses)
                    {
                        foreach (var deviceUID in clause.Devices)
                        {
                            var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
                            var formulaOperation = new FormulaOperation();
                            formulaOperation.FirstOperand = (byte)clause.StateType;
                            formulaOperation.SecondOperand = device.InternalKAUNo;
                            formulaOperations.Add(formulaOperation);
                        }
                    }
                }
            }

            var formulaOperationType = FormulaOperationType.END;
            byte operationType = (byte)formulaOperationType;

            byte firstOperand = 0;
            short secondOperand = 0;

            Formula.Add(operationType);
            Formula.Add(firstOperand);
            Formula.AddRange(BitConverter.GetBytes(secondOperand));
        }

        void SetPropertiesBytes()
        {
            Parameters = new List<byte>();

            foreach (var property in Device.Properties)
            {
                var driverProperty = Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
                if (driverProperty.IsInternalDeviceParameter)
                {
                    byte parameterNo = driverProperty.No;
                    short parameterValue = (short)property.Value;

                    Parameters.Add(parameterNo);
                    Parameters.AddRange(BitConverter.GetBytes(parameterValue));
                    Parameters.Add(0);
                }
            }
        }
    }
}