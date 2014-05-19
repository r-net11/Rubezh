using System;
using System.Collections.Generic;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class DirectionDescriptor : BaseDescriptor
	{
		public DirectionDescriptor(XDirection direction)
		{
			DatabaseType = DatabaseType.Gk;
			DescriptorType = DescriptorType.Direction;
			Direction = direction;
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((ushort)0x106);
			SetAddress((ushort)0);
			SetFormulaBytes();
			SetPropertiesBytes();
		}

		void SetFormulaBytes()
		{
			Formula = new FormulaBuilder();
			if (Direction.InputZones.Count > 0 || Direction.InputDevices.Count > 0)
			{
				var inputObjectsCount = 0;
				foreach (var directionZone in Direction.DirectionZones)
				{
					Formula.AddGetBitOff(directionZone.StateBit, directionZone.Zone);
					if (inputObjectsCount > 0)
					{
						Formula.Add(FormulaOperationType.OR);
					}
					inputObjectsCount++;
				}
				foreach (var directionDevice in Direction.DirectionDevices)
				{
					Formula.AddGetBitOff(directionDevice.StateBit, directionDevice.Device);
					if (inputObjectsCount > 0)
					{
						Formula.Add(FormulaOperationType.OR);
					}
					inputObjectsCount++;
				}

				Formula.Add(FormulaOperationType.DUP);

				Formula.AddGetBit(XStateBit.Norm, Direction);
				Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Направления");
				Formula.AddPutBit(XStateBit.TurnOn_InAutomatic, Direction);

				Formula.Add(FormulaOperationType.COM, comment: "Условие Выключения");
				Formula.AddGetBit(XStateBit.Norm, Direction);
				Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Направления");
				Formula.AddPutBit(XStateBit.TurnOff_InAutomatic, Direction);
			}
			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}

		void SetPropertiesBytes()
		{
			var binProperties = new List<BinProperty>();
			binProperties.Add(new BinProperty()
			{
				No = 0,
				Value = Direction.Delay
			});
			binProperties.Add(new BinProperty()
			{
				No = 1,
				Value = Direction.Hold
			});
			binProperties.Add(new BinProperty()
			{
				No = 2,
				Value = (ushort)Direction.DelayRegime
			});

			foreach (var binProperty in binProperties)
			{
				Parameters.Add(binProperty.No);
				Parameters.AddRange(BitConverter.GetBytes(binProperty.Value));
				Parameters.Add(0);
			}
		}
	}
}