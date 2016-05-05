using System;

namespace StrazhDeviceSDK.API
{
	public class DeviceJournalItem
	{
		public DateTime DateTime { get; set; }

		public string OperatorName { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }
	}
}