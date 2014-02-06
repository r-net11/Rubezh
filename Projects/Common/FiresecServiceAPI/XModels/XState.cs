using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XState
	{
		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public List<XStateClass> StateClasses { get; set; }

		[DataMember]
		public XStateClass StateClass { get; set; }

		[DataMember]
		public List<XAdditionalState> AdditionalStates { get; set; }

		[DataMember]
		public int OnDelay { get; set; }

		[DataMember]
		public int HoldDelay { get; set; }

		[DataMember]
		public int OffDelay { get; set; }

		[DataMember]
		public List<XMeasureParameterValue> XMeasureParameterValues { get; set; }

		[DataMember]
		public string PresentationName { get; set; }

		public event Action StateChanged;
		public void OnStateChanged()
		{
			if (StateChanged != null)
				StateChanged();
		}

		public event Action MeasureParametersChanged;
		public void OnMeasureParametersChanged()
		{
			if (MeasureParametersChanged != null)
				MeasureParametersChanged();
		}

		public XState()
		{
			AdditionalStates = new List<XAdditionalState>();
			StateClasses = new List<XStateClass>();
			StateClass = XStateClass.Unknown;
			XMeasureParameterValues = new List<XMeasureParameterValue>();
		}

		public XState(XDevice device)
			: this()
		{
			Device = device;
			UID = device.UID;
			BaseObjectType = XBaseObjectType.Deivce;
		}

		public XState(XZone zone)
			: this()
		{
			Zone = zone;
			UID = zone.UID;
			BaseObjectType = XBaseObjectType.Zone;
		}

		public XState(XDirection direction)
			: this()
		{
			Direction = direction;
			UID = direction.UID;
			BaseObjectType = XBaseObjectType.Direction;
		}

		public XState(XPumpStation pumpStation)
			: this()
		{
			PumpStation = pumpStation;
			UID = pumpStation.UID;
			BaseObjectType = XBaseObjectType.PumpStation;
		}

		public XState(XDelay delay)
			: this()
		{
			Delay = delay;
			UID = delay.UID;
			BaseObjectType = XBaseObjectType.Delay;
		}

		public XState(XPim pim)
			: this()
		{
			Pim = pim;
			UID = pim.UID;
			BaseObjectType = XBaseObjectType.Pim;
		}

		public XDevice Device { get; private set; }
		public XZone Zone { get; private set; }
		public XDirection Direction { get; private set; }
		public XPumpStation PumpStation { get; private set; }
		public XDelay Delay { get; private set; }
		public XPim Pim { get; private set; }
		public XBaseObjectType BaseObjectType { get; private set; }

		public void CopyTo(XState state)
		{
			state.AdditionalStates = AdditionalStates;
			state.OnDelay = OnDelay;
			state.HoldDelay = HoldDelay;
			state.OffDelay = OffDelay;
			state.StateClasses = StateClasses;
			state.StateClass = StateClass;
		}
	}
}