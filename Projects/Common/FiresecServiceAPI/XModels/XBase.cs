using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public abstract class XBase
	{
		public XBase()
		{
			ClearBinaryData();
		}

		public List<XBase> InputXBases { get; set; }
		public List<XBase> OutputXBases { get; set; }

		public XDevice KauDatabaseParent { get; set; }
		public XDevice GkDatabaseParent { get; set; }

		public ushort GKDescriptorNo { get; set; }
		public ushort KAUDescriptorNo { get; set; }

		public void ClearBinaryData()
		{
			InputXBases = new List<XBase>();
			OutputXBases = new List<XBase>();
		}

		public ushort GetNearestDatabaseNo()
		{
			if (KauDatabaseParent != null)
				return KAUDescriptorNo;
			if (GkDatabaseParent != null)
				return GKDescriptorNo;
			return 0;
		}

		public abstract string DescriptorInfo { get; }
		public abstract string GetDescriptorName();
		public abstract XBaseState GetXBaseState();
	}
}