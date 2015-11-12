using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursNetwork.BCD;
using ResursNetwork.OSI.ApplicationLayer.Devices.ValueConverters;

namespace ResursNetwork.Incotex.Models
{
	internal class MpowerValueConverter:
		IValueConverter
	{
		/// <summary>
		/// Преобразует массив из 2 байт в значение float (mm.mm кВт*ч)
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public ValueType FromArray(byte[] array)
		{
			return Mpower.FromArray(array).PowerLimit;
		}

		/// <summary>
		/// Сериализует значение (Mpower или float (mm.mm кВт*ч)) в массив
		/// из 2 байт в BCD формате
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public byte[] ToArray(ValueType value)
		{
			return Mpower.ToArray(value);
		}
	}

	/// <summary>
	/// Соотвествует типу данных протокола Incontex Меркурий 203
	/// </summary>
	public struct Mpower
	{
		#region Fields And Properties

		static MpowerValueConverter _converter;
		ushort _powerLimitBcd;

		public float PowerLimit
		{
			get { return ToFloat(_powerLimitBcd); }
		}

		#endregion

		#region Methods

		static float ToFloat(ushort bcdValue)
		{
			if (BcdConverter.IsValid(bcdValue))
			{
				return ((float)BcdConverter.ToUInt16(bcdValue)) / 100;
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
		/// <param name="array"></param>
		/// <returns></returns>
		public static Mpower FromArray(byte[] array)
		{
			if (array.Length != 2)
			{
				throw new InvalidCastException(
					"Невозможно привести массив байт к структуре. Неверная длина массива");
			}

			//if (BitConverter.IsLittleEndian)
			//{
			//	Array.Reverse(array);
			//}
			return new Mpower
			{
				_powerLimitBcd = BitConverter.ToUInt16(array, 0)
			};			
		}

		/// <summary>
		/// Сериализует значение (Mpower или float (mm.mm кВт*ч)) в массив
		/// из 2 байт в BCD формате
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte[] ToArray(ValueType value)
		{
			if (value is Mpower)
			{
				var bcd = (Mpower)value;
				return BitConverter.GetBytes(bcd._powerLimitBcd);
			}
			else if (value is float)
			{
				var x = (float)value;

				if (x >= 100)
				{
					throw new ArgumentOutOfRangeException(
						"Значение лимта мощьности не можеть быть больше или равно 100");
				}

				// Получаем целую часть числа
				byte integerQuotient = Convert.ToByte(Math.Truncate(x));
				// Получаем дробную часть числа
				byte remainder = Convert.ToByte(Math.Round((x - integerQuotient), 2) * 100);

				byte[] result = new byte[2];
				result[0] = BcdConverter.ToBcdByte(integerQuotient);
				result[1] = BcdConverter.ToBcdByte(remainder);
				return result;
			}
			else
			{
				throw new InvalidCastException("Невозможно сериализовать значение");
			}
		}

		public static IValueConverter GetValueConveter()
		{
			if (_converter == null)
			{
				_converter = new MpowerValueConverter();
			}
			return (IValueConverter)_converter;
		}

		#endregion
	}
}
