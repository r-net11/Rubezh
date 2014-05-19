using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public enum SKDJournalItemType
	{
		[EnumMember]
		[DescriptionAttribute("Система")]
		System,

		[EnumMember]
		[DescriptionAttribute("Контроллер")]
		Controller,

		[EnumMember]
		[DescriptionAttribute("Считыватель")]
		Reader,
	}
}