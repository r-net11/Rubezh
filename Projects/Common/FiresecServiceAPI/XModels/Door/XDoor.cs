using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	public class XDoor : INamedBase, IIdentity
	{
		public XDoor()
		{
			UID = Guid.NewGuid();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public ushort No { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<Guid> DeviceUIDs { get; set; }

		public string PresentationName
		{
			get { return No + "." + Name; }
		}

		public void OnChanged()
		{
			if (Changed != null)
				Changed();
		}
		public event Action Changed;
	}
}