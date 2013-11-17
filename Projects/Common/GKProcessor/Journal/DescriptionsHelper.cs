using System.Collections.Generic;
using System.Linq;
using System.Collections;
using FiresecAPI.Models;

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
				AddToResult(JournalStringsHelper.ToFire((byte)i), DescriptionType.Fire);
				AddToResult(JournalStringsHelper.ToFailure((byte)i), DescriptionType.Failure);
				AddToResult(JournalStringsHelper.ToBUSHFailure((byte)i), DescriptionType.Failure);
				AddToResult(JournalStringsHelper.ToInformation((byte)i), DescriptionType.Information);
				AddToResult(JournalStringsHelper.ToBUSHInformation((byte)i), DescriptionType.Information);
				AddToResult(JournalStringsHelper.ToUser((byte)i), DescriptionType.User);
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

		static void AddToResult(string descriptionName, DescriptionType descriptionType)
		{
			if (descriptionName != "" && !result.Any(x => x.Name == descriptionName))
			{
				result.Add(new Description(descriptionName, descriptionType));
			}
		}
	}
}