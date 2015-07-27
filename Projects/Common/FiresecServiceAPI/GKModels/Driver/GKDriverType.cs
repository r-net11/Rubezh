using System.ComponentModel;

namespace FiresecAPI.GK
{
	public enum GKDriverType
	{
		[Description("Локальная сеть")]
		System = 0,

		[Description("ГК")]
		GK = 1,

		[Description("Индикатор ГК")]
		GKIndicator = 2,

		[Description("Линия ГК")]
		GKLine = 3,

		[Description("Реле ГК")]
		GKRele = 4,

		[Description("Индикатор Неисправность КАУ")]
		KAUIndicator = 7,
		
		[Description("КАУ RSR2")]
		RSR2_KAU = 29,

		[Description("АЛС")]
		RSR2_KAU_Shleif = 30,

		[Description("ИПР R2")]
		RSR2_HandDetector = 31,

		[Description("ИПД R2")]
		RSR2_SmokeDetector = 32,

		[Description("ИПК R2")]
		RSR2_CombinedDetector = 33,

		[Description("МР R2")]
		RSR2_RM_1 = 35,

		[Description("МР-2 R2")]
		RSR2_RM_2 = 36,

		[Description("МА R2")]
		RSR2_AM_1 = 37,

		[Description("МА-4 R2")]
		RSR2_AM_4 = 38,

		[Description("МА-2 R2")]
		RSR2_AM_2 = 39,

		[Description("МДУ-1 R2")]
		RSR2_MDU = 40,

		[Description("МДУ24 R2")]
		RSR2_MDU24 = 41,

		[Description("МАП R2")]
		RSR2_MAP4 = 42,

		[Description("МВК R2")]
		RSR2_MVK8 = 43,

		[Description("МАП-4 R2")]
		RSR2_MAP4_Group = 44,

		[Description("МВК-8 R2")]
		RSR2_MVK8_Group = 45,

		[Description("ИПТ R2")]
		RSR2_HeatDetector = 46,

		[Description("ППУ ДН R2")]
		RSR2_Bush_Drenazh = 47,

		[Description("ППУ ЖН R2")]
		RSR2_Bush_Jokey = 48,

		[Description("ППУ ПН R2")]
		RSR2_Bush_Fire = 49,

		[Description("ППУ В R2")]
		RSR2_Bush_Shuv = 50,

		[Description("БУЗ КВ R2")]
		RSR2_Valve_KV = 51,

		[Description("БУЗ КВ-МВ R2")]
		RSR2_Valve_KVMV = 52,

		[Description("БУЗ КВ-ДУ R2")]
		RSR2_Valve_DU = 53,

		[Description("ОПК-R2")]
		RSR2_OPK = 57,

		[Description("ОПС-R2")]
		RSR2_OPS = 58,

		[Description("ОПЗ-R2")]
		RSR2_OPZ = 59,

		[Description("МВП R2")]
		RSR2_MVP = 60,

		[Description("Линия МВП")]
		RSR2_MVP_Part = 61,

		[Description("НК R2")]
		RSR2_CodeReader = 62,

		[Description("ИО-ИК R2")]
		RSR2_GuardDetector = 63,

		[Description("Контроллер Wiegand")]
		RSR2_CardReader = 64,

		[Description("МР-4 R2")]
		RSR2_RM_4 = 65,

		[Description("Буз КВ R2")]
		RSR2_Buz_KV = 66,

		[Description("Буз КВ-МВ R2")]
		RSR2_Buz_KVMV = 67,

		[Description("Буз КВ-ДУ R2")]
		RSR2_Buz_KVDU = 68,

		[Description("ИО-ПЗ R2")]
		RSR2_GuardDetectorSound = 69,

		[Description("Отражение ГК")]
		RSR2_GKMirror = 70,

		[Description("Извещательные устройства")]
		RSR2_GKMirrorDetectorsDevice = 71,

		[Description("Пожарные зоны")]
		RSR2_GKMirrorFireZone = 72,

		[Description("Исполнительные устройства")]
		RSR2_GKMirrorPerformersDevice = 73,

		[Description("Зоны пожара тушения")]
		RSR2_GKMirrorFightFireZone = 74,

		[Description("Охранные зоны")]
		RSR2_GKMirrorGuardZone = 75,

		[Description("Направления")]
		RSR2_GKMirrorDirection = 76,

		[Description("Группа индикаторов")]
		GKIndicatorsGroup = 77,

		[Description("Группа реле")]
		GKRelaysGroup = 78
	}
}