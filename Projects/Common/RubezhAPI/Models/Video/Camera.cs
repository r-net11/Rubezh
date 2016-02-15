using Common;
using Infrustructure.Plans.Interfaces;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class Camera : IStateProvider, IPlanPresentable
	{
		public Camera()
		{
			UID = Guid.NewGuid();
			PlanElementUIDs = new List<Guid>();
			AllowMultipleVizualization = false;
			CameraState = new CameraState(this);
		}
		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Ip { get; set; }

		[DataMember]
		public int StreamNo { get; set; }

		[DataMember]
		public int ChannelNumber { get; set; }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		[DataMember]
		public bool AllowMultipleVizualization { get; set; }
		[DataMember]
		public string RviDeviceName { get; set; }

		[DataMember]
		public Guid RviDeviceUID { get; set; }

		[DataMember]
		public int RviChannelNo { get; set; }

		[DataMember]
		public string RviChannelName { get; set; }

		[DataMember]
		public int CountPresets { get; set; }

		[DataMember]
		public int CountTemplateBypass { get; set; }

		[DataMember]
		public int CountTemplatesAutoscan { get; set; }
		[DataMember]
		public int ShowDetailsWidth { get; set; }
		[DataMember]
		public int ShowDetailsHeight { get; set; }
		[DataMember]
		public bool IsAddedInConfiguration { get; set; }
		[DataMember]
		public string RviServerUrl { get; set; }
		public int ShowDetailsMarginLeft { get; set; }
		public int ShowDetailsMarginTop { get; set; }

		public void OnChanged()
		{
			if (Changed != null)
				Changed();
		}
		public event Action Changed;

		public void OnUIDChanged(Guid oldUID, Guid newUID)
		{
			if (UIDChanged != null)
				UIDChanged(oldUID, newUID);
		}
		public event Action<Guid, Guid> UIDChanged;
		[XmlIgnore]
		public string Name
		{
			get { return string.Format("Поток {0}", StreamNo); }
		}
		[XmlIgnore]
		public string PresentationName
		{
			get { return string.Format("{0}. {1}. Поток {2}", RviDeviceName, RviChannelName, StreamNo); }
		}

		[XmlIgnore]
		public CameraState CameraState { get; private set; }

		//#region IStateProvider Members

		IDeviceState IStateProvider.StateClass { get { return CameraState; } }

		//#endregion

		//#region IDeviceState<XStateClass> Members

		//public XStateClass IDeviceState<XStateClass>.StateType
		//{
		//	get { return XStateClass.Norm; }
		//}

		//event Action IDeviceState<XStateClass>.StateChanged
		//{
		//	add { }
		//	remove { }
		//}

		//#endregion
	}
}