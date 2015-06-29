using System;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class TimeTrackDocument
	{
		[Key]
		public Guid UID { get; set; }

		public Guid? EmployeeUID { get; set; }
		public Employee Employee { get; set; }

		public DateTime StartDateTime { get; set; }

		public DateTime EndDateTime { get; set; }

		public int DocumentCode { get; set; }

		public string Comment { get; set; }

		public string FileName { get; set; }

		public DateTime DocumentDateTime { get; set; }

		public int DocumentNumber { get; set; }
	}
}
