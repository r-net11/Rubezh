using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class ModelBase
	{
		public ModelBase()
		{
			UID = Guid.NewGuid();
		}
		[Key]
		public Guid UID { get; set; }
		[MaxLength(50)]
		public string Name { get; set; }
		[MaxLength(200)]
		public string Description { get; set; }
	}
}
