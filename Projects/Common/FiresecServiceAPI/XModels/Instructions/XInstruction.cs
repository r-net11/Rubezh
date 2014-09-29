using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
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

		public Guid UID { get; set; }
		public string Name { get; set; }
		public XAlarmType AlarmType { get; set; }
		public XInstructionType InstructionType { get; set; }
		public List<Guid> ZoneUIDs { get; set; }
		public List<Guid> Devices { get; set; }
		public List<Guid> Directions { get; set; }
		public string Text { get; set; }
		public string MediaSource { get; set; }

		[XmlIgnore]
		public bool HasText
		{
			get
			{
				return !String.IsNullOrWhiteSpace(Text);
			}
		}

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