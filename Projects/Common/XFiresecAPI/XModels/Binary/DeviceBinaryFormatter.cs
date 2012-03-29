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
                            var formulaOperation1 = new FormulaOperation()
                            {
                                FormulaOperationType = FormulaOperationType.GETBIT,
                                FirstOperand = (byte)clause.StateType,
                                SecondOperand = device.InternalKAUNo
                            };
                            FormulaOperations.Add(formulaOperation1);

                            var formulaOperation2 = new FormulaOperation()
                            {
                                FormulaOperationType = FormulaOperationType.PUTBIT,
                                FirstOperand = (byte)clause.StateType,
                                SecondOperand = device.InternalKAUNo,
                                Comment = "Проверка состояния одного объекта"
                            };
                            FormulaOperations.Add(formulaOperation2);
                        }
                        else
                        {
                            var forstDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == clause.Devices[0]);
                            var formulaOperation1 = new FormulaOperation()
                            {
                                FormulaOperationType = FormulaOperationType.GETBIT,
                                FirstOperand = (byte)clause.StateType,
                                SecondOperand = forstDevice.InternalKAUNo
                            };
                            FormulaOperations.Add(formulaOperation1);

                            for (int i = 1; i < clause.Devices.Count; i++)
                            {
                                var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == clause.Devices[i]);
                                var formulaOperation = new FormulaOperation()
                                {
                                    FormulaOperationType = FormulaOperationType.GETBIT,
                                    FirstOperand = (byte)clause.StateType,
                                    SecondOperand = device.InternalKAUNo
                                };
                                FormulaOperations.Add(formulaOperation);

                                var formulaOperation2 = new FormulaOperation()
                                {
                                    Comment = "Проверка состояния очередного объекта объекта"
                                };
                                switch (clause.ClauseOperationType)
                                {
                                    case ClauseOperationType.All:
                                        formulaOperation2.FormulaOperationType = FormulaOperationType.AND;
                                        break;

                                    case ClauseOperationType.One:
                                        formulaOperation2.FormulaOperationType = FormulaOperationType.OR;
                                        break;
                                }

                                FormulaOperations.Add(formulaOperation2);
                            }
                        }

                        if (clauseIndex + 1 < stateLogic.Clauses.Count)
                        {
                            var formulaOperation3 = new FormulaOperation()
                            {
                                Comment = "Объединение нескольких условий"
                            };
                            switch (clause.ClauseJounOperationType)
                            {
                                case ClauseJounOperationType.And:
                                    formulaOperation3.FormulaOperationType = FormulaOperationType.AND;
                                    break;

                                case ClauseJounOperationType.Or:
                                    formulaOperation3.FormulaOperationType = FormulaOperationType.OR;
                                    break;
                            }
                            FormulaOperations.Add(formulaOperation3);
                        }
                    }

                    var formulaOperation4 = new FormulaOperation()
                    {
                        FormulaOperationType = FormulaOperationType.PUTBIT,
                        FirstOperand = (byte)stateLogic.StateType,
                        SecondOperand = Device.InternalKAUNo,
                        Comment = "Запись бита глобального словосостояния"
                    };
                    FormulaOperations.Add(formulaOperation4);
                }
            }

            var formulaOperation5 = new FormulaOperation()
            {
                FormulaOperationType = FormulaOperationType.END,
                Comment = "Завершающий оператор"
            };
            FormulaOperations.Add(formulaOperation5);

            foreach (var formulaOperation in FormulaOperations)
            {
                Formula.Add((byte)formulaOperation.FormulaOperationType);
                Formula.Add(formulaOperation.FirstOperand);
                Formula.AddRange(BitConverter.GetBytes(formulaOperation.SecondOperand));
            }
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