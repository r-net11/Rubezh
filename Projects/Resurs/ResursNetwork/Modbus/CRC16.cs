using System;
using System.Collections.Generic;
using System.Text;

namespace ResursNetwork.Modbus
{
    /// <summary>
    /// Для хранения значения CRC16 
    /// </summary>
    public struct CRC16
    {
        #region Fields And Properties

        private Byte _High;
        /// <summary>
        /// 
        /// </summary>
        public Byte High
        {
            get { return _High; }
            set { _High = value; }
        }
        private Byte _Low;
        /// <summary>
        /// 
        /// </summary>
        public Byte Low
        {
            get { return _Low; }
            set { _Low = value; }
        }
        /// <summary>
        /// Значение CRC16
        /// </summary>
        public UInt16 Value
        {
            get 
            {
                UInt16 value;
                unchecked
                {
                    value = _High;
                    value = (UInt16)(value << 8);
                    value |= Low;
                }
                return value;
            }
            set
            {
                unchecked
                {
                    this.Low = (Byte)value;
                    this.High = (Byte)(value >> 8);
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="value"></param>
        public CRC16(UInt16 value)
        {
            _High = 0;
            _Low = 0;
            Value = value;
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="high">Старший байт CRC16</param>
        /// <param name="low">Младший байт CRC16</param>
        public CRC16(Byte high, Byte low)
        {
            _High = high;
            _Low = low;
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="crc16">CRC16 Где: LowByte = Array[0] и
        /// HighByte = Array[1]</param>
        public CRC16(Byte[] crc16)
        {
            if (crc16 != null)
            {
                if (crc16.Length == 2)
                {
                    _Low = crc16[0];
                    _High = crc16[1];
                }
                else
                {
                    throw new ArgumentException(
                        "Неверная длина массива переданного в качестве агрумента",
                        "crc16");
                }
            }
            else
            {
                throw new ArgumentNullException("crc16",
                    "Попытка передачи массива установленног в null в качестве агрумента");
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Возвращает CRC16 в виде массива из 2 байт. Где: LowByte = Array[0] и
        /// HighByte = Array[1]
        /// </summary>
        /// <returns>CRC16</returns>
        public Byte[] ToArray()
        {
            return new Byte[] { _Low, _High };
        }
        
		/// <summary>
        /// Рассчитывает CRC16 для переданного массива
        /// </summary>
        public static CRC16 GetCRC16(Byte[] array)
        {
            UInt16 crct = 0xFFFF;

            for (int i = 0; i < array.Length; i++)
            {
                crct = (UInt16)(crct ^ System.Convert.ToUInt16(array[i]));

                for (int n = 0; n <= 7; n++)
                {
                    if ((crct & 0x0001) == 0x0001)
                    {
                        crct = (UInt16)(crct >> 1);
                        crct = (UInt16)(crct ^ 0xA001);
                    }
                    else
                    {
                        crct = (UInt16)(crct >> 1);
                    }
                }
            }
            return new CRC16(crct);
        }

		/// <summary>
		/// Получает сообщение и проверяет контрольную сумму в нём
		/// </summary>
		/// <param name="array">Сетевое сообщение (с контрольной суммой)</param>
		/// <returns></returns>
		public static bool CheckCRC16(Byte[] array)
		{
			if (array.Length < 3)
			{
				throw new ArgumentOutOfRangeException("Array.Lengh < 3");
			}

			// Выделяем CRC из массива 
			var crc = new CRC16
			{
				High = array[array.Length - 1],
				Low = array[array.Length - 2]
			};

			var data = new Byte[array.Length - 2];
			Array.Copy(array, data, data.Length); 
			// Для оставшихся байт рассчитываем CRC16
			return crc == CRC16.GetCRC16(data) ? true : false;

		}

		public static bool operator ==(CRC16 x, CRC16 y)
		{
			return (x.High == y.High) && (x.Low == y.Low) ? true : false;
		}

		public static bool operator !=(CRC16 x, CRC16 y)
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
