using FiresecAPI.GK;
using FiresecAPI.Models;
using System.Collections.Generic;
using System;

namespace GKProcessor
{
	public class GuardZoneDescriptor : BaseDescriptor
	{
		public GuardZoneDescriptor(XGuardZone zone)
		{
			DatabaseType = DatabaseType.Gk;
			DescriptorType = DescriptorType.GuardZone;
			GuardZone = zone;
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((ushort)0x108);
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
				Value = (ushort)GuardZone.Delay
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