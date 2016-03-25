using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;

namespace GKProcessor
{
	public static class DescriptorHelper
	{
		/// <summary>
		/// Метод не для IsCardOrCodeReader
		/// </summary>
		public static int AddFireOrFailureLogic(IEnumerable<GKDevice> guardDevices, FormulaBuilder Formula)
		{
			var count = 0;
			foreach (var guardDevice in guardDevices)
			{
				Formula.AddGetBit(GKStateBit.Fire1, guardDevice);
				if (count > 0)
					Formula.Add(FormulaOperationType.OR);
				
				Formula.AddGetBit(GKStateBit.Fire2, guardDevice);
				Formula.Add(FormulaOperationType.OR);
				Formula.AddGetBit(GKStateBit.Failure, guardDevice);
				Formula.Add(FormulaOperationType.OR);

				count++;
			}
			return count;
		}

		public static IEnumerable<GKGuardZoneDevice> GetGuardZoneDevices(GKGuardZone guardZone, GKGuardZoneDeviceActionType actionType, bool IsCardOrCodeReader)
		{
			if (!IsCardOrCodeReader)
				return guardZone.GuardZoneDevices.FindAll(x => x.ActionType == actionType && !x.Device.Driver.IsCardReaderOrCodeReader);
			
			if (actionType == GKGuardZoneDeviceActionType.SetGuard)
				return guardZone.GuardZoneDevices.FindAll(x => x.CodeReaderSettings.SetGuardSettings.CanBeUsed);
			if (actionType == GKGuardZoneDeviceActionType.ResetGuard)
				return guardZone.GuardZoneDevices.FindAll(x => x.CodeReaderSettings.ResetGuardSettings.CanBeUsed);
			if (actionType == GKGuardZoneDeviceActionType.ChangeGuard)
				return guardZone.GuardZoneDevices.FindAll(x => x.CodeReaderSettings.ChangeGuardSettings.CanBeUsed);
			if (actionType == GKGuardZoneDeviceActionType.SetAlarm)
				return guardZone.GuardZoneDevices.FindAll(x => x.CodeReaderSettings.AlarmSettings.CanBeUsed);

			return new List<GKGuardZoneDevice>();
		}
		public static int AddCodeReadersLogic(IEnumerable<GKGuardZoneDevice> guardDevices, FormulaBuilder Formula, GKStateBit commandStateBit)
		{
			var count = 0;
			foreach (var guardDevice in guardDevices)
			{
				if (commandStateBit == GKStateBit.TurnOnNow_InAutomatic || commandStateBit == GKStateBit.TurnOn_InAutomatic)
					FormulaHelper.AddCodeReaderLogic(Formula, guardDevice.CodeReaderSettings.SetGuardSettings, guardDevice.Device);
				if (commandStateBit == GKStateBit.TurnOffNow_InAutomatic || commandStateBit == GKStateBit.TurnOff_InAutomatic)
					FormulaHelper.AddCodeReaderLogic(Formula, guardDevice.CodeReaderSettings.ResetGuardSettings, guardDevice.Device);
				if (commandStateBit == GKStateBit.Fire1)
					FormulaHelper.AddCodeReaderLogic(Formula, guardDevice.CodeReaderSettings.AlarmSettings, guardDevice.Device);
				if (count > 0)
				{
					Formula.Add(FormulaOperationType.OR);
				}
				count++;
			}
			return count;
		}

		public static void AddChangeLogic(List<GKGuardZoneDevice> guardZoneDevices, FormulaBuilder Formula)
		{
			var changeGuardDevices1 = guardZoneDevices.FindAll(x => !x.Device.Driver.IsCardReaderOrCodeReader);
			var changeGuardDevices2 = guardZoneDevices.FindAll(x => x.Device.Driver.IsCardReaderOrCodeReader);
			if (changeGuardDevices1.Count > 0)
				DescriptorHelper.AddFireOrFailureLogic(changeGuardDevices1.Select(x => x.Device), Formula);
			int count = 0;
			if (changeGuardDevices2.Count > 0)
				foreach (var guardDevice in changeGuardDevices2)
				{
					FormulaHelper.AddCodeReaderLogic(Formula, guardDevice.CodeReaderSettings.ChangeGuardSettings, guardDevice.Device);
					if (count > 0)
					{
						Formula.Add(FormulaOperationType.OR);
					}
					count++;
				}
			if (changeGuardDevices1.Count > 0 && changeGuardDevices2.Count > 0)
				Formula.Add(FormulaOperationType.OR);
		}
	}
}
