using System;
using System.Runtime.Serialization;
using FiresecAPI.GK;
using FiresecAPI.SKD;

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
	}
}