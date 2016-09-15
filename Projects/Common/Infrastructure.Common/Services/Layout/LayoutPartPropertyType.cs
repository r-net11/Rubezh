using System.ComponentModel;
using Localization.Common.InfrastructureCommon;
using Localization.Converters;

namespace Infrastructure.Common.Services.Layout
{
	public enum LayoutPartPropertyType
	{

		[LocalizedDescription(typeof(CommonResources), "Integer")]
		//[Description("Целое")]
		Integer,

		[LocalizedDescription(typeof(CommonResources), "Double")]
		//[Description("Вещественное")]
		Double,

		[LocalizedDescription(typeof(CommonResources), "Boolean")]
		//[Description("Логическое")]
		Boolean,

		[LocalizedDescription(typeof(CommonResources), "DateTime")]
		//[Description("Дата и время")]
		DateTime,

		[LocalizedDescription(typeof(CommonResources), "String")]
		//[Description("Строка")]
		String,

		[LocalizedDescription(typeof(CommonResources), "Object")]
		//[Description("Объект")]
		Object,
	}
}