using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI.ParameterNames
{
	public class ParameterNamesMZEP55Counter : ParameterNamesBase
	{
		/// <summary>
		/// Счётчик подключён
		/// </summary>
		public const string IsConnected = "IsConnected";
		/// <summary>
		/// Счётчик открыт на чтение
		/// </summary>
		public const string CanRead = "CanRead";
		/// <summary>
		///Счётчик открыт на запись
		/// </summary>
		public const string CanWrite = "CanWrite";
		/// <summary>
		///Пароль первого уровня
		/// </summary>
		public const string FirstPassword = "FirstPassword";
		/// <summary>
		///Пароль второго уровня
		/// </summary>
		public const string SecondPassword = "SecondPassword";
		/// <summary>
		//Коэффициент трансформации
		/// </summary>
		public const string TransformFactor = "TransformFactor";
		/// <summary>
		//Шаг записи расхода в лог
		/// </summary>
		public const string LogStep = "LogStep";
		/// <summary>
		//Дата фиксации расхода для дерева пользователей
		/// </summary>
		public const string UserTreeDate = "UserTreeDate";
		/// <summary>
		//Дата фиксации расхода для дерева баланса
		/// </summary>
		public const string BallanceTreeDate = "BallanceTreeDate";
		/// <summary>
		//Параметры режимов индикации
		/// </summary>
		public const string IndicationParameters = "IndicationParameters";
		/// <summary>
		//Ток
		/// </summary>
		public const string Current = "Current";
		/// <summary>
		//Напряжение
		/// </summary>
		public const string Voltage = "Voltage";
		/// <summary>
		//Активная мощность
		/// </summary>
		public const string Power = "Power";
		/// <summary>
		//Коэффициент мощности
		/// </summary>
		public const string PowerFactor = "PowerFactor";
		/// <summary>
		//Частота сетевого напряжения
		/// </summary>
		public const string Frequency = "Frequency";
		/// <summary>
		//Активная энергия по текщему тарифу
		/// </summary>
		public const string Energy = "Energy";
		/// <summary>
		//Время наработки
		/// </summary>
		public const string WorkoutTime = "WorkoutTime";
		/// <summary>
		//Величина ограничения
		/// </summary>
		public const string Restriction = "Restriction";
		/// <summary>
		//Отображение тарифов
		/// </summary>
		public const string ShownTariffsCount = "ShownTariffsCount";
	}
}
