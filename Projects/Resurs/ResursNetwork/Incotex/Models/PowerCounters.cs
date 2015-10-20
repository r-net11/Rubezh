using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursNetwork.BCD;

namespace ResursNetwork.Incotex.Models
{
	/// <summary>
	/// Для хранения содержимого аккумуляторов 
	/// (счётчиков нарастающего итога) 
	/// потреблённой эл. энергии 4-х зон 
	/// </summary>
	public struct PowerCounters
	{
		#region Fields And Properties

		uint _counterTarif1Bcd;
		uint _counterTarif2Bcd;
		uint _counterTarif3Bcd;
		uint _counterTarif4Bcd;

		/// <summary>
		/// Значение счётчика тарифа 1 (кВт*ч)
		/// </summary>
		public float ValueTotalTarif1
		{
			get { return ToFloat(_counterTarif1Bcd); }
		}

		/// <summary>
		/// Значение счётчика тарифа 2 (кВт*ч)
		/// </summary>
		public float ValueTotalTarif2
		{
			get { return ToFloat(_counterTarif2Bcd); }
		}

		/// <summary>
		/// Значение счётчика тарифа 3 (кВт*ч)
		/// </summary>
		public float ValueTotalTarif3
		{
			get { return ToFloat(_counterTarif3Bcd); }
		}

		/// <summary>
		/// Значение счётчика тарифа 4 (кВт*ч)
		/// </summary>
		public float ValueTotalTarif4
		{
			get { return ToFloat(_counterTarif4Bcd); }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Преобразует в кВт*ч
		/// </summary>
		/// <param name="bcdValue">значение из счётчика эл. энергии</param>
		/// <returns>Значение в кВт*ч с точностью до 2 разрядов после запятой</returns>
		static float ToFloat(uint bcdValue)
		{
			if (BcdConverter.IsValid(bcdValue))
			{
				return ((float)BcdConverter.ToUInt32(bcdValue)) / 100;
			}
			else
			{
				throw new InvalidCastException(
					"Невозможно выполнить приведение типов. Формат исходного числа не BCD");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array">Массив значений аккумуляторов потреблённой эл. энергии
		/// полученный из счётчика в формате BCD (десятки Вт.ч) старшие разряды вперёд</param>
		/// <returns></returns>
		public static PowerCounters FromArray(uint[] array)
		{
			if (array.Length == 4)
 			{
				return new PowerCounters
				{
					_counterTarif1Bcd = array[0],
					_counterTarif2Bcd = array[1],
					_counterTarif3Bcd = array[2],
					_counterTarif4Bcd = array[3]
				};
			}
			else
			{
				throw new ArgumentException();
			}
		}

		#endregion
	}
}
