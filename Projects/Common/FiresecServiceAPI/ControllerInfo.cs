using System;

namespace StrazhAPI
{
	public class ControllerInfo
	{
		public Guid DeviceUid { get; set; }
		public int JournalItemsCount { get; set; }
		public int AlarmJournlItemsCount { get; set; }
		public string MacAddress { get; set; }
	}
}