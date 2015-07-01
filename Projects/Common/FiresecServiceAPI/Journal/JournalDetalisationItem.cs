﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Journal
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
			string result = "";
			foreach (var detalisation in detalisations)
			{
				result += "$%" + detalisation.Name + "%%" + detalisation.Value + "%$";
			}
			return result;
		}

		public static List<JournalDetalisationItem> StringToList(string detalisation)
		{
            var result = new List<JournalDetalisationItem>();
            if (detalisation == null)
                return result;
            var detalisationStringsItems = detalisation.Split(new string[] { "$" }, StringSplitOptions.RemoveEmptyEntries);
			
			foreach (var detSubString in detalisationStringsItems)
			{
				var nameValueString = detSubString.Split(new string[] { "%" }, StringSplitOptions.RemoveEmptyEntries);
				if (nameValueString.Length >= 2)
				{
					result.Add(new JournalDetalisationItem(nameValueString[0], nameValueString[1]));
				}
			};
			return result;
		}
	}
}