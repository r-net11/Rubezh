using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using Entities.DeviceOriented;
using FiresecAPI.GK;
using Infrustructure.Plans.Interfaces;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class Camera : IStateProvider, IDeviceState<XStateClass>, IIdentity, IPlanPresentable
	{
		public Camera()
		{
			UID = Guid.NewGuid();
			Children = new List<Camera>();
			ZoneUIDs = new List<Guid>();
			PlanElementUIDs = new List<Guid>();
			Width = 300;
			Height = 300;
			AllowMultipleVizualization = false;
			Status = DeviceStatuses.Disconnected;

			Name = "Новая камера";
		}

		[XmlIgnore]
		public Camera Parent { get; set; }

		[XmlIgnore]
		public string ImageSource
		{
			get { return "/Controls;component/Images/" + this.CameraType.ToString() + ".png"; }
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Ip { get; set; }

		[DataMember]
		public int StreamNo { get; set; }

		[XmlIgnore]
		public DeviceStatuses Status { get; set; }

		[DataMember]
		public int ChannelNumber { get; set; }

		[DataMember]
		public List<Camera> Children { get; set; }

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
		public XStateClass StateClass { get; set; }

		[DataMember]
		public CameraType CameraType { get; set; }

		[XmlIgnore]
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

		[DataMember]
		public Guid RviDeviceUID { get; set; }

		[DataMember]
		public int RviChannelNo { get; set; }

		[DataMember]
		public string RviRTSP { get; set; }

		public void OnChanged()
		{
			if (Changed != null)
				Changed();
		}

		[XmlIgnore]
		public string PresentationName
		{
			get { return Name + " " + Ip; }
		}

		public event Action Changed;

		#region IStateProvider Members

		IDeviceState<XStateClass> IStateProvider.StateClass
		{
			get { return this; }
		}

		#endregion

		#region IDeviceState<XStateClass> Members

		XStateClass IDeviceState<XStateClass>.StateType
		{
			get { return StateClass; }
		}

		event Action IDeviceState<XStateClass>.StateChanged
		{
			add { }
			remove { }
		}

		#endregion
	}
}