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
		[Description("Редактированиe лицевого счета")]
		EditConsumer,
		[Description("Добавлениe лицевого счета")]
		AddConsumer,
		[Description("Удалениe лицевого счета")]
		DeleteConsumer,
		[Description("Редактированиe тарифа")]
		EditTariff,
		[Description("Добавлениe тарифа")]
		AddTariff,
		[Description("Удалениe тарифа")]
		DeleteTariff,
		[Description("Редактированиe пополнения баланса")]
		EditDeposit,
		[Description("Пополнение баланса")]
		AddDeposit,
		[Description("Удалениe пополнения баланса")]
		DeleteDeposit,
	}
}