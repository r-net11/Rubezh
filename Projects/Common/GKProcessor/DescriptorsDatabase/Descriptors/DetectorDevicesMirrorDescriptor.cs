using RubezhAPI.GK;

namespace GKProcessor
{
	public class DetectorDevicesMirrorDescriptor : BaseDescriptor
	{
		GKDevice Device { get; set; }
		public DetectorDevicesMirrorPimDescriptor DetectorDevicesMirrorPimDescriptor { get; private set; }

		public DetectorDevicesMirrorDescriptor(GKDevice device)
			: base(device)
		{
			DescriptorType = DescriptorType.Device;
			Device = device;
			DetectorDevicesMirrorPimDescriptor = new DetectorDevicesMirrorPimDescriptor(device);
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

		}
	}

	public class DetectorDevicesMirrorPimDescriptor : PimDescriptor
	{
		private GKDevice Device { get; set; }

		public DetectorDevicesMirrorPimDescriptor(GKDevice device)
			: base(device.DetectorDevicesMirrorPim)
		{
			Device = device;
			foreach (var detectorDevice in Device.GKReflectionItem.Devices)
			{
				Formula.AddGetBit(GKStateBit.Failure, detectorDevice);
				Pim.LinkToDescriptor(detectorDevice);
			}
			Pim.LinkToDescriptor(Pim);
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes(0x107);
			SetAddress(0);
			SetFormulaBytes();
		}

		private void SetFormulaBytes()
		{
			Formula = new FormulaBuilder();

			var clauseGroup = new GKClauseGroup();
			var clause = new GKClause();
			clauseGroup.Clauses.Add(clause);
			clause.Devices = Device.GKReflectionItem.Devices;
			clause.ClauseOperationType = ClauseOperationType.AnyDevice;
			clause.StateType = GKStateBit.Ignore;

			Formula.AddClauseFormula(clauseGroup);
			Formula.AddPutBit(GKStateBit.Ignore, Device);

			clause.StateType = GKStateBit.Off;
			Formula.AddClauseFormula(clauseGroup);
			Formula.AddPutBit(GKStateBit.TurnOffNow_InAutomatic, Device);

			clause.StateType = GKStateBit.Fire1;
			Formula.AddClauseFormula(clauseGroup);
			Formula.AddPutBit(GKStateBit.Fire1, Device);

			clause.StateType = GKStateBit.Norm;
			Formula.AddClauseFormula(clauseGroup);
			Formula.AddPutBit(GKStateBit.Norm, Device);

			Formula.Add(FormulaOperationType.END);
		}
	}
}
