using System;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class TimeTrackDocumnetType
	{
		[Key]
		public Guid UID { get; set; }

		public Guid? OrganisationUID { get; set; }
		public Organisation Organisation { get; set; }

		public string Name { get; set; }

		public string ShortName { get; set; }

		public int DocumentCode { get; set; }

		public int DocumentType { get; set; }
	}
}
