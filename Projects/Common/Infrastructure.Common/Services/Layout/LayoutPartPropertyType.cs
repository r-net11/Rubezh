using System.ComponentModel;
using Localization;

namespace Infrastructure.Common.Services.Layout
{
	public enum LayoutPartPropertyType
	{
		//[Description("Целое")]
        [LocalizedDescription(typeof(Resources.Language.Services.Layout.LayoutPartPropertyType), "Integer")]
		Integer,

        //[Description("Вещественное")]
        [LocalizedDescription(typeof(Resources.Language.Services.Layout.LayoutPartPropertyType), "Double")]
		Double,

        //[Description("Логическое")]
        [LocalizedDescription(typeof(Resources.Language.Services.Layout.LayoutPartPropertyType), "Boolean")]
		Boolean,

        //[Description("Дата и время")]
        [LocalizedDescription(typeof(Resources.Language.Services.Layout.LayoutPartPropertyType), "DateTime")]
		DateTime,

        //[Description("Строка")]
        [LocalizedDescription(typeof(Resources.Language.Services.Layout.LayoutPartPropertyType), "String")]
		String,

        //[Description("Объект")]
        [LocalizedDescription(typeof(Resources.Language.Services.Layout.LayoutPartPropertyType), "Object")]
		Object,
	}
}