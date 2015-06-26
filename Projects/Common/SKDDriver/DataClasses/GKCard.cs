using System;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class GKCard
	{
		[Key]
		public Guid UID { get; set; }

		public string IpAddress { get; set; }

		public int GKNo { get; set; }

		public int CardNo { get; set; }

		public bool IsActive { get; set; }

		public int UserType { get; set; }

        public string FIO { get; set; }
	}
}
