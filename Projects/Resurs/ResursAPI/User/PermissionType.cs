using System.ComponentModel;

namespace ResursAPI
{
	public enum PermissionType
	{
		[DescriptionAttribute("права на вкладку устройства")]
		Device,

		[DescriptionAttribute("права на редактирование устройсв")]
		EditDevice,
	
		[DescriptionAttribute("Права на вкладку квартиры ")]
		Apartment,

		[DescriptionAttribute("Права на редактирование квартир ")]
		EditApartment,

		[DescriptionAttribute("Права на вкладку пользователь ")]
		User,

		[DescriptionAttribute("Права на редактирование пользователя ")]
		EditUser,
	}
}