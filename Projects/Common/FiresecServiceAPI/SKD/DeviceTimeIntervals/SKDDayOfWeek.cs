using System.ComponentModel;
using Localization;

namespace FiresecAPI.SKD
{
	public enum SKDDayOfWeek
	{
		//[Description("Воскресенье")]
        [LocalizedDescription(typeof(Resources.Language.SKD.DeviceTimeIntervals.SKDDayOfWeek), "Sunday")]
		Sunday = 1,

        //[Description("Понедельник")]
        [LocalizedDescription(typeof(Resources.Language.SKD.DeviceTimeIntervals.SKDDayOfWeek), "Monday")]
		Monday = 2,

        //[Description("Вторник")]
        [LocalizedDescription(typeof(Resources.Language.SKD.DeviceTimeIntervals.SKDDayOfWeek), "Tuesday")]
		Tuesday = 3,

        //[Description("Среда")]
        [LocalizedDescription(typeof(Resources.Language.SKD.DeviceTimeIntervals.SKDDayOfWeek), "Wednesday")]
		Wednesday = 4,

        //[Description("Четверг")]
        [LocalizedDescription(typeof(Resources.Language.SKD.DeviceTimeIntervals.SKDDayOfWeek), "Thursday")]
		Thursday = 5,

        //[Description("Пятница")]
        [LocalizedDescription(typeof(Resources.Language.SKD.DeviceTimeIntervals.SKDDayOfWeek), "Friday")]
		Friday = 6,

        //[Description("Суббота")]
        [LocalizedDescription(typeof(Resources.Language.SKD.DeviceTimeIntervals.SKDDayOfWeek), "Saturday")]
		Saturday = 7,
	}
}