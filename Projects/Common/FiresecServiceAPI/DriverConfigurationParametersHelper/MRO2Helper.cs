using System.Collections.Generic;
using System.Linq;

namespace FiresecAPI.Models
{
	public class MRO2Helper
	{
		public static void Create(List<Driver> drivers)
		{
			var driver = drivers.FirstOrDefault(x => x.DriverType == DriverType.MRO_2);
			if (driver != null)
			{
				driver.HasConfigurationProperties = true;
				//ConfigurationDriverHelper.AddIntProprety(driver, 0x80, "Проигрываемое сообщение", "Проигрываемое сообщение", 0, 1, 1, 8);
				ConfigurationDriverHelper.AddIntProprety(driver, 0x82, "Количество повторов", "Количество повторов", 0, 3, 0, 255);
				ConfigurationDriverHelper.AddIntProprety(driver, 0x83, "Сообщений в памяти", "Сообщений в памяти", 0, 1, 0, 255).IsReadOnly = true;
				ConfigurationDriverHelper.AddIntProprety(driver, 0x84, "Сопротивление выходной линии, 0.1 Ом", "Сопротивление выходной линии, 0.1 Ом", 0, 0, 0, 255).IsReadOnly = true;
				ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x87, "Режим работы", 0, "Ведомый", "Ведущий", 1, 0, 0, false, false, "1");
				//ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x80, "Источник воспроизведения", 1, "Внутренняя память", "Аналоговый вход", 0, 1, 2);
				ConfigurationDriverHelper.AddIntProprety(driver, 0x88, "Задержка включения, с", "Задержка включения, с", 0, 0, 0, 255);

				var property1 = new DriverProperty()
				{
					IsAUParameter = true,
					No = 0x89,
					Name = "Линейный вход",
					Caption = "Линейный вход",
					Default = "0"
				};
				ConfigurationDriverHelper.AddPropertyParameter(property1, "250мВ", 0);
				ConfigurationDriverHelper.AddPropertyParameter(property1, "500мВ", 1);
				ConfigurationDriverHelper.AddPropertyParameter(property1, "775мВ", 2);
				driver.Properties.Add(property1);

				ConfigurationDriverHelper.AddPlainEnumProprety(driver, 0x90, "Рабочее напряжение", 0, "12", "24", 0, 0, 0, false, false, "1");
			}
		}

		#region Методы для параметра "Проигрываемое сообщение"
		public static int SetMessageNumber(int intValue)
		{
			int[] bits;
			if (intValue == 8)
			{
				bits = new int[] { 1, 1, 1 };
			}
			else if (intValue == 7)
			{
				bits = new int[] { 1, 1, 0 };
			}
			else if (intValue == 6)
			{
				bits = new int[] { 1, 0, 1 };
			}
			else if (intValue == 5)
			{
				bits = new int[] { 1, 0, 0 };
			}
			else if (intValue == 4)
			{
				bits = new int[] { 0, 1, 1 };
			}
			else if (intValue == 3)
			{
				bits = new int[] { 0, 1, 0 };
			}
			else if (intValue == 2)
			{
				bits = new int[] { 0, 0, 1 };
			}
			else
			{
				bits = new int[] { 0, 0, 0 };
			}
			return bits[0] << 7 + bits[1] << 4 + bits[2] << 2;
		}

		public static Property GetMessageNumber(int offsetParamValue)
		{
			bool bit7 = GetBit(offsetParamValue, 7);
			bool bit4 = GetBit(offsetParamValue, 4);
			bool bit2 = GetBit(offsetParamValue, 2);

			var property = new Property()
			{
				Name = "Проигрываемое сообщение",
				Value = MessageNumFromBits(bit7, bit4, bit2).ToString()
			};

			return property;
		}

		static int MessageNumFromBits(bool bit7, bool bit4, bool bit2)
		{
			int result;
			if (bit7 && bit4 && bit2)
				result = 8;
			else if (bit7 && bit4 && !bit2)
				result = 7;
			else if (bit7 && !bit4 && bit2)
				result = 6;
			else if (bit7 && !bit4 && !bit2)
				result = 5;
			else if (!bit7 && bit4 && bit2)
				result = 4;
			else if (!bit7 && bit4 && !bit2)
				result = 3;
			else if (!bit7 && !bit4 && bit2)
				result = 2;
			else
				result = 1;
			return result;
		}

		static bool GetBit(int offsetParamValue, int minBit)
		{
			int value = offsetParamValue;
			int maxBit = minBit + 1;

			byte byteOffsetParamValue = (byte)value;
			byteOffsetParamValue = (byte)(byteOffsetParamValue >> minBit);
			byteOffsetParamValue = (byte)(byteOffsetParamValue << minBit);
			value = byteOffsetParamValue;

			byteOffsetParamValue = (byte)value;
			byteOffsetParamValue = (byte)(byteOffsetParamValue << 8 - maxBit);
			byteOffsetParamValue = (byte)(byteOffsetParamValue >> 8 - maxBit);
			value = byteOffsetParamValue;

			if (value != 0)
				return true;
			else
				return false;
		}
		#endregion Методы для параметра "Проигрываемое сообщение"
	}
}