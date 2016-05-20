using System.ComponentModel;

namespace RubezhAPI.GK
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
		
		[Description("КАУ RS")]
		RSR2_KAU = 29,

		[Description("АЛС")]
		RSR2_KAU_Shleif = 30,

		[Description("ИПР")]
		RSR2_HandDetector = 31,

		[Description("ИПД")]
		RSR2_SmokeDetector = 32,

		[Description("ИПК")]
		RSR2_CombinedDetector = 33,

		[Description("МР")]
		RSR2_RM_1 = 35,

		[Description("МР-2")]
		RSR2_RM_2 = 36,

		[Description("МА")]
		RSR2_AM_1 = 37,

		[Description("МА-4")]
		RSR2_AM_4 = 38,

		[Description("МА-2")]
		RSR2_AM_2 = 39,

		[Description("МДУ-1")]
		RSR2_MDU = 40,

		[Description("МДУ24")]
		RSR2_MDU24 = 41,

		[Description("МАП")]
		RSR2_MAP4 = 42,

		[Description("МВК")]
		RSR2_MVK8 = 43,

		[Description("МАП-4")]
		RSR2_MAP4_Group = 44,

		[Description("МВК-8")]
		RSR2_MVK8_Group = 45,

		[Description("ИПТ")]
		RSR2_HeatDetector = 46,

		[Description("ППУ ДН")]
		RSR2_Bush_Drenazh = 47,

		[Description("ППУ ЖН")]
		RSR2_Bush_Jokey = 48,

		[Description("ППУ ПН")]
		RSR2_Bush_Fire = 49,

		[Description("ППУ В")]
		RSR2_Bush_Shuv = 50,

		[Description("БУЗ КВ")]
		RSR2_Valve_KV = 51,

		[Description("БУЗ КВ-МВ")]
		RSR2_Valve_KVMV = 52,

		[Description("БУЗ КВ-ДУ")]
		RSR2_Valve_DU = 53,

		[Description("ОПК")]
		RSR2_OPK = 57,

		[Description("ОПС")]
		RSR2_OPS = 58,

		[Description("ОПЗ")]
		RSR2_OPZ = 59,

		[Description("МВП")]
		RSR2_MVP = 60,

		[Description("Линия МВП")]
		RSR2_MVP_Part = 61,

		[Description("НК")]
		RSR2_CodeReader = 62,

		[Description("ИО-ИК R2")]
		RSR2_GuardDetector = 63,

		[Description("Контроллер Wiegand")]
		RSR2_CardReader = 64,

		[Description("МР-4")]
		RSR2_RM_4 = 65,

		[Description("Буз КВ")]
		RSR2_Buz_KV = 66,

		[Description("Буз КВ-МВ")]
		RSR2_Buz_KVMV = 67,

		[Description("Буз КВ-ДУ")]
		RSR2_Buz_KVDU = 68,

		[Description("ИО-ПЗ")]
		RSR2_GuardDetectorSound = 69,

		[Description("Отражение ГК")]
		GKMirror = 70,

		[Description("Образ извещательных устройств")]
		DetectorDevicesMirror = 71,

		[Description("Образ пожарных зон")]
		FireZonesMirror = 72,

		[Description("Образ исполнительных устройств")]
		ControlDevicesMirror = 73,

		[Description("Образ зон пожаротушения")]
		FirefightingZonesMirror = 74,

		[Description("Образ охранных зон")]
		GuardZonesMirror = 75,

		[Description("Образ направлений")]
		DirectionsMirror = 76,

		[Description("Группа индикаторов")]
		GKIndicatorsGroup = 77,

		[Description("Группа реле")]
		GKRelaysGroup = 78,

		[Description("Оповещатель свето-звуковой")]
		RSR2_OPSZ = 79,

		[Description("Звуковой оповещатель комбинированный")]
		RSR2_OPKS = 80,

		[Description("Световой оповещатель комбинированный")]
		RSR2_OPKZ = 81,

		[Description("АБПЦ")]
		RSR2_ABPC = 82,

		[Description("АБШС")]
		RSR2_ABShS = 83,
		
		[Description("АБШС-2")]
		RSR2_ABShS_Group = 84,

		[Description("АБТК")]
		RSR2_ABTK = 85,

		[Description("АБТК-2")]
		RSR2_ABTK_Group = 86,

		[Description("ИПТЭ")]
		RSR2_HeatDetectorEridan = 87,

		[Description("ИПРЭ")]
		RSR2_HandDetectorEridan = 88,

		[Description("ИОЛИТ")]
		RSR2_IOLIT = 89,

		[Description("МРК")]
		RSR2_MRK = 90,

		[Description("ИПР RK")]
		RK_HandDetector = 91,

		[Description("ИПД RK")]
		RK_SmokeDetector = 92,

		[Description("ИПТ RK")]
		RK_HeatDetector = 93,

		[Description("РМ RK")]
		RK_RM = 94,

		[Description("АМ RK")]
		RK_AM = 95,

		[Description("ОПК RK")]
		RK_OPK = 96,

		[Description("ОПЗ RK")]
		RK_OPZ = 97,

		[Description("УДП")]
		RSR2_Button = 98,

		[Description("МВК-4")]
		RSR2_MVK4_Group = 99,

		[Description("МВК-2")]
		RSR2_MVK2_Group = 100,

		[Description("НСЧ")]
		RSR2_CodeCardReader = 101,

		[Description("ЗОВ")]
		RSR2_ZOV = 102,

		[Description("СКОПА")]
		RSR2_SCOPA = 103,

		[Description("Контроллер доступа")]
		RSR2_KDKR = 104,
		
		[Description("Линия КД")]
		RSR2_KDKR_Part = 105,
		
		[Description("Замок КД")]
		KD_KDZ = 106,

		[Description("Концевик(кнопка) КД")]
		KD_KDK = 107,

		[Description("КВ КД")]
		KD_KDKV = 108,

		[Description("ТД КД")]
		KD_KDTD = 109,
		
		[Description("ИОВ")]
		RSR2_HandGuardDetector = 110,

		[Description("ИС")]
		RSR2_IS = 111
	}
}