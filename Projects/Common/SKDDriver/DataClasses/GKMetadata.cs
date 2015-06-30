using System;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class GKMetadata
	{
		[Key]
		public Guid UID { get; set; }

		public string IpAddress { get; set; }

		public string SerialNo { get; set; }

		public int LastJournalNo { get; set; }
	}

}
