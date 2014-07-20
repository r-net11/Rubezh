using FiresecAPI.GK;
using FiresecAPI.Models;
using System.Collections.Generic;
using System;

namespace GKProcessor
{
	public class CodeDescriptor : BaseDescriptor
	{
		public CodeDescriptor(XCode code)
		{
			DatabaseType = DatabaseType.Gk;
			DescriptorType = DescriptorType.Code;
			Code = code;
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((ushort)0x109);
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
				Value = (ushort)(Code.Password % 65536)
			});
			binProperties.Add(new BinProperty()
			{
				No = 1,
				Value = (ushort)(Code.Password / 65536)
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