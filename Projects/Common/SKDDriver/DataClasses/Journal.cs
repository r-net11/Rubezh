using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RubezhDAL.DataClasses
{
	public class Journal
	{
		[Key]
		public Guid UID { get; set; }
		[Index]
		public Guid? EmployeeUID { get; set; }
		[Index]
		public Employee Employee { get; set; }
		[Index]
		public DateTime SystemDate { get; set; }
		[Index]
		public DateTime? DeviceDate { get; set; }
		[Index]
		public int Subsystem { get; set; }
		[Index]
		public int Name { get; set; }
		[Index]
		public int Description { get; set; }
		[Index]
		[MaxLength(4000)]
		public string DescriptionText { get; set; }
		[Index]
		public int ObjectType { get; set; }
		[Index]
		public Guid ObjectUID { get; set; }
		[Index]
		[MaxLength(4000)]
		public string Detalisation { get; set; }
		[Index]
		[MaxLength(50)]
		public string UserName { get; set; }
		[Index]
		public Guid? VideoUID { get; set; }
		[Index]
		public Guid? CameraUID { get; set; }
		[Index]
		[MaxLength(100)]
		public string ObjectName { get; set; }
		[Index]
		public int CardNo { get; set; }
	}
}
