using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKState : IDeviceState
	{
		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public List<XStateClass> StateClasses { get; set; }

		[DataMember]
		public XStateClass StateClass { get; set; }

		[DataMember]
		public List<GKAdditionalState> AdditionalStates { get; set; }

		[DataMember]
		public int OnDelay { get; set; }

		[DataMember]
		public int HoldDelay { get; set; }

		[DataMember]
		public int OffDelay { get; set; }

		[DataMember]
		public List<GKMeasureParameterValue> XMeasureParameterValues { get; set; }

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

		public GKState()
		{
			AdditionalStates = new List<GKAdditionalState>();
			StateClasses = new List<XStateClass>();
			StateClass = XStateClass.Unknown;
			XMeasureParameterValues = new List<GKMeasureParameterValue>();
		}

		public GKState(GKDevice device)
			: this()
		{
			Device = device;
			UID = device.UID;
			BaseObjectType = GKBaseObjectType.Deivce;
		}

		public GKState(GKPumpStation pumpStation)
			: this()
		{
			PumpStation = pumpStation;
			UID = pumpStation.UID;
			BaseObjectType = GKBaseObjectType.PumpStation;
		}

		public GKState(GKMPT mpt)
			: this()
		{
			MPT = mpt;
			UID = mpt.UID;
			BaseObjectType = GKBaseObjectType.PumpStation;
		}

		public GKState(GKDelay delay)
			: this()
		{
			Delay = delay;
			UID = delay.UID;
			BaseObjectType = GKBaseObjectType.Delay;
		}

		public GKState(GKPim pim)
			: this()
		{
			Pim = pim;
			UID = pim.UID;
			BaseObjectType = GKBaseObjectType.Pim;
		}

		public GKState(GKDoor door)
			: this()
		{
			Door = door;
			UID = door.UID;
			BaseObjectType = GKBaseObjectType.Door;
		}

		public GKState(GKSKDZone zone)
			: this()
		{
			SKDZone = zone;
			UID = zone.UID;
			BaseObjectType = GKBaseObjectType.SKDZone;
		}

		public GKDevice Device { get; private set; }
		public GKPumpStation PumpStation { get; private set; }
		public GKMPT MPT { get; private set; }
		public GKDelay Delay { get; private set; }
		public GKPim Pim { get; private set; }
		public GKDoor Door { get; private set; }
		public GKSKDZone SKDZone { get; private set; }
		public GKBaseObjectType BaseObjectType { get; private set; }

		public void CopyTo(GKState state)
		{
			state.AdditionalStates = AdditionalStates;
			state.OnDelay = OnDelay;
			state.HoldDelay = HoldDelay;
			state.OffDelay = OffDelay;
			state.StateClasses = StateClasses;
			state.StateClass = StateClass;
		}

		#region IDeviceState<XStateClass> Members
		XStateClass IDeviceState.StateClass
		{
			get { return StateClass; }
		}
		string IDeviceState.Name
		{
			get
			{
				switch (BaseObjectType)
				{
					case GKBaseObjectType.Deivce:
						switch (StateClass)
						{
							case XStateClass.Fire1:
								return "Сработка 1";
							case XStateClass.Fire2:
								return "Сработка 2";
						}
						break;

					case GKBaseObjectType.Door:
						switch (StateClass)
						{
							case XStateClass.Fire1:
								return "Тревога";
						}
						break;

				}
				return StateClass.ToDescription();
			}
		}
		#endregion
	}
}