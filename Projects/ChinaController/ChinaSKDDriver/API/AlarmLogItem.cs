using System;
using StrazhDeviceSDK;

namespace StrazhDeviceSDK.API
{
	public class AlarmLogItem
	{
		public int LogNo { get; set; }

		public DateTime DateTime { get; set; }

		public string UserName { get; set; }

		public string LogType { get; set; }

		public string LogMessage { get; set; }

		public string CardId { get; set; }

		public AlarmType AlarmType { get; set; }

		public int Channel { get; set; }
	}
}