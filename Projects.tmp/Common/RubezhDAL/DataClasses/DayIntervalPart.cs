using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RubezhDAL.DataClasses
{
	public class DayIntervalPart
	{
		[Key]
		public Guid UID { get; set; }
		[Index]
		public Guid? DayIntervalUID { get; set; }
		public DayInterval DayInterval { get; set; }

		public double BeginTimeTotalSeconds { get; set; }

		public double EndTimeTotalSeconds { get; set; }

		public int Number { get; set; }
	}
}
