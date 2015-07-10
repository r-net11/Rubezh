using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDDriver.DataClasses
{
	public class Journal
	{
		[Key]
		public Guid UID { get; set; }

		public Guid? EmployeeUID { get; set; }
		public Employee Employee { get; set; }
		[Index]
		public DateTime SystemDate { get; set; }
		[Index]
		public DateTime? DeviceDate { get; set; }

		public int Subsystem { get; set; }
		[Index]
		public int Name { get; set; }
		[Index]
		public int Description { get; set; }
		[MaxLength(4000)]
		public string DescriptionText { get; set; }

		public int ObjectType { get; set; }
		[Index]
		public Guid ObjectUID { get; set; }
		[MaxLength(4000)]
		public string Detalisation { get; set; }
		[MaxLength(50)]
		public string UserName { get; set; }

		public Guid? VideoUID { get; set; }

		public Guid? CameraUID { get; set; }
		[MaxLength(100)]
		public string ObjectName { get; set; }

		public int CardNo { get; set; }
	}
}
