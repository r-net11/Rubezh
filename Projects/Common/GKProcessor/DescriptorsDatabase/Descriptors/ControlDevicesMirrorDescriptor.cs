using RubezhAPI.GK;

namespace GKProcessor
{
	public class ControlDevicesMirrorDescriptor : BaseDescriptor
	{
		GKDevice Device { get; set; }

		public ControlDevicesMirrorDescriptor(GKDevice device)
			: base(device)
		{
			DescriptorType = DescriptorType.Device;
			Device = device;
			foreach (var dev in Device.GKReflectionItem.Devices)
			{
				Device.LinkToDescriptor(dev);
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
			foreach (var device in Device.GKReflectionItem.Devices)
			{
				Formula.AddGetWord(false, device);
				count++;
				if (count > 1)
				{
					Formula.Add(FormulaOperationType.OR);
				}
			}
			count = 0;
			foreach (var device in Device.GKReflectionItem.Devices)
			{
				Formula.AddGetWord(true, device);
				count++;
				if (count > 1)
				{
					Formula.Add(FormulaOperationType.OR);
				}
			}

			Formula.Add(FormulaOperationType.CONST, 0, 0x400);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddPutWord(true, Device);
			Formula.AddPutWord(false, Device);
		}
	}
}
