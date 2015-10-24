using RubezhAPI.GK;

namespace GKProcessor
{
	public class DetectorDevicesMirrorDescriptor : BaseDescriptor
	{
		GKDevice Device { get; set; }

		public DetectorDevicesMirrorDescriptor(GKDevice device)
			: base(device)
		{
			DescriptorType = DescriptorType.Device;
			Device = device;
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
			foreach (var device in Device.GKReflectionItem.Devices)
			{
				
			}
		}
	}
}
