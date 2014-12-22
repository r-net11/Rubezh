using System.Linq;
using FiresecAPI.GK;
using System.Collections.Generic;
using FiresecClient;

namespace GKProcessor
{
	public class GuardZonePimDescriptor : PimDescriptor
	{
		public List<GKGuardZoneDevice> GuardZoneDevices { get; private set; }
		public GKGuardZone PimGuardZone { get; private set; }

		public GuardZonePimDescriptor(GKGuardZone pimGuardZone, List<GKGuardZoneDevice> guardZoneDevices, DatabaseType databaseType) : base(pimGuardZone.Pim, databaseType)
		{
			PimGuardZone = pimGuardZone;
			GuardZoneDevices = guardZoneDevices;
			foreach (var guardDevice in GuardZoneDevices)
			{
				Pim.LinkGKBases(guardDevice.Device);
			}
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((ushort)0x107);
			SetAddress((ushort)0);
			SetFormulaBytes();
		}

		void SetFormulaBytes()
		{
			Formula = new FormulaBuilder();
			if ((DatabaseType == DatabaseType.Gk && GKBase.IsLogicOnKau) || (DatabaseType == DatabaseType.Kau && !GKBase.IsLogicOnKau))
			{
				Formula.Add(FormulaOperationType.END);
				FormulaBytes = Formula.GetBytes();
				return;
			}

			SetGuardZoneChangeLogic(PimGuardZone, GuardZoneDevices, Formula, DatabaseType);

			//var count = 0;
			//foreach (var guardDevice in GuardZoneDevices)
			//{
			//    if (guardDevice.Device.DriverType == GKDriverType.RSR2_CodeReader)
			//    {
			//        var settingsPart = guardDevice.CodeReaderSettings.ChangeGuardSettings;
			//        var stateBit = CodeReaderEnterTypeToStateBit(settingsPart.CodeReaderEnterType);
			//        var code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == settingsPart.CodeUIDs.FirstOrDefault());

			//        Formula.AddGetBit(stateBit, guardDevice.Device, DatabaseType.Gk);
			//        switch (PimGuardZone.GuardZoneEnterMethod)
			//        {
			//            case GKGuardZoneEnterMethod.GlobalOnly:
			//                Formula.Add(FormulaOperationType.BR, 1, 3);
			//                Formula.Add(FormulaOperationType.KOD, 0, DatabaseType == DatabaseType.Gk ? guardDevice.Device.GKDescriptorNo : guardDevice.Device.KAUDescriptorNo);
			//                Formula.Add(FormulaOperationType.CMPKOD, 1, DatabaseType == DatabaseType.Gk ? code.GKDescriptorNo : code.KAUDescriptorNo);
			//                break;

			//            case GKGuardZoneEnterMethod.UserOnly:
			//                Formula.Add(FormulaOperationType.BR, 1, 2);
			//                Formula.Add(FormulaOperationType.ACS, (byte)PimGuardZone.SetGuardLevel, guardDevice.Device.GKDescriptorNo);
			//                Formula.Add(FormulaOperationType.AND);
			//                break;

			//            case GKGuardZoneEnterMethod.Both:
			//                Formula.Add(FormulaOperationType.BR, 1, 5);
			//                Formula.Add(FormulaOperationType.KOD, 0, guardDevice.Device.GKDescriptorNo);
			//                Formula.Add(FormulaOperationType.CMPKOD, 1, code.GKDescriptorNo);
			//                Formula.Add(FormulaOperationType.ACS, (byte)PimGuardZone.SetGuardLevel, guardDevice.Device.GKDescriptorNo);
			//                Formula.Add(FormulaOperationType.OR);
			//                break;
			//        }
			//    }
			//    else
			//    {
			//        Formula.AddGetBit(GKStateBit.Fire1, guardDevice.Device, DatabaseType);
			//    }
			//    if (count > 0)
			//    {
			//        Formula.Add(FormulaOperationType.OR);
			//    }
			//    count++;
			//}
			Formula.Add(FormulaOperationType.DUP);
			Formula.AddGetBit(GKStateBit.On, Pim, DatabaseType);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddPutBit(GKStateBit.TurnOffNow_InAutomatic, Pim, DatabaseType);
			Formula.AddGetBit(GKStateBit.Off, Pim, DatabaseType);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddPutBit(GKStateBit.TurnOnNow_InAutomatic, Pim, DatabaseType);
			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}

		public static void SetGuardZoneChangeLogic(GKGuardZone pimGuardZone, List<GKGuardZoneDevice> guardZoneDevices, FormulaBuilder formula, DatabaseType databaseType)
		{
			var count = 0;
			foreach (var guardDevice in guardZoneDevices)
			{
				if (guardDevice.Device.DriverType == GKDriverType.RSR2_CodeReader)
				{
					var settingsPart = guardDevice.CodeReaderSettings.ChangeGuardSettings;
					var stateBit = CodeReaderEnterTypeToStateBit(settingsPart.CodeReaderEnterType);
					var code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == settingsPart.CodeUIDs.FirstOrDefault());

					formula.AddGetBit(stateBit, guardDevice.Device, DatabaseType.Gk);
					switch (pimGuardZone.GuardZoneEnterMethod)
					{
						case GKGuardZoneEnterMethod.GlobalOnly:
							formula.Add(FormulaOperationType.BR, 1, 3);
							formula.Add(FormulaOperationType.KOD, 0, databaseType == DatabaseType.Gk ? guardDevice.Device.GKDescriptorNo : guardDevice.Device.KAUDescriptorNo);
							formula.Add(FormulaOperationType.CMPKOD, 1, databaseType == DatabaseType.Gk ? code.GKDescriptorNo : code.KAUDescriptorNo);
							break;

						case GKGuardZoneEnterMethod.UserOnly:
							formula.Add(FormulaOperationType.BR, 1, 2);
							formula.Add(FormulaOperationType.ACS, (byte)pimGuardZone.SetGuardLevel, guardDevice.Device.GKDescriptorNo);
							formula.Add(FormulaOperationType.AND);
							break;

						case GKGuardZoneEnterMethod.Both:
							formula.Add(FormulaOperationType.BR, 1, 5);
							formula.Add(FormulaOperationType.KOD, 0, guardDevice.Device.GKDescriptorNo);
							formula.Add(FormulaOperationType.CMPKOD, 1, code.GKDescriptorNo);
							formula.Add(FormulaOperationType.ACS, (byte)pimGuardZone.SetGuardLevel, guardDevice.Device.GKDescriptorNo);
							formula.Add(FormulaOperationType.OR);
							break;
					}
				}
				else
				{
					formula.AddGetBit(GKStateBit.Fire1, guardDevice.Device, databaseType);
				}
				if (count > 0)
				{
					formula.Add(FormulaOperationType.OR);
				}
				count++;
			}
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