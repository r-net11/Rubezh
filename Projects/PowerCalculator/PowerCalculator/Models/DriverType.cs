using System.ComponentModel;

namespace PowerCalculator.Models
{
    public enum DriverType
    {
        [Description("Контроллер адресных устройств")]
        RSR2_KAU,

        [Description("Извещатель пожарный дымовой")]
        RSR2_SmokeDetector,

        [Description("Извещатель пожарный тепловой")]
        RSR2_HeatDetector,

        [Description("Извещатель пожарный комбинорованный")]
        RSR2_CombinedDetector,

        [Description("Извещатель пожарный ручной")]
        RSR2_HandDetector,

        [Description("Модуль дымоудаления (220В)")]
        RSR2_MDU,

        [Description("Модуль дымоудаления (24В)")]
        RSR2_MDU24,

        [Description("Метка адресная - 1")]
        RSR2_AM_1,

        [Description("Метка адресная - 2")]
        RSR2_AM_2,

        [Description("Метка адресная - 4")]
        RSR2_AM_4,

        [Description("Метка адресная с питанием - 4")]
        RSR2_MAP4,

		[Description("Модуль релейный - 1")]
		RSR2_RM_1,

        [Description("Модуль релейный - 2")]
        RSR2_RM_2,

        [Description("Модуль релейный - 4")]
        RSR2_RM_4,

        [Description("Модуль выходов с контрлоем")]
        RSR2_MVK8,

        [Description("Модуль подпитки")]
        RSR2_MP,

        [Description("Модуль ветвления и подпитки")]
        RSR2_MVP,

        [Description("Оповещатель световой")]
        RSR2_OPS,

        [Description("Оповещатель звуковой")]
        RSR2_OPZ,

        [Description("Оповещатель комбинированный")]
        RSR2_OPK,

        [Description("Контроллер виганд")]
        RSR2_Cardreader,

        [Description("Шкаф управления насосом или вентилятором")]
		RSR2_Bush_Shuv,

        [Description("Шкаф управления задвижкой")]
		RSR2_Valve_DU,

        [Description("Извещатель охранный инфракрасный (объемник)")]
        RSR2_GuardDetector,

        [Description("Извещатель охранный звуковой разбития")]
        RSR2_GuardDetectorSound
    }
}