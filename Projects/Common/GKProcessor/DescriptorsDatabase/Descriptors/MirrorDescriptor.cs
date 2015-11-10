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
			foreach (var dev in Device.GKReflectionItem.Devices)
			{
				Device.LinkToDescriptor(dev);
			}
			foreach (var dir in Device.GKReflectionItem.Diretions)
			{
				Device.LinkToDescriptor(dir);
			}
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes(Device.Driver.DriverTypeNo);

			var address = 0;
			if (Device.Driver.IsDeviceOnShleif)
				address = (Device.ShleifNo - 1) * 256 + Device.IntAddress;
			SetAddress((ushort)address);
		}

		public override void BuildFormula()
		{
			Formula = new FormulaBuilder();
			int count = 0;
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

			Formula.Add(FormulaOperationType.CONST, 0, 0x400);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddPutWord(true, Device);
		}
	}
}
