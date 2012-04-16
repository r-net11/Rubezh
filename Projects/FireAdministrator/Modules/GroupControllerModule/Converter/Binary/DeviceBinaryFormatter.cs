using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using FiresecClient;

namespace GroupControllerModule.Converter
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
            InitializeIsGK();
        }

        void InitializeIsGK()
        {
            var kauDevice = Device.AllParents.FirstOrDefault(x => x.Driver.DriverType == XDriverType.KAU);
            var gkDevice = Device.AllParents.FirstOrDefault(x => x.Driver.DriverType == XDriverType.KAU);

            foreach (var stateLogic in Device.DeviceLogic.StateLogics)
            {
                foreach (var clause in stateLogic.Clauses)
                {
                    foreach (var deviceUID in clause.Devices)
                    {
                        var clauseDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
                        var cluseKauDevice = clauseDevice.AllParents.FirstOrDefault(x => x.Driver.DriverType == XDriverType.KAU);
                        var clauseGkDevice = clauseDevice.AllParents.FirstOrDefault(x => x.Driver.DriverType == XDriverType.KAU);
                        if (kauDevice.UID != cluseKauDevice.UID)
                        {
                            IsGKObject = true;
                        }
                        if (gkDevice.UID != clauseGkDevice.UID)
                        {
                            throw (new Exception("Устройства находятся в отношении логики разных ГК"));
                        }
                    }

                    foreach (var zoneNo in clause.Zones)
                    {
                        var zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == zoneNo);
                        var kauDevices = zone.KAUDevices;
                    }
                }
            }
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
            FormulaOperations = new List<FormulaOperation>();

            if (Device.Driver.HasLogic)
            {
                foreach (var stateLogic in Device.DeviceLogic.StateLogics)
                {
                    for (int clauseIndex = 0; clauseIndex < stateLogic.Clauses.Count; clauseIndex++)
                    {
                        var clause = stateLogic.Clauses[clauseIndex];

                        if (clause.Devices.Count == 1)
                        {
                            var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == clause.Devices[0]);

                            AddFormulaOperation(FormulaOperationType.GETBIT,
                                (byte)clause.StateType,
                                device.InternalKAUNo);

                            AddFormulaOperation(FormulaOperationType.PUTBIT,
                                (byte)clause.StateType,
                                device.InternalKAUNo,
                                "Проверка состояния одного объекта");
                        }
                        else
                        {
                            var firstDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == clause.Devices[0]);
                            AddFormulaOperation(FormulaOperationType.GETBIT,
                                (byte)clause.StateType,
                                firstDevice.InternalKAUNo);

                            for (int i = 1; i < clause.Devices.Count; i++)
                            {
                                var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == clause.Devices[i]);

                                AddFormulaOperation(FormulaOperationType.GETBIT,
                                    (byte)clause.StateType,
                                    device.InternalKAUNo);

                                var formulaOperationType = FormulaOperationType.AND;

                                switch (clause.ClauseOperationType)
                                {
                                    case ClauseOperationType.All:
                                        formulaOperationType = FormulaOperationType.AND;
                                        break;

                                    case ClauseOperationType.One:
                                        formulaOperationType = FormulaOperationType.OR;
                                        break;
                                }

                                AddFormulaOperation(formulaOperationType,
                                    0,
                                    0,
                                    "Проверка состояния очередного объекта объекта");
                            }
                        }

                        if (clauseIndex + 1 < stateLogic.Clauses.Count)
                        {
                            var formulaOperationType = FormulaOperationType.AND;
                            switch (clause.ClauseJounOperationType)
                            {
                                case ClauseJounOperationType.And:
                                    formulaOperationType = FormulaOperationType.AND;
                                    break;

                                case ClauseJounOperationType.Or:
                                    formulaOperationType = FormulaOperationType.OR;
                                    break;
                            }
                            AddFormulaOperation(formulaOperationType,
                                0,
                                0,
                                "Объединение нескольких условий");
                        }
                    }

                    AddFormulaOperation(FormulaOperationType.PUTBIT,
                        (byte)stateLogic.StateType,
                        Device.InternalKAUNo,
                        "Запись бита глобального словосостояния");
                }
            }
            else
            {
                AddFormulaOperation(FormulaOperationType.PUTBIT,
                    (byte)XStateType.Ignore,
                    Device.InternalKAUNo,
                    "Проверка бита обхода");
            }

            AddFormulaOperation(FormulaOperationType.END,
                0,
                0,
                "Завершающий оператор");

            foreach (var formulaOperation in FormulaOperations)
            {
                Formula.Add((byte)formulaOperation.FormulaOperationType);
                Formula.Add(formulaOperation.FirstOperand);
                Formula.AddRange(BitConverter.GetBytes(formulaOperation.SecondOperand));
            }
        }

        void AddFormulaOperation(FormulaOperationType formulaOperationType, byte firstOperand = 0, short secondOperand = 0, string comment = null)
        {
            var formulaOperation = new FormulaOperation()
            {
                FormulaOperationType = formulaOperationType,
                FirstOperand = firstOperand,
                SecondOperand = secondOperand,
                Comment = comment
            };
            FormulaOperations.Add(formulaOperation);
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