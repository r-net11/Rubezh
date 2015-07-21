using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDDriver.DataClasses
{
	public class AdditionalColumn
	{
		[Key]
		public Guid UID { get; set; }
		[Index]
		public Guid? EmployeeUID { get; set; }
		public Employee Employee { get; set; }

		public Guid? AdditionalColumnTypeUID { get; set; }
		public AdditionalColumnType AdditionalColumnType { get; set; }

		public Guid? PhotoUID { get; set; }
		public Photo Photo { get; set; }

		public string TextData { get; set; }
	}
}
