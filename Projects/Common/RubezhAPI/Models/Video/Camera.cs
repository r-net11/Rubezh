using RubezhAPI.GK;
using RubezhAPI.Plans.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class Camera : IStateProvider, IPlanPresentable
	{
		public Camera()
		{
			PlanElementUIDs = new List<Guid>();
			AllowMultipleVizualization = false;
			CameraState = new CameraState(this);
			RviStreams = new List<RviStream>();
			ShowDetailsWidth = 600;
			ShowDetailsHeight = 600;
			ShowDetailsMarginLeft = 500;
			ShowDetailsMarginTop = 500;
		}
		[DataMember]
		public List<RviStream> RviStreams { get; set; }
		[DataMember]
		public int SelectedRviStreamNumber { get; set; }
		[XmlIgnore]
		public RviStream SelectedRviStream { get { return RviStreams.Single(x => x.Number == SelectedRviStreamNumber); } }
		[DataMember]
		public Guid UID { get; set; }
		[DataMember]
		public string Ip { get; set; }
		[DataMember]
		public bool AllowMultipleVizualization { get; set; }
		[DataMember]
		public string RviDeviceName { get; set; }
		[DataMember]
		public Guid RviDeviceUID { get; set; }
		[DataMember]
		public int Number { get; set; }
		[DataMember]
		public string Name { get; set; }

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
		public string RviServerUrl { get; set; }
		[DataMember]
		public int ShowDetailsMarginLeft { get; set; }
		[DataMember]
		public int ShowDetailsMarginTop { get; set; }
		[DataMember]
		public int Vendor { get; set; }

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

		public void OnStatusChanged()
		{
			if (StatusChanged != null)
				StatusChanged();
			if (CameraState != null)
				CameraState.OnStateChanged();
		}
		public event Action StatusChanged;
		[XmlIgnore]
		public List<Guid> PlanElementUIDs { get; set; }
		public void OnPlanElementUIDsChanged()
		{
			if (PlanElementUIDsChanged != null)
				PlanElementUIDsChanged();
		}
		public event Action PlanElementUIDsChanged;
		[XmlIgnore]
		public RviStatus Status { get; set; }
		[XmlIgnore]
		public bool IsRecordOnline { get; set; }
		[XmlIgnore]
		public bool IsOnGuard { get; set; }
		[XmlIgnore]
		public string PresentationName
		{
			get { return string.Format("{0}. {1}", RviDeviceName, Name); }
		}
		[XmlIgnore]
		public CameraState CameraState { get; set; }
		IDeviceState IStateProvider.StateClass { get { return CameraState; } }

		[XmlIgnore]
		public string ImageSource { get { return "/Controls;component/RviDevicesIcons/Camera.png"; } }
	}
}