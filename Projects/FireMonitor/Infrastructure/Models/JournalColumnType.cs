using System.ComponentModel;
using Localization.Converters;
using Localization.Infrastructure.Common;

namespace Infrastructure.Models
{
	public enum JournalColumnType
    {
        [LocalizedDescription(typeof(CommonResources), "SubSystem")]
		//[DescriptionAttribute("Подсистема")]
		SubsystemType,

        [LocalizedDescription(typeof(CommonResources), "User")]
		//[DescriptionAttribute("Пользователь")]
		UserName
	}
}