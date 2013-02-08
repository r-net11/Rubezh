using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace XFiresecAPI
{
	[DataContract]
	public class XDelay : XBinaryBase
	{
		public XDelay()
		{
			UID = Guid.NewGuid();
			DelayState = new XDelayState()
			{
				Delay = this
			};
		}

		public XDelayState DelayState { get; set; }
		public override XBaseState GetXBaseState() { return DelayState; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public ushort DelayTime { get; set; }

		[DataMember]
		public ushort SetTime { get; set; }

		[DataMember]
		public DelayRegime DelayRegime { get; set; }

		public override XBinaryInfo BinaryInfo
		{
			get
			{
				return new XBinaryInfo()
				{
					Type = "Задержка",
					Name = Name,
					Address = ""
				};
			}
		}

		public override string GetBinaryDescription()
		{
			return Name;
		}
	}

	public enum DelayRegime
	{
		[DescriptionAttribute("Выключено")]
		Off = 0,

		[DescriptionAttribute("Включено")]
		On = 1
	}
}