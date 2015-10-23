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
		[Description("Редактированиe пользователя")]
		EditUser,
		[Description("Добавлениe пользователя")]
		AddUser,
		[Description("Удалениe пользователя")]
		DeleteUser,
		[Description("Редактированиe устройства")]
		EditDevice,
		[Description("Добавлениe устройства")]
		AddDevice,
		[Description("Удалениe устройства")]
		DeleteDevice,
		[Description("Редактированиe абонента")]
		EditApartment,
		[Description("Добавлениe абонента")]
		AddApartment,
		[Description("Удалениe абонента")]
		DeleteApartment,
		[Description("Редактированиe тарифа")]
		EditTariff,
		[Description("Добавлениe тарифа")]
		AddTariff,
		[Description("Удалениe тарифа")]
		DeleteTariff,
	}
}