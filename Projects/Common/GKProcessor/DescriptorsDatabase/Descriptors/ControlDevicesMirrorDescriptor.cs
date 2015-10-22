using RubezhAPI.GK;

namespace GKProcessor
{
	public class ControlDevicesMirrorDescriptor : BaseDescriptor
	{
		GKDevice Device { get; set; }
		GKReflectionItem ReflectionItem { get; set; }

		public ControlDevicesMirrorDescriptor(GKDevice device)
			: base(device)
		{
			DescriptorType = DescriptorType.Device;
			Device = device;
			ReflectionItem = device.GKReflectionItem;
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
			if ((DatabaseType == DatabaseType.Gk && GKBase.IsLogicOnKau) ||
				(DatabaseType == DatabaseType.Kau && !GKBase.IsLogicOnKau))
			{
				Formula.Add(FormulaOperationType.END);
				return;
			}

			var clauseGroup = new GKClauseGroup();
			var clause = new GKClause();
			clauseGroup.Clauses.Add(clause);
			clause.Devices = ReflectionItem.Devices;
			clause.ClauseOperationType = ClauseOperationType.AnyDevice;
			clause.StateType = GKStateBit.Ignore;

			Formula.AddClauseFormula(clauseGroup);
			Formula.AddPutBit(GKStateBit.Ignore, Device);

			clause.StateType = GKStateBit.Stop_InManual;
			Formula.AddClauseFormula(clauseGroup);
			Formula.AddPutBit(GKStateBit.Stop_InManual, Device);

			clause.StateType = GKStateBit.TurningOn;
			Formula.AddClauseFormula(clauseGroup);
			Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);

			clause.StateType = GKStateBit.On;
			Formula.AddClauseFormula(clauseGroup);
			Formula.AddPutBit(GKStateBit.TurnOnNow_InAutomatic, Device);

			clause.StateType = GKStateBit.TurningOff;
			Formula.AddClauseFormula(clauseGroup);
			Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Device);

			clause.StateType = GKStateBit.Off;
			Formula.AddClauseFormula(clauseGroup);
			Formula.AddPutBit(GKStateBit.TurnOffNow_InAutomatic, Device);
			
			Formula.Add(FormulaOperationType.END);
		}
	}
}
