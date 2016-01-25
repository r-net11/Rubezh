using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using opcproxy;

namespace OpcFoundation
{
	public class OpcDaTagValue
	{
		public enum Quality: ushort
		{
			Bad = 0x00,
			ConfigError = 0x04,
			NotConnected = 0x08,
			DeviceFailure = 0x0C,
			SensorFailure = 0x10,
			LastKnown = 0x14,
			CommFailure = 0x18,
			OutOfService = 0x1C,
			Initializing = 0x20,
			Uncertain = 0x40,
			LastUsable = 0x44,
			SensorCalibration = 0x50,
			EGUExceeded = 0x54,
			SubNormal = 0x58,
			Good = 0xC0,
			LocalOverride = 0xD8,
		}

		#region Constructors

		public OpcDaTagValue(string tagId, ushort valueType, 
			object value, _FILETIME timeStamp, ushort signalQuality)
		{
			TagId = tagId;
			ValueType = ConvertToValueType(valueType);
			Value = value;
			TimeStamp = ConvertToDateTime(timeStamp);
			if (Enum.IsDefined(typeof(Quality), signalQuality))
			{
				SignalQuality = (Quality)signalQuality;
			}
			else
			{
				throw new InvalidCastException(String.Format(
					"Невозможно конвертировать значение {0} в тип Quality", 
					signalQuality));
			}
		}

		#endregion

		#region Fields And Properties

		public string TagId { get; set; }
		public VarEnum ValueType { get; set; }
		public object Value { get; set; }
		public DateTime TimeStamp { get; set; }
		public Quality SignalQuality { get; set; }

		#endregion

		#region Methods

		public static DateTime ConvertToDateTime(_FILETIME dateTime)
		{
			long lFT = (((long)dateTime.dwHighDateTime) << 32) + dateTime.dwLowDateTime;
			DateTime dt = System.DateTime.FromFileTime(lFT);
			return dt;
		}

		public static VarEnum ConvertToValueType(ushort valueType)
		{
			return (VarEnum)valueType;
		}

		#endregion
	}

}
