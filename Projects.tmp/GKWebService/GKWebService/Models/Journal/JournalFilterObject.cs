using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKWebService.Models
{
	public class JournalFilterObject
	{
		public Guid UID { get; set; }

		public string Name { get; set; }

		public string ImageSource { get; set; }

		public int Level { get; set; }

		public JournalFilterObject(Guid uid, string imageSource, string name, int level)
		{
			UID = uid;
			ImageSource = imageSource;
			Name = name;
			Level = level;
		}
	}
}