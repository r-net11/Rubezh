using System;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class Photo
	{
		[Key]
		public Guid UID { get; set; }

		public byte[] Data { get; set; } 
	}
}
