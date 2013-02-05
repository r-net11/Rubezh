using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XDelay : XBinaryBase
	{
		public XDelay()
		{
			InitialState = false;
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public bool InitialState { get; set; }

		[DataMember]
		public ushort DelayTime { get; set; }

		[DataMember]
		public ushort SetTime { get; set; }

		public override XBaseState GetXBaseState() { return null; }

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
}