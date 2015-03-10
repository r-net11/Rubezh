﻿using System.ComponentModel;
namespace FiresecAPI.GK
{
	public enum GKDriverType
	{
		[Description("Локальная сеть")]
		System,
		[Description("ГК")]
		GK,
		[Description("Индикатор ГК")]
		GKIndicator,
		[Description("Линия ГК")]
		GKLine,
		[Description("Реле ГК")]
		GKRele,
		[Description("КАУ")]
		KAU,
		[Description("АЛС")]
		KAU_Shleif,
		[Description("Индикатор Неисправность КАУ")]
		KAUIndicator,
		[Description("ИП-64")]
		SmokeDetector,
		[Description("ИП-29")]
		HeatDetector,
		[Description("ИП-212")]
		CombinedDetector,
		[Description("ИПР")]
		HandDetector,
		[Description("АМ-1")]
		AM_1,
		[Description("АМП")]
		AMP_1,
		[Description("АМ1-Т")]
		AM1_T,
		[Description("РМ-1")]
		RM_1,
		[Description("МРО-2М")]
		MRO_2,
		[Description("МДУ-1")]
		MDU,
		[Description("МПТ-1")]
		MPT,
		[Description("ШУЗ")]
		Valve,
		[Description("Пожарный насос")]
		FirePump,
		[Description("Жокей насос")]
		JockeyPump,
		[Description("Дренажный насос")]
		DrainagePump,
		[Description("РМ-2")]
		RM_2,
		[Description("РМ-3")]
		RM_3,
		[Description("РМ-4")]
		RM_4,
		[Description("РМ-5")]
		RM_5,
		[Description("АМ-4")]
		AM_4,
		[Description("АМП-4")]
		AMP_4,
		[Description("КАУ RSR2")]
		RSR2_KAU,
		[Description("АЛС")]
		RSR2_KAU_Shleif,
		[Description("ИПР R2")]
		RSR2_HandDetector,
		[Description("ИПД R2")]
		RSR2_SmokeDetector,
		[Description("ИПК R2")]
		RSR2_CombinedDetector,
		[Description("ИП2Д R2")]
		RSR2_SmokeDetector2,
		[Description("МР R2")]
		RSR2_RM_1,
		[Description("МР-2 R2")]
		RSR2_RM_2,
		[Description("МА R2")]
		RSR2_AM_1,
		[Description("МА-4 R2")]
		RSR2_AM_4,
		[Description("МА-2 R2")]
		RSR2_AM_2,
		[Description("МДУ-1 R2")]
		RSR2_MDU,
		[Description("МАП R2")]
		RSR2_MAP4,
		[Description("МВК R2")]
		RSR2_MVK8,
		[Description("МАП-4 R2")]
		RSR2_MAP4_Group,
		[Description("МВК-8 R2")]
		RSR2_MVK8_Group,
		[Description("ИПТ R2")]
		RSR2_HeatDetector,
		[Description("ППУ ДН R2")]
		RSR2_Bush_Drenazh,
		[Description("ППУ ЖН R2")]
		RSR2_Bush_Jokey,
		[Description("ППУ ПН R2")]
		RSR2_Bush_Fire,
		[Description("ППУ В R2")]
		RSR2_Bush_Shuv,
		[Description("БУЗ КВ R2")]
		RSR2_Valve_KV,
		[Description("БУЗ КВ-МВ R2")]
		RSR2_Valve_KVMV,
		[Description("БУЗ КВ-ДУ R2")]
		RSR2_Valve_DU,
		[Description ("ИВЭПР")]
		Battery,
		[Description("ШУ")]
		Shu,
		[Description("ШУВ")]
		Shuv,
		[Description("ОПК-R2")]
		RSR2_OPK,
		[Description("ОПС-R2")]
		RSR2_OPS,
		[Description("ОПЗ-R2")]
		RSR2_OPZ,
		[Description("МВП R2")]
		RSR2_MVP,
		[Description("Линия МВП")]
		RSR2_MVP_Part,
		[Description("НК R2")]
		RSR2_CodeReader,
		[Description("ИО-ИК R2")]
		RSR2_GuardDetector,
		[Description("Контроллер Wiegand")]
		RSR2_CardReader,
		[Description("МР-4 R2")]
		RSR2_RM_4,
		[Description("Буз КВ R2")]
		RSR2_Buz_KV,
		[Description("Буз КВ-МВ R2")]
		RSR2_Buz_KVMV,
		[Description("Буз КВ-ДУ R2")]
		RSR2_Buz_KVDU,
		[Description("ИО-ПЗ R2")]
		RSR2_GuardDetectorSound,
	}
}