using System;
using System.Collections.Generic;
using XFiresecAPI;

namespace Common.GK
{
	public class DelayDescriptor : BaseDescriptor
	{
		public DelayDescriptor(XDelay delay)
		{
			DatabaseType = DatabaseType.Gk;
			DescriptorType = DescriptorType.Delay;
			Delay = delay;
			Build();
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((ushort)0x101);
			SetAddress((ushort)0);
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
				Value = Delay.DelayTime
			});
			binProperties.Add(new BinProperty()
			{
				No = 1,
				Value = Delay.SetTime
			});
			binProperties.Add(new BinProperty()
			{
				No = 2,
				Value = (ushort)Delay.DelayRegime
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