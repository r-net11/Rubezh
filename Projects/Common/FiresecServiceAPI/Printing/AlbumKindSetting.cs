using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReportSystem.Api.Interfaces;

namespace StrazhAPI.Printing
{
	internal class AlbumKindSetting : IPaperKindSetting
	{
		public string Name { get; private set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public bool IsSystem { get { return true; } }

		public AlbumKindSetting()
		{
			Name = "A4 Альбомная ориентация";
			Width = 297;
			Height = 210;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
