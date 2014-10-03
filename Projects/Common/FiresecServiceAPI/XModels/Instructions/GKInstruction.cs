using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKInstruction
	{
		public GKInstruction()
		{
			UID = Guid.NewGuid();
			ZoneUIDs = new List<Guid>();
			Devices = new List<Guid>();
			Directions = new List<Guid>();
			Name = "";
			Text = "";
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public GKAlarmType AlarmType { get; set; }

		[DataMember]
		public GKInstructionType InstructionType { get; set; }

		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

		[DataMember]
		public List<Guid> Devices { get; set; }

		[DataMember]
		public List<Guid> Directions { get; set; }

		[DataMember]
		public string Text { get; set; }

		[XmlIgnore]
		public bool HasText
		{
			get
			{
				return !String.IsNullOrWhiteSpace(Text);
			}
		}

		[DataMember]
		public string MediaSource { get; set; }

		[XmlIgnore]
		public bool HasMedia
		{
			get
			{
				return File.Exists(MediaSource);
			}
		}
	}
}