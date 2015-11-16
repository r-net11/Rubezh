using RubezhAPI.GK;

namespace GKProcessor
{
	public class MirrorDescriptor : BaseDescriptor
	{
		GKDevice Device { get; set; }

		public MirrorDescriptor(GKDevice device)
			: base(device)
		{
			DescriptorType = DescriptorType.Device;
			Device = device;
			foreach (var gkBase in Device.GKReflectionItem.GKBases)
			{
				Device.LinkToDescriptor(gkBase);
			}
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes(Device.Driver.DriverTypeNo);
			SetAddress(0);
		}

		public override void BuildFormula()
		{
			Formula = new FormulaBuilder();
			int count;

			if (Device.DriverType == GKDriverType.FirefightingZonesMirror)
			{
				count = 0;
				foreach (var gkBase in Device.GKReflectionItem.Diretions)
				{
					Formula.AddGetWord(false, gkBase);
					count++;
					if (count > 1)
					{
						Formula.Add(FormulaOperationType.OR);
					}
				}
				Formula.Add(FormulaOperationType.CONST, 0, 0xFFF1);
				Formula.Add(FormulaOperationType.OR);

				count = 0;
				foreach (var gkBase in Device.GKReflectionItem.Zones)
				{
					Formula.AddGetWord(false, gkBase);
					count++;
					if (count > 1)
					{
						Formula.Add(FormulaOperationType.OR);
					}
				}
				if (count > 0)
				{
					Formula.Add(FormulaOperationType.CONST, 0, 0xE);
					Formula.Add(FormulaOperationType.OR);
					Formula.Add(FormulaOperationType.OR);
				}
				Formula.AddPutWord(false, Device);

				count = 0;
				foreach (var gkBase in Device.GKReflectionItem.Diretions)
				{
					Formula.AddGetWord(true, gkBase);
					count++;
					if (count > 1)
					{
						Formula.Add(FormulaOperationType.OR);
					}
				}
			}
			else
			{
				count = 0;
				foreach (var gkBase in Device.GKReflectionItem.GKBases)
				{
					Formula.AddGetWord(false, gkBase);
					count++;
					if (count > 1)
					{
						Formula.Add(FormulaOperationType.OR);
					}
				}
				Formula.AddPutWord(false, Device);
				count = 0;
				foreach (var gkBase in Device.GKReflectionItem.GKBases)
				{
					Formula.AddGetWord(true, gkBase);
					count++;
					if (count > 1)
					{
						Formula.Add(FormulaOperationType.OR);
					}
				}
			}

			Formula.Add(FormulaOperationType.CONST, 0, 0x400);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddPutWord(true, Device);
		}
	}
}