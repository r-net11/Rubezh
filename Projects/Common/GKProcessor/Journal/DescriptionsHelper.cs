using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.Journal;

namespace GKProcessor
{
	public static class DescriptionsHelper
	{
		static List<Description> result;

		public static List<Description> GetAllDescriptions()
		{
			result = new List<Description>();
			for (int i = 1; i <= 255; i++)
			{
				AddToResult(JournalStringsHelper.ToFire((byte)i));
				AddToResult(JournalStringsHelper.ToFailure((byte)i));
				AddToResult(JournalStringsHelper.ToBatteryFailure((byte)i));
				AddToResult(JournalStringsHelper.ToInformation((byte)i));
				AddToResult(JournalStringsHelper.ToUser((byte)i));
			}
			foreach (JournalEventDescriptionType item in Enum.GetValues(typeof(JournalEventDescriptionType)))
			{
				AddToResult(item.ToDescription());
			}
			result.Sort(new DescriptionComparer());
			return result;
		}

		public class DescriptionComparer : IComparer<Description>
		{
			public int Compare(Description x, Description y)
			{
				string s1 = x.Name;
				string s2 = y.Name;
				return string.Compare(s1, s2);
			}
		}

		static void AddToResult(string descriptionName)
		{
			if (descriptionName != "" && !result.Any(x => x.Name == descriptionName))
			{
				result.Add(new Description(descriptionName));
			}
		}
	}
}