using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class Journal
	{
		public Journal()
		{
			UID = Guid.NewGuid();
		}
		[Key]
		public Guid UID { get; set; }
		public JournalType JournalType { get; set; }
		public DateTime DateTime { get; set; }
		[MaxLength(100)]
		public string UserName { get; set; }
		[MaxLength(200)]
		public string ObjectName { get; set; }
		[MaxLength(500)]
		public string Description { get; set; }
		public Guid? ObjectUID { get; set; }
		public Guid? UserUID { get; set; }
	}
}