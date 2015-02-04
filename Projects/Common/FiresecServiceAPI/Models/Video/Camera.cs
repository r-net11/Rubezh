using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using FiresecAPI.GK;
using Infrustructure.Plans.Interfaces;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class Camera : IStateProvider, IDeviceState<XStateClass>, IPlanPresentable
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
		public string Ip { get; set; }

		[DataMember]
		public int StreamNo { get; set; }

		[DataMember]
		public int ChannelNumber { get; set; }

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

		[DataMember]
		public int CountPresets { get; set; }

		[DataMember]
		public int CountTemplateBypass { get; set; }

		[DataMember]
		public int CountTemplatesAutoscan { get; set; }

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
			//return XStateClass
			get { return this; }
		}

		#endregion

		#region IDeviceState<XStateClass> Members

		XStateClass IDeviceState<XStateClass>.StateType
		{
			//get { return StateClass; }
			get { return XStateClass.Norm; }
		}

		event Action IDeviceState<XStateClass>.StateChanged
		{
			add { }
			remove { }
		}

		#endregion
	}
}