using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrazhAPI.Printing
{
	public interface IPaperKindSetting
	{
		string Name { get; }
		int Width { get; }
		int Height { get; }
		bool IsSystem { get; }
	}
}
