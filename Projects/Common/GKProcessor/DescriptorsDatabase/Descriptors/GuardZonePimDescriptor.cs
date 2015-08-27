using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;

namespace GKProcessor
{
	public class GuardZonePimDescriptor : PimDescriptor
	{
		public List<GKGuardZoneDevice> GuardZoneDevices { get; private set; }
		public GKGuardZone PimGuardZone { get; private set; }

		public GuardZonePimDescriptor(GKGuardZone pimGuardZone, List<GKGuardZoneDevice> guardZoneDevices)
			: base(pimGuardZone.Pim)
		{
			PimGuardZone = pimGuardZone;
			GuardZoneDevices = guardZoneDevices;
			foreach (var guardDevice in GuardZoneDevices)
			{
				if (Pim != null)
					Pim.LinkToDescriptor(guardDevice.Device);
			}
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes(0x107);
			SetAddress(0);
			SetFormulaBytes();
		}

		void SetFormulaBytes()
		{
			Formula = new FormulaBuilder();
			if ((DatabaseType == DatabaseType.Gk && GKBase.IsLogicOnKau) || (DatabaseType == DatabaseType.Kau && !GKBase.IsLogicOnKau))
			{
				Formula.Add(FormulaOperationType.END);
				return;
			}

			SetGuardZoneChangeLogic(PimGuardZone, GuardZoneDevices, Formula, true);

			Formula.Add(FormulaOperationType.DUP);
			Formula.AddGetBit(GKStateBit.On, PimGuardZone);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddPutBit(GKStateBit.TurnOffNow_InAutomatic, Pim);
			Formula.AddGetBit(GKStateBit.Off, PimGuardZone);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddPutBit(GKStateBit.TurnOnNow_InAutomatic, Pim);
			Formula.Add(FormulaOperationType.END);
		}

		public static void SetGuardZoneChangeLogic(GKGuardZone pimGuardZone, List<GKGuardZoneDevice> guardZoneDevices, FormulaBuilder formula, bool isPim = false)
		{
			var count = 0;
			foreach (var guardDevice in guardZoneDevices)
			{
				if (guardDevice.Device.DriverType == GKDriverType.RSR2_CodeReader || guardDevice.Device.DriverType == GKDriverType.RSR2_CardReader)
				{
					var settingsPart = guardDevice.CodeReaderSettings.ChangeGuardSettings;

					var stateBit = CodeReaderEnterTypeToStateBit(settingsPart.CodeReaderEnterType);
					formula.AddGetBit(stateBit, guardDevice.Device);
					formula.Add(FormulaOperationType.BR, 2, 2);
					formula.Add(FormulaOperationType.CONST);
					if (isPim)
						formula.Add(FormulaOperationType.BR, 0, 2);
					else
						formula.Add(FormulaOperationType.BR, 0, 2);
					var formulaNo = formula.FormulaOperations.Count;

					if (settingsPart.CodeUIDs.Count > 0 && settingsPart.AccessLevel == 0)
					{
						var codesCount = 0;
						foreach (var codeUID in settingsPart.CodeUIDs)
						{
							var code =
								GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == settingsPart.CodeUIDs.FirstOrDefault());
							if (code != null)
							{
								formula.Add(FormulaOperationType.KOD, 0, 0, guardDevice.Device);
								formula.Add(FormulaOperationType.CMPKOD, 1, 0, code);
							}
							if (codesCount > 0)
							{
								formula.Add(FormulaOperationType.OR);
							}
							codesCount++;
						}
					}

					if (settingsPart.CodeUIDs.Count > 0 && settingsPart.AccessLevel > 0)
					{
						var codesCount = 0;
						foreach (var codeUID in settingsPart.CodeUIDs)
						{
							var code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == settingsPart.CodeUIDs.FirstOrDefault());
							if (code != null)
							{
								formula.Add(FormulaOperationType.KOD, 0, guardDevice.Device.GKDescriptorNo);
								formula.Add(FormulaOperationType.CMPKOD, 1, code.GKDescriptorNo);
							}
							if (codesCount > 0)
							{
								formula.Add(FormulaOperationType.OR);
							}
							codesCount++;
						}
					}

					if (count > 0)
					{
						formula.Add(FormulaOperationType.OR);
					}
					
					//gotoFormulaOperation.SecondOperand = (ushort)(formula.FormulaOperations.Count - formulaNo + (isPim ? 4 : 3));
				}
				else
				{
					formula.AddGetBit(GKStateBit.Fire1, guardDevice.Device);
					if (count > 0)
					{
						formula.Add(FormulaOperationType.OR);
					}
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