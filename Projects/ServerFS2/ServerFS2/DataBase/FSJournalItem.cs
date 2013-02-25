using System;
using FiresecAPI;
using FiresecAPI.Models;

namespace ServerFS2.DataBase
{
	public class FSJournalItem
	{
		public DateTime DeviceTime { get; set; } //

		public DateTime SystemTime { get; set; } //

		public string ZoneName { get; set; } //

		public string Description { get; set; } //

		public string DeviceName { get; set; } //

		public Guid DeviceUID { get; set; } //

		public string PanelName { get; set; }

		public Guid PanelUID { get; set; }

		public SubsystemType SubsystemType { get; set; }

		public StateType StateType { get; set; } //

		public string Detalization { get; set; } //

		public int DeviceCategory { get; set; } //

		public string UserName { get; set; }
	}
}