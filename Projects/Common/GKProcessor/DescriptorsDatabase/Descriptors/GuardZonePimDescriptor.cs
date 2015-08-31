using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;

namespace GKProcessor
{
	public class GuardZonePimDescriptor : PimDescriptor
	{
		public List<Tuple<GKDevice, GKCodeReaderSettingsPart>> GuardZoneDevices { get; private set; }
		public GKGuardZone PimGuardZone { get; private set; }

		public GuardZonePimDescriptor(GKGuardZone pimGuardZone, List<Tuple<GKDevice, GKCodeReaderSettingsPart>> guardZoneDevices)
			: base(pimGuardZone.Pim)
		{
			PimGuardZone = pimGuardZone;
			GuardZoneDevices = guardZoneDevices;
			foreach (var guardDevice in GuardZoneDevices)
			{
				if (Pim != null)
					Pim.LinkToDescriptor(guardDevice.Item1);
			}
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes(0x107);
			SetAddress(0);
		}

		public override void BuildFormula()
		{
			Formula = new FormulaBuilder();
			if ((DatabaseType == DatabaseType.Gk && GKBase.IsLogicOnKau) || (DatabaseType == DatabaseType.Kau && !GKBase.IsLogicOnKau))
			{
				Formula.Add(FormulaOperationType.END);
				return;
			}

			GuardZoneDescriptor.AddSettings(GuardZoneDevices, Formula, GKStateBit.No);
			Formula.AddGetBit(GKStateBit.Off, PimGuardZone);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddPutBit(GKStateBit.TurnOnNow_InAutomatic, Pim);
			GuardZoneDescriptor.AddSettings(GuardZoneDevices, Formula, GKStateBit.No);
			Formula.AddGetBit(GKStateBit.On, PimGuardZone);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddPutBit(GKStateBit.TurnOffNow_InAutomatic, Pim);
			Formula.Add(FormulaOperationType.END);
		}

		static GKStateBit CodeReaderEnterTypeToStateBit(GKCodeReaderEnterType codeReaderEnterType)
		{
			switch (codeReaderEnterType)
			{
				case GKCodeReaderEnterType.CodeOnly:
					return GKStateBit.Attention;

				case GKCodeReaderEnterType.CodeAndOne:
					return GKStateBit.Fire1;

				case GKCodeReaderEnterType.CodeAndTwo:
					return GKStateBit.Fire2;
			}
			return GKStateBit.Fire1;
		}
	}
}