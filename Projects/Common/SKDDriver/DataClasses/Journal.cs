using System;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class Journal
	{
		[Key]
		public Guid UID { get; set; }

		public Guid? EmployeeUID { get; set; }
		public Employee Employee { get; set; }

		public DateTime SystemDate { get; set; }

		public DateTime? DeviceDate { get; set; }

		public int Subsystem { get; set; }

		public int Name { get; set; }

		public int Description { get; set; }

		public string DescriptionText { get; set; }

		public int ObjectType { get; set; }

		public Guid ObjectUID { get; set; }

		public string Detalisation { get; set; }

		public string UserName { get; set; }

		public Guid? VideoUID { get; set; }

		public Guid? CameraUID { get; set; }

		public string ObjectName { get; set; }

		public int CardNo { get; set; }
	}
}
