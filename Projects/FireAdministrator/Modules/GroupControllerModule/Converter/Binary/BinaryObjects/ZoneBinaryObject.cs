using XFiresecAPI;
using System.Collections.Generic;

namespace GKModule.Converter.Binary
{
	public class ZoneBinaryObject : BinaryObjectBase
	{
		XZone Zone;

		public ZoneBinaryObject(XZone zone)
		{
			Zone = zone;

			DeviceType = ToBytes((short)0x100);
			Address = ToBytes((short)0);

			InputDependenses = new List<byte>();
			OutputDependenses = new List<byte>();
			Parameters = new List<byte>();
			Offset = new List<byte>();

			SetFormulaBytes();

			InitializeAllBytes();
		}

		void SetFormulaBytes()
		{
		    Formula = new List<byte>();
		    FormulaOperations = new List<FormulaOperation>();

			var count = Zone.DetectorCount;

			foreach (var device in Zone.Devices)
			{
				var no = device.InternalKAUNo;
			}

		//    if (Device.Driver.HasLogic)
		//    {
		//        foreach (var stateLogic in Device.DeviceLogic.StateLogics)
		//        {
		//            for (int clauseIndex = 0; clauseIndex < stateLogic.Clauses.Count; clauseIndex++)
		//            {
		//                var clause = stateLogic.Clauses[clauseIndex];

		//                if (clause.Devices.Count == 1)
		//                {
		//                    var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == clause.Devices[0]);
		//                    GetBit(device, (byte)clause.StateType);

		//                    AddFormulaOperation(FormulaOperationType.PUTBIT,
		//                        (byte)clause.StateType,
		//                        device.InternalKAUNo,
		//                        "Проверка состояния одного объекта");
		//                }
		//                else
		//                {
		//                    var firstDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == clause.Devices[0]);
		//                    GetBit(firstDevice, (byte)clause.StateType);

		//                    for (int i = 1; i < clause.Devices.Count; i++)
		//                    {
		//                        var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == clause.Devices[i]);

		//                        AddFormulaOperation(FormulaOperationType.GETBIT,
		//                            (byte)clause.StateType,
		//                            device.InternalKAUNo);

		//                        var formulaOperationType = FormulaOperationType.AND;
		//                        switch (clause.ClauseOperationType)
		//                        {
		//                            case ClauseOperationType.All:
		//                                formulaOperationType = FormulaOperationType.AND;
		//                                break;

		//                            case ClauseOperationType.One:
		//                                formulaOperationType = FormulaOperationType.OR;
		//                                break;
		//                        }

		//                        AddFormulaOperation(formulaOperationType,
		//                            comment: "Проверка состояния очередного объекта объекта");
		//                    }
		//                }

		//                if (clauseIndex + 1 < stateLogic.Clauses.Count)
		//                {
		//                    var formulaOperationType = FormulaOperationType.AND;
		//                    switch (clause.ClauseJounOperationType)
		//                    {
		//                        case ClauseJounOperationType.And:
		//                            formulaOperationType = FormulaOperationType.AND;
		//                            break;

		//                        case ClauseJounOperationType.Or:
		//                            formulaOperationType = FormulaOperationType.OR;
		//                            break;
		//                    }
		//                    AddFormulaOperation(formulaOperationType,
		//                        comment: "Объединение нескольких условий");
		//                }
		//            }

		//            AddFormulaOperation(FormulaOperationType.PUTBIT,
		//                (byte)stateLogic.StateType,
		//                Device.InternalKAUNo,
		//                "Запись бита глобального словосостояния");
		//        }
		    }

		//    AddFormulaOperation(FormulaOperationType.END,
		//        comment: "Завершающий оператор");

		//    foreach (var formulaOperation in FormulaOperations)
		//    {
		//        Formula.Add((byte)formulaOperation.FormulaOperationType);
		//        Formula.Add(formulaOperation.FirstOperand);
		//        Formula.AddRange(BitConverter.GetBytes(formulaOperation.SecondOperand));
		//    }
		//}

		//void AddFormulaOperation(FormulaOperationType formulaOperationType, byte firstOperand = 0, short secondOperand = 0, string comment = null)
		//{
		//    var formulaOperation = new FormulaOperation()
		//    {
		//        FormulaOperationType = formulaOperationType,
		//        FirstOperand = firstOperand,
		//        SecondOperand = secondOperand,
		//        Comment = comment
		//    };
		//    FormulaOperations.Add(formulaOperation);
		//}

		//void GetBit(XDevice device, byte bitNo)
		//{
		//    if ((bitNo == 1) || (bitNo == 2) || (bitNo == 3))
		//    {
		//        if (device.Driver.UseOffBitInLogic)
		//        {
		//            AddFormulaOperation(FormulaOperationType.GETBIT,
		//                (byte)6,
		//                device.InternalKAUNo,
		//                "Проверка бита обхода");

		//            AddFormulaOperation(FormulaOperationType.COM);
		//        }
		//    }

		//    AddFormulaOperation(FormulaOperationType.GETBIT,
		//        bitNo,
		//        device.InternalKAUNo);
		//}
	}
}