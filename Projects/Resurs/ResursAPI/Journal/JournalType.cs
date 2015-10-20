using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public enum JournalType
	{
		[Description("Вход в систему")]
		System,
		[Description("Редактирования пользователя")]
		EditUser,
		[Description("Добавления пользователя")]
		AddUser,
		[Description("Удаления пользователя")]
		DeleteUser,
		[Description("Редактирования устройства")]
		EditDevice,
		[Description("Добавления устройства")]
		AddDevice,
		[Description("Удаления устройства")]
		DeleteDevice,
		[Description("Редактирования абонента")]
		EditApartment,
		[Description("Добавления абонента")]
		AddApartment,
		[Description("Удаления абонента")]
		DeleteApartment,
		[Description("Редактирования тарифа")]
		EditTariff,
		[Description("Добавления тарифа")]
		AddTariff,
		[Description("Удаления тарифа")]
		DeleteTariff,
	}
}