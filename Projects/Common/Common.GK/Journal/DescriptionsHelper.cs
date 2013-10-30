using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.GK
{
	public static class DescriptionsHelper
	{
		static List<string> result;

		public static List<string> GetAllDescriptions()
		{
			result = new List<string>();
			for (int i = 1; i <= 255; i++)
			{
				AddToResult(StringHelper.ToFailure((byte)i));
				AddToResult(StringHelper.ToBUSHFailure((byte)i));
				AddToResult(StringHelper.ToInformation((byte)i));
				AddToResult(StringHelper.ToBUSHInformation((byte)i));
			}
			result.Sort();
			return result;
		}

		static void AddToResult(string description)
		{
			if (description != "" && !result.Contains(description))
			{
				result.Add(description);
			}	
		}
	}
}
