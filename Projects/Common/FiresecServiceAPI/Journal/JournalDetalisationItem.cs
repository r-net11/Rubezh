using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.Journal
{
	[DataContract]
	public class JournalDetalisationItem
	{
		public JournalDetalisationItem()
		{
		}

		public JournalDetalisationItem(string name, string value)
		{
			Name = name;
			Value = value;
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Value { get; set; }

		public static string ListToString(List<JournalDetalisationItem> detalisations)
		{
			var result = string.Empty;
			foreach (var detalisation in detalisations)
			{
				result += "$%" + detalisation.Name + "%%" + detalisation.Value + "%$";
			}
			return result;
		}

		public static List<JournalDetalisationItem> StringToList(string detalisation)
		{
			var detalisationStringsItems = detalisation.Split(new[] { "$" }, StringSplitOptions.RemoveEmptyEntries);
			var result = new List<JournalDetalisationItem>();
			foreach (var detSubString in detalisationStringsItems)
			{
				var nameValueString = detSubString.Split(new[] { "%" }, StringSplitOptions.RemoveEmptyEntries);
				if (nameValueString.Length >= 2)
				{
					result.Add(new JournalDetalisationItem(nameValueString[0], nameValueString[1]));
				}
			}
			return result;
		}
	}
}