using System;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XPim : XBase
	{
		public XPim()
		{
			UID = Guid.NewGuid();
			PimState = new XPimState()
			{
				Pim = this
			};
		}

		public XPimState PimState { get; set; }
		public override XBaseState GetXBaseState() { return PimState; }

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

		public override string PresentationName
		{
			get { return Name; }
		}

		public override string DescriptorInfo
		{
			get { return "ПИМ " + Name; }
		}

		public override string GetDescriptorName()
		{
			return Name;
		}
	}
}