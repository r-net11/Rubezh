using System.Collections.Generic;

namespace GKProcessor
{
	public static class DescriptionsHelper
	{
		static List<string> result;

		public static List<string> GetAllDescriptions()
		{
			result = new List<string>();
			for (int i = 1; i <= 255; i++)
			{
				AddToResult(JournalStringsHelper.ToFailure((byte)i));
				AddToResult(JournalStringsHelper.ToBUSHFailure((byte)i));
				AddToResult(JournalStringsHelper.ToInformation((byte)i));
				AddToResult(JournalStringsHelper.ToBUSHInformation((byte)i));
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