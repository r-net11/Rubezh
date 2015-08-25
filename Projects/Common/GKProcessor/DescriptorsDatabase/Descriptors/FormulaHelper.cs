using FiresecAPI.GK;
using FiresecClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKProcessor
{
	public static class FormulaHelper
	{
		public static bool AddCodeReaderLogic(FormulaBuilder formula, GKCodeReaderSettingsPart settingsPart, GKDevice device)
		{
			if (settingsPart.CodeReaderEnterType == GKCodeReaderEnterType.None || (settingsPart.CodeUIDs.Count == 0 && settingsPart.AccessLevel == 0))
				return false;

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
				formula.Add(FormulaOperationType.KOD, 0, 0, device);
				formula.Add(FormulaOperationType.CMPKOD, 1, 0, code);
				if (codeIndex > 0)
				{
					formula.Add(FormulaOperationType.OR);
				}
				codeIndex++;
			}
			if (settingsPart.AccessLevel > 0)
			{
				formula.Add(FormulaOperationType.ACS, (byte)settingsPart.AccessLevel, 0, device);
				if (codeIndex > 0)
				{
					formula.Add(FormulaOperationType.OR);
				}
			}

			gotoFormulaOperation.SecondOperand = (ushort)(formula.FormulaOperations.Count - formulaNo);
			return true;
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