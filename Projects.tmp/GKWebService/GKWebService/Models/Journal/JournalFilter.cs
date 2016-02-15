using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKWebService.Models
{
	public class JournalFilter
	{
		public List<Guid> ObjectUids { get; set; }
		public List<InputFilterEvent> Events { get; set; }
		public DateTime? BeginDate { get; set; }
		public DateTime? EndDate { get; set; }
	}

	public class InputFilterEvent
	{
		public int Type { get; set; }
		public int Value { get; set; }
	}

	public class JournalFilterJson
	{
		public List<JournalFilterObject> Objects { get; set; }
		public List<JournalFilterEvent> Events { get; set; }
		public DateTime MinDate { get; set; }
		public DateTime MaxDate { get; set; }
	}
}