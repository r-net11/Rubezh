using System;
using System.Collections.Generic;
using RubezhAPI.GK;

namespace GKProcessor
{
	public class CodeDescriptor : BaseDescriptor
	{
		GKCode Code { get; set; }

		public CodeDescriptor(GKCode code)
			: base(code)
		{
			DescriptorType = DescriptorType.Code;
			Code = code;
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((ushort)0x109);
			SetAddress((ushort)0);
			SetPropertiesBytes();
		}

		public override void BuildFormula()
		{
			Formula = new FormulaBuilder();
			Formula.Add(FormulaOperationType.END);
		}

		void SetPropertiesBytes()
		{
			Parameters = new List<byte>();
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