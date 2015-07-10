using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDDriver.DataClasses
{
	public class TimeTrackDocument
	{
		[Key]
		public Guid UID { get; set; }
		[Index]
		public Guid? EmployeeUID { get; set; }
		public Employee Employee { get; set; }

		public DateTime StartDateTime { get; set; }

		public DateTime EndDateTime { get; set; }

		public int DocumentCode { get; set; }
		[MaxLength(100)]
		public string Comment { get; set; }
		[MaxLength(100)]
		public string FileName { get; set; }

		public DateTime DocumentDateTime { get; set; }

		public int DocumentNumber { get; set; }
	}
}
