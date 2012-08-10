using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace Common.GK
{
	public class DirectionBinaryObject : BinaryObjectBase
	{
		public DirectionBinaryObject(XDirection direction, DatabaseType databaseType)
		{
			DatabaseType = databaseType;
			ObjectType = ObjectType.Direction;
			Direction = direction;
			Build();
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((short)0x106);
			SetAddress((short)0);
			Parameters = new List<byte>();
			SetFormulaBytes();
			SetPropertiesBytes();
			InitializeAllBytes();
		}

		void SetFormulaBytes()
		{
			Formula = new FormulaBuilder();
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
				Value = Direction.Regime
			});

			Parameters = new List<byte>();
			foreach (var binProperty in binProperties)
			{
				Parameters.Add(binProperty.No);
				Parameters.AddRange(BitConverter.GetBytes(binProperty.Value));
				Parameters.Add(0);
			}
		}
	}
}