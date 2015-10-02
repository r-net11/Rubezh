﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class Journal
	{
		public Journal()
		{
			UID = Guid.NewGuid();
		}
		[Key]
		public Guid UID { get; set; }

		public JournalType JurnalType { get; set; }

		public DateTime DateTime { get; set; }

		public string NameUser { get; set; }	}
}
