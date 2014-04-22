using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKDModule.Intervals.Common
{
	public static class CopyHelper
	{
		private const string CopyFormat = "{0} ({1})";

		public static string CopyName(string name, IEnumerable<string> names)
		{
			if (!names.Contains(name))
				return name;
			int index = 1;
			while (true)
			{
				var newName = string.Format(CopyFormat, name, index);
				if (!names.Contains(newName))
					return newName;
				index++;
			}
		}
	}
}
