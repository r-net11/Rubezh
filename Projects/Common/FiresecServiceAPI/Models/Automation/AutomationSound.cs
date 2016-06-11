using System;
using System.Runtime.Serialization;
using StrazhAPI.Models;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class AutomationSound
	{
		public AutomationSound()
		{
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public SoundLibraryType SoundLibraryType { get; set; }
	}
}