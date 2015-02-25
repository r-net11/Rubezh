using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SKDModule.Common
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

		public static string CopyFileName(string name, IEnumerable<string> names)
		{
			if (!names.Contains(name))
				return name;
			int index = 1;
			while (true)
			{
				var shortName = Path.GetFileNameWithoutExtension(name);
				var ext = Path.GetExtension(name);
				var newName = string.Format(CopyFormat, shortName, index) + ext;
				if (!names.Contains(newName))
					return newName;
				index++;
			}
		}

		
	}
}