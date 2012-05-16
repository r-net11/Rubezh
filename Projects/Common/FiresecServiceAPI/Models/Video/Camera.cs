using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class Camera
	{
		public Camera()
		{
			Zones = new List<ulong>();
			Width = 300;
			Height = 300;
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Address { get; set; }

		[DataMember]
		public int Left { get; set; }

		[DataMember]
		public int Top { get; set; }

		[DataMember]
		public int Width { get; set; }

		[DataMember]
		public int Height { get; set; }

		[DataMember]
		public bool IgnoreMoveResize { get; set; }

		[DataMember]
		public StateType StateType { get; set; }

		[DataMember]
		public List<ulong> Zones { get; set; }
	}
}