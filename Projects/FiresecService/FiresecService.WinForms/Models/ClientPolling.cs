using System;

namespace FiresecService.Models
{
	public class ClientPolling
	{
		public string Client { get; set; }
		public Guid UID { get; set; }
		public DateTime FirstPollTime { get; set; }
		public DateTime LastPollTime { get; set; }
		public int CallbackIndex { get; set; }
	}
}