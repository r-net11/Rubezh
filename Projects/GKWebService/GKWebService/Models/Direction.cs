using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKWebService.Models
{
	public class Direction
	{
		public Guid UID { get; set; }
		public int No { get; set; }
		public string Name { get; set; }
		public string State { get; set; }
		public string StateIcon { get; set; }
	}
}