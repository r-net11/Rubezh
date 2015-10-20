using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursNetwork.BCD;

namespace ResursNetwork.Incotex.Models
{
    public enum DayOfWeek : byte
    {
        Sunday = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6,
        /// <summary>
        /// Праздничный день
        /// </summary>
        Selebration = 7
    }
    public struct IncotexDateTime
    {
        #region Fields And Properties
        
        private DayOfWeek _DayOfWeek;
        private byte _Seconds;
        private byte _Minutes;
        private byte _Hours;
        private byte _DayOfMonth;
        private byte _Month;
        private byte _Year;

        /// <summary>
        /// День недели или празднечный день
        /// </summary>
        public DayOfWeek DayOfWeek
        {
            get { return _DayOfWeek; }
            set { _DayOfWeek = value; }
        }
        /// <summary>
        /// Секунды
        /// </summary>
        public byte Seconds
        {
            get { return _Seconds; }
            set 
            {
                if (BcdConverter.IsValid(value))
                {
                    _Seconds = value; 
                }
                else
                {
                    throw new InvalidCastException(String.Format(
                        "Преобразовать значение {0} в формат BCD не возможно", value));
                }
            }
        }
        /// <summary>
        /// Минуты
        /// </summary>
        public byte Minutes
        {
            get { return _Minutes; }
            set 
            { 
                if (BcdConverter.IsValid(value))
                {
                    _Minutes = value;
                }
                else
                {
                    throw new InvalidCastException(String.Format(
                        "Преобразовать значение {0} в формат BCD не возможно", value));
                }
            }
        }
        /// <summary>
        /// Часы
        /// </summary>
        public byte Hours
        {
            get { return _Hours; }
            set 
            {
                if (BcdConverter.IsValid(value))
                {
                    _Hours = value;
                }
                else
                {
                    throw new InvalidCastException(String.Format(
                        "Преобразовать значение {0} в формат BCD не возможно", value));
                }  
            }
        }
        public byte DayOfMonth
        {
            get { return _DayOfMonth; }
            set 
            {
                if (BcdConverter.IsValid(value))
                {
                    _DayOfMonth = value;
                }
                else
                {
                    throw new InvalidCastException(String.Format(
                        "Преобразовать значение {0} в формат BCD не возможно", value));
                } 
            }
        }
        public byte Month
        {
            get { return _Month; }
            set 
            { 
                if (BcdConverter.IsValid(value))
                {
                    if (BcdConverter.ToByte(value) < 13)
                    _Month = value;
                }
                else
                {
                    throw new InvalidCastException(String.Format(
                        "Преобразовать значение {0} в формат BCD не возможно", value));
                } 
            }
        }
        public byte Year
        {
            get { return _Year; }
            set 
            { 
                if (BcdConverter.IsValid(value))
                {
                    _Year = value;
                }
                else
                {
                    throw new InvalidCastException(String.Format(
                        "Преобразовать значение {0} в формат BCD не возможно", value));
                }                 
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Возвращает дату и время в формате System.DateTime
        /// </summary>
        /// <returns></returns>
        public System.DateTime ToDateTime()
        {
            return new System.DateTime((2000 + BcdConverter.ToByte(Year)),
                BcdConverter.ToByte(Month), BcdConverter.ToByte(DayOfMonth), BcdConverter.ToByte(Hours),
                BcdConverter.ToByte(Minutes), BcdConverter.ToByte(Seconds));
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static IncotexDateTime FromDateTime(System.DateTime datetime)
		{
			return new IncotexDateTime
				{
					Year = BcdConverter.ToBcdByte(Convert.ToByte(datetime.Year - 2000)),
					DayOfMonth = BcdConverter.ToBcdByte(Convert.ToByte(datetime.Day)),
					DayOfWeek = (Incotex.Models.DayOfWeek)Convert.ToByte(datetime.DayOfWeek),
					Hours = BcdConverter.ToBcdByte(Convert.ToByte(datetime.Hour)),
					Minutes = BcdConverter.ToBcdByte(Convert.ToByte(datetime.Minute)),
					Seconds = BcdConverter.ToBcdByte(Convert.ToByte(datetime.Second)),
					Month = BcdConverter.ToBcdByte(Convert.ToByte(datetime.Month))
				};
		}

		public static System.DateTime FromIncotexDateTime(IncotexDateTime datetime)
		{
 			return new System.DateTime((2000 + BcdConverter.ToByte(datetime.Year)),
				BcdConverter.ToByte(datetime.Month), 
				BcdConverter.ToByte(datetime.DayOfMonth), 
				BcdConverter.ToByte(datetime.Hours),
				BcdConverter.ToByte(datetime.Minutes),
				BcdConverter.ToByte(datetime.Seconds));
		}
		
		#endregion
	}
}
