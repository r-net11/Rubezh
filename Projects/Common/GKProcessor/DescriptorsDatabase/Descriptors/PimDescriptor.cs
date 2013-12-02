using System;
using System.Collections.Generic;
using XFiresecAPI;

namespace GKProcessor
{
	public class PimDescriptor : BaseDescriptor
	{
		public PimDescriptor(XPim pim)
		{
			DatabaseType = DatabaseType.Gk;
			DescriptorType = DescriptorType.Pim;
			Pim = pim;
			Build();
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((ushort)0x107);
			SetAddress((ushort)0);
			SetFormulaBytes();
			SetPropertiesBytes();
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
				Value = Pim.DelayTime
			});
			binProperties.Add(new BinProperty()
			{
				No = 1,
				Value = Pim.SetTime
			});
			binProperties.Add(new BinProperty()
			{
				No = 2,
				Value = (ushort)Pim.DelayRegime
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