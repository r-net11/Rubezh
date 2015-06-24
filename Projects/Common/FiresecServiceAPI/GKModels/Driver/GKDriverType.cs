using System.ComponentModel;
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
		Obsolete_KAU,
		[Description("АЛС")]
		Obsolete_KAU_Shleif,
		[Description("Индикатор Неисправность КАУ")]
		KAUIndicator,
		[Description("ИП-64")]
		Obsolete_SmokeDetector,
		[Description("ИП-29")]
		Obsolete_HeatDetector,
		[Description("ИП-212")]
		Obsolete_CombinedDetector,
		[Description("ИПР")]
		Obsolete_HandDetector,
		[Description("АМ-1")]
		Obsolete_AM_1,
		[Description("АМП")]
		Obsolete_AMP_1,
		[Description("АМ1-Т")]
		Obsolete_AM1_T,
		[Description("РМ-1")]
		Obsolete_RM_1,
		[Description("МРО-2М")]
		Obsolete_MRO_2,
		[Description("МДУ-1")]
		Obsolete_MDU,
		[Description("МПТ-1")]
		Obsolete_MPT,
		[Description("ШУЗ")]
		Obsolete_Valve,
		[Description("Пожарный насос")]
		Obsolete_FirePump,
		[Description("Жокей насос")]
		Obsolete_JockeyPump,
		[Description("Дренажный насос")]
		Obsolete_DrainagePump,
		[Description("РМ-2")]
		Obsolete_RM_2,
		[Description("РМ-3")]
		Obsolete_RM_3,
		[Description("РМ-4")]
		Obsolete_RM_4,
		[Description("РМ-5")]
		Obsolete_RM_5,
		[Description("АМ-4")]
		Obsolete_AM_4,
		[Description("АМП-4")]
		Obsolete_AMP_4,
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
		[Description("МДУ24 R2")]
		RSR2_MDU24,
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
		Obsolete_Battery,
		[Description("ШУ")]
		Obsolete_Shu,
		[Description("ШУВ")]
		Obsolete_Shuv,
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
		[Description("Отражение ГК")]
		RSR2_GKMirror,
		[Description("Отражение")]
		RSR2_GKMirrorItem,
		[Description("Группа индикаторов")]
		GKIndicatorsGroup,
		[Description("Группа реле")]
		GKRelaysGroup
	}
}