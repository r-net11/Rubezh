using System.ComponentModel;
using Localization;

namespace FiresecAPI.Automation
{
	public enum ExplicitType
	{
		//[Description("Целое")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ExplicitType), "Integer")]
		Integer,

		//[Description("Логическое")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ExplicitType), "Boolean")]
        Boolean,

		//[Description("Дата и время")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ExplicitType), "DateTime")]
        DateTime,

		//[Description("Время")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ExplicitType), "Time")]
        Time,

		//[Description("Строка")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ExplicitType), "String")]
        String,

		//[Description("Объектная ссылка")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ExplicitType), "Object")]
        Object,

		//[Description("Перечисление")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ExplicitType), "Enum")]
        Enum
	}
}