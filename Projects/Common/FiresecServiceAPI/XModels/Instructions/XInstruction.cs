using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XInstruction
	{
		public XInstruction()
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
		public XAlarmType AlarmType { get; set; }

		[DataMember]
		public XInstructionType InstructionType { get; set; }

		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

		[DataMember]
		public List<Guid> Devices { get; set; }

        [DataMember]
        public List<Guid> Directions { get; set; }

        [DataMember]
		public string Text { get; set; }

		public bool HasText
		{
			get
			{
				return !String.IsNullOrWhiteSpace(Text);
			}
		}

		[DataMember]
		public string MediaSource { get; set; }

		public bool HasMedia
		{
			get
			{
				return File.Exists(MediaSource);
			}
		}
    }
}