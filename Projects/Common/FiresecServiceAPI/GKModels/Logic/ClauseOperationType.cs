﻿using System.ComponentModel;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Тип операции
	/// </summary>
	public enum ClauseOperationType
	{
		[DescriptionAttribute("в любой зоне из")]
		AnyZone,

		[DescriptionAttribute("во всех зонах из")]
		AllZones,

		[DescriptionAttribute("в любом устройстве из")]
		AnyDevice,

		[DescriptionAttribute("во всех устройствах из")]
		AllDevices,

		[DescriptionAttribute("в любом направлении из")]
		AnyDirection,

		[DescriptionAttribute("во всех направлениях из")]
		AllDirections,

		[DescriptionAttribute("в любом МПТ из")]
		AnyMPT,

		[DescriptionAttribute("во всех МПТ из")]
		AllMPTs,

		[DescriptionAttribute("в любой задержке из")]
		AnyDelay,

		[DescriptionAttribute("во всех задержках из")]
		AllDelays,

		[DescriptionAttribute("в любой охранной зоне из")]
		AnyGuardZone,

		[DescriptionAttribute("во всех охранных зонах из")]
		AllGuardZones,

		[DescriptionAttribute("в любой ТД из")]
		AnyDoor,

		[DescriptionAttribute("во всех ТД из")]
		AllDoors,

		[DescriptionAttribute("в любой НС из")]
		AnyPumpStation,

		[DescriptionAttribute("во всех НС из")]
		AllPumpStations,

		[DescriptionAttribute("в любом индикаторе из")]
		AnyIndicator,

		[DescriptionAttribute("во всех индикаторах из")]
		AllIndicators
	}
}