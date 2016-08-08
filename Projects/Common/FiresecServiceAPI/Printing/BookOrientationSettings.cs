using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrazhAPI.Printing
{
	internal class BookOrientationSettings : IPaperKindSetting
	{
		public string Name { get; private set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public bool IsSystem { get { return true; } }

		public BookOrientationSettings()
		{
			Name = "A4 Книжная ориентация";
			Width = 210;
			Height = 297;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
