using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.Models.Skud
{
	public class Document
	{
		public Guid Uid { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime? IssueDate { get; set; }
		public DateTime? LaunchDate { get; set; }
	}
}
