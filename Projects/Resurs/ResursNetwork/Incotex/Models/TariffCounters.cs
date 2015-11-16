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
	public struct TariffCounters
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
			set { _counterTarif1Bcd = ToBcd(value); }
		}

		/// <summary>
		/// Значение счётчика тарифа 2 (кВт*ч)
		/// </summary>
		public float ValueTotalTarif2
		{
			get { return ToFloat(_counterTarif2Bcd); }
			set { _counterTarif2Bcd = ToBcd(value); }
		}

		/// <summary>
		/// Значение счётчика тарифа 3 (кВт*ч)
		/// </summary>
		public float ValueTotalTarif3
		{
			get { return ToFloat(_counterTarif3Bcd); }
			set { _counterTarif3Bcd = ToBcd(value); }
		}

		/// <summary>
		/// Значение счётчика тарифа 4 (кВт*ч)
		/// </summary>
		public float ValueTotalTarif4
		{
			get { return ToFloat(_counterTarif4Bcd); }
			set { _counterTarif3Bcd = ToBcd(value); }
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
		/// Преобразует значение (кВт*ч) в передставление BCD
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		static uint ToBcd(float value)
		{
			if (value > 999999.99f)
			{
				throw new InvalidCastException(String.Format(
					"Попытка преоборазовать число {0} большее 999999.99", value));
			}
			return BcdConverter.ToBcdUInt32(Convert.ToUInt32(value * 100));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array">Массив значений аккумуляторов потреблённой эл. энергии
		/// полученный из счётчика в формате BCD (десятки Вт.ч) старшие разряды вперёд</param>
		/// <returns></returns>
		public static TariffCounters FromArray(uint[] array)
		{
			if (array.Length == 4)
 			{
				return new TariffCounters
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public static TariffCounters FromArray(byte[] array)
		{
			if (array.Length != 16)
			{
				throw new InvalidCastException(
					"Невозможно привести массив байт к структуре. Неверная длина массива");
			}

			byte[] value = new byte[4];
			uint[] values = new uint[4]; 

			for (int i = 0; i < 4; i++)
			{
				Array.Copy(array, (i*4), value, 0, 4);

				//if (BitConverter.IsLittleEndian)
				//{
				//	Array.Reverse(value);
				//}
				values[i] = BitConverter.ToUInt32(value, 0);	
			}

			return FromArray(values);
		}

		public static bool operator ==(TariffCounters x, TariffCounters y)
		{
			return (x._counterTarif1Bcd == y._counterTarif1Bcd) &&
					(x._counterTarif2Bcd == y._counterTarif2Bcd) &&
					(x._counterTarif3Bcd == y._counterTarif3Bcd) &&
					(x._counterTarif4Bcd == y._counterTarif4Bcd) ? true : false;
		}

		public static bool operator !=(TariffCounters x, TariffCounters y)
		{
			return x == y ? false : true;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#endregion
	}
}
