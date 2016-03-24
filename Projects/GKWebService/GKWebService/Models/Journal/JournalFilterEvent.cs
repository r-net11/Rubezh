using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKWebService.Models
{
	public class JournalFilterEvent
	{
		/// <summary>
		/// Subsystem, event или description
		/// </summary>
		public int Type { get; set; }
		/// <summary>
		/// Значение enum
		/// </summary>
		public int Value { get; set; }
		public string Name { get; set; }
		public string ImageSource { get; set; }
		public int Level { get; set; }
		public int ParentValue { get; set; }

		public JournalFilterEvent(int type, int value, string name, string imageSource, int level, int parentValue)
		{
			Type = type;
			Value = value;
			Name = name;
			ImageSource = imageSource;
			Level = level;
			ParentValue = parentValue;
		}
	}
}