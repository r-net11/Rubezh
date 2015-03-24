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

		public GuardZonePimDescriptor(GKGuardZone pimGuardZone, List<GKGuardZoneDevice> guardZoneDevices, DatabaseType databaseType)
			: base(pimGuardZone.Pim, databaseType)
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

			Formula.Add(FormulaOperationType.DUP);
			Formula.AddGetBit(GKStateBit.On, Pim, DatabaseType);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddPutBit(GKStateBit.TurnOffNow_InAutomatic, Pim, DatabaseType);
			Formula.AddGetBit(GKStateBit.Off, Pim, DatabaseType);
			Formula.Add(FormulaOperationType.AND);
			SetGuardZoneChangeLogic(PimGuardZone, GuardZoneDevices, Formula, DatabaseType);
			Formula.AddPutBit(GKStateBit.TurnOnNow_InAutomatic, Pim, DatabaseType);
			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}

		public static void SetGuardZoneChangeLogic(GKGuardZone pimGuardZone, List<GKGuardZoneDevice> guardZoneDevices, FormulaBuilder formula, DatabaseType databaseType)
		{
			var count = 0;
			foreach (var guardDevice in guardZoneDevices)
			{
				if (guardDevice.Device.DriverType == GKDriverType.RSR2_CodeReader || guardDevice.Device.DriverType == GKDriverType.RSR2_CardReader)
				{
					var settingsPart = guardDevice.CodeReaderSettings.ChangeGuardSettings;
					var stateBit = CodeReaderEnterTypeToStateBit(settingsPart.CodeReaderEnterType);

					switch (pimGuardZone.GuardZoneEnterMethod)
					{
						case GKGuardZoneEnterMethod.GlobalOnly:
							var codesCount = 0;
							foreach (var codeUID in settingsPart.CodeUIDs)
							{
								var code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == settingsPart.CodeUIDs.FirstOrDefault());
								if (code != null)
								{
									formula.Add(FormulaOperationType.KOD, 0, databaseType == DatabaseType.Gk ? guardDevice.Device.GKDescriptorNo : guardDevice.Device.KAUDescriptorNo);
									formula.Add(FormulaOperationType.CMPKOD, 1, databaseType == DatabaseType.Gk ? code.GKDescriptorNo : code.KAUDescriptorNo);
								}
								if (codesCount > 0)
								{
									formula.Add(FormulaOperationType.OR);
								}
								codesCount++;
							}
							break;

						case GKGuardZoneEnterMethod.UserOnly:
							formula.Add(FormulaOperationType.ACS, (byte)pimGuardZone.SetGuardLevel, guardDevice.Device.GKDescriptorNo);
							break;

						case GKGuardZoneEnterMethod.Both:
							codesCount = 0;
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

							formula.Add(FormulaOperationType.ACS, (byte)pimGuardZone.SetGuardLevel, guardDevice.Device.GKDescriptorNo);
							formula.Add(FormulaOperationType.OR);
							break;
					}

					formula.AddGetBit(stateBit, guardDevice.Device, databaseType);
					formula.Add(FormulaOperationType.AND);
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