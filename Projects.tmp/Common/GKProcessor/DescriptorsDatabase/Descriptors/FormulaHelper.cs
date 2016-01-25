using RubezhAPI.GK;
using RubezhAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKProcessor
{
	public static class FormulaHelper
	{
		public static void AddCodeReaderLogic(FormulaBuilder formula, GKCodeReaderSettingsPart settingsPart, GKDevice device)
		{
			var stateBit = CodeReaderEnterTypeToStateBit(settingsPart.CodeReaderEnterType);
			formula.AddGetBit(stateBit, device);
			formula.Add(FormulaOperationType.BR, 2, 2);
			formula.Add(FormulaOperationType.CONST);
			var gotoFormulaOperation = formula.Add(FormulaOperationType.BR, 0, 0);
			var formulaNo = formula.FormulaOperations.Count;

			var codeIndex = 0;
			foreach (var codeUID in settingsPart.CodeUIDs)
			{
				var code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == codeUID);
				formula.AddWithGKBase(FormulaOperationType.KOD, 0, device);
				formula.AddWithGKBase(FormulaOperationType.CMPKOD, 1, code);
				if (codeIndex > 0)
				{
					formula.Add(FormulaOperationType.OR);
				}
				codeIndex++;
			}
			if (settingsPart.AccessLevel > 0)
			{
				formula.AddWithGKBase(FormulaOperationType.ACS, (byte)settingsPart.AccessLevel, device);
				if (codeIndex > 0)
				{
					formula.Add(FormulaOperationType.OR);
				}
			}

			gotoFormulaOperation.SecondOperand = (ushort)(formula.FormulaOperations.Count - formulaNo);
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