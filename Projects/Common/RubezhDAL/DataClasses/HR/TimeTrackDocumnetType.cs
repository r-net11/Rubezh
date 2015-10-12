using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDDriver.DataClasses
{
	public class TimeTrackDocumentType
	{
		[Key]
		public Guid UID { get; set; }
		[Index]
		public Guid? OrganisationUID { get; set; }
		public Organisation Organisation { get; set; }
		[MaxLength(4000)]
		public string Name { get; set; }
		[MaxLength(10)]
		public string ShortName { get; set; }

		public int DocumentCode { get; set; }

		public int DocumentType { get; set; }
	}
}
