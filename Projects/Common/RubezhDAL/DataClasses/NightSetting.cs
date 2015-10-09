using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RubezhDAL.DataClasses
{
	public class NightSetting
	{
		[Key]
		public Guid UID { get; set; }
		[Index]
		public Guid? OrganisationUID { get; set; }
		public Organisation Organisation { get; set; }

		public int NightStartTime { get; set; }

		public int NightEndTime { get; set; }
	}
}
