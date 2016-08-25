using System.ComponentModel;
using Localization.Converters;
using Localization.Infrastructure.Common;

namespace Infrastructure.Models
{
	public enum XJournalColumnType
    {
        [LocalizedDescription(typeof(CommonResources), "IpAddress")]
		//[DescriptionAttribute("IP-адрес ГК")]
		GKIpAddress = 0,

        [LocalizedDescription(typeof(CommonResources), "SubSystem")]
		//[DescriptionAttribute("Подсистема")]
		SubsystemType = 1,

        [LocalizedDescription(typeof(CommonResources), "User")]
		//[DescriptionAttribute("Пользователь")]
		UserName = 2
	}
}