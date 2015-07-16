using System;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class GKMetadata
	{
		[Key]
		public Guid UID { get; set; }
		[MaxLength(50)]
		public string IpAddress { get; set; }
		[MaxLength(50)]
		public string SerialNo { get; set; }

		public int LastJournalNo { get; set; }
	}

}
