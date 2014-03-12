using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using XFiresecAPI;

namespace FiresecAPI.Models
{
	[DataContract]
	public class Camera
	{
		public Camera()
		{
			UID = Guid.NewGuid();
			ZoneUIDs = new List<Guid>();
			PlanElementUIDs = new List<Guid>();
			Width = 300;
			Height = 300;
			AllowMultipleVizualization = false;
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Address { get; set; }

		[DataMember]
		public string Login { get; set; }

		[DataMember]
		public string Password { get; set; }

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
		public XStateClass StateClass {get; set;}

		public XStateClass CameraStateStateClass
		{
			get { return XStateClass.Norm; }
		}

		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		[DataMember]
		public bool AllowMultipleVizualization { get; set; }

		public void OnChanged()
		{
			if (Changed != null)
				Changed();
		}

		public string PresentationName
		{
			get { return Name + " " + Address; }
		}

		public event Action Changed;
	}
}