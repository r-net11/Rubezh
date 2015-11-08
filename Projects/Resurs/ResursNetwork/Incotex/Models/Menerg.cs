using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursNetwork.BCD;
using ResursNetwork.OSI.ApplicationLayer.Devices.ValueConverters;

namespace ResursNetwork.Incotex.Models
{
	internal class MenergValueConverter : IValueConverter
	{
		/// <summary>
		/// Преобразует массив из 2 байт в значение ushort (mmmm кВт*ч)
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public ValueType FromArray(byte[] array)
		{
			return Menerg.FromArray(array).PowerLimitPerMonth;
		}

		/// <summary>
		/// Сериализует значение (Mpower или ushort (mmmm кВт*ч)) в массив
		/// из 2 байт в BCD формате
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public byte[] ToArray(ValueType value)
		{
			return Menerg.ToArray(value);
		}
	}

	public struct Menerg
	{
		#region Fields And Properties

		static MenergValueConverter _converter;
		ushort _powerLimitBcd;

		public ushort PowerLimitPerMonth
		{
			get { return BcdConverter.ToUInt16(_powerLimitBcd); }
			set { _powerLimitBcd = BcdConverter.ToBcdUInt16(value); }
		}

		#endregion

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public static Menerg FromArray(byte[] array)
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
			return new Menerg
			{
				_powerLimitBcd = BitConverter.ToUInt16(array, 0)
			};
		}

		/// <summary>
		/// Сериализует значение (Mpower или ushort (mmmm кВт*ч)) в массив
		/// из 2 байт в BCD формате
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte[] ToArray(ValueType value)
		{
			if (value is Menerg)
			{
				var bcd = (Menerg)value;
				return BitConverter.GetBytes(bcd._powerLimitBcd);
			}
			else if (value is ushort)
			{
				var x = (ushort)value;

				if (x >= 10000)
				{
					throw new ArgumentOutOfRangeException(
						"Значение лимта мощьности не можеть быть больше или равно 100");
				}

				var bcd = BcdConverter.ToBcdUInt16(x);
				return BitConverter.GetBytes(bcd);
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
				_converter = new MenergValueConverter();
			}
			return (IValueConverter)_converter;
		}

		#endregion
	}
}
