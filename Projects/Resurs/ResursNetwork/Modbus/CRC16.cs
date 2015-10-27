using System;
using System.Collections.Generic;
using System.Text;

namespace ResursNetwork.Modbus
{
    /// <summary>
    /// ��� �������� �������� CRC16 
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
        /// �������� CRC16
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
        /// �����������
        /// </summary>
        /// <param name="value"></param>
        public CRC16(UInt16 value)
        {
            _High = 0;
            _Low = 0;
            Value = value;
        }
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="high">������� ���� CRC16</param>
        /// <param name="low">������� ���� CRC16</param>
        public CRC16(Byte high, Byte low)
        {
            _High = high;
            _Low = low;
        }
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="crc16">CRC16 ���: LowByte = Array[0] �
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
                        "�������� ����� ������� ����������� � �������� ���������",
                        "crc16");
                }
            }
            else
            {
                throw new ArgumentNullException("crc16",
                    "������� �������� ������� ������������� � null � �������� ���������");
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// ���������� CRC16 � ���� ������� �� 2 ����. ���: LowByte = Array[0] �
        /// HighByte = Array[1]
        /// </summary>
        /// <returns>CRC16</returns>
        public Byte[] ToArray()
        {
            return new Byte[] { _Low, _High };
        }
        
		/// <summary>
        /// ������������ CRC16 ��� ����������� �������
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
		/// �������� ��������� � ��������� ����������� ����� � ��
		/// </summary>
		/// <param name="array">������� ��������� (� ����������� ������)</param>
		/// <returns></returns>
		public static bool CheckCRC16(Byte[] array)
		{
			if (array.Length < 3)
			{
				throw new ArgumentOutOfRangeException("Array.Lengh < 3");
			}

			// �������� CRC �� ������� 
			var crc = new CRC16
			{
				High = array[array.Length - 1],
				Low = array[array.Length - 2]
			};

			var data = new Byte[array.Length - 2];
			Array.Copy(array, data, data.Length); 
			// ��� ���������� ���� ������������ CRC16
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
