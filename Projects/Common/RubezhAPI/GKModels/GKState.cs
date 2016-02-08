﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;

namespace RubezhAPI.GK
{
	[DataContract]
	public class GKState : IDeviceState
	{
		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public Guid ReferenceUid { get; set; }

		[DataMember]
		public bool IsReservedState { get; set; }

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
		public int RunningTime { get; set; }

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
			BaseObjectType = GKBaseObjectType.Device;
		}

		public GKState(GKZone zone)
			: this()
		{
			Zone = zone;
			UID = zone.UID;
			BaseObjectType = GKBaseObjectType.Zone;
		}

		public GKState(GKDirection direction)
			: this()
		{
			Direction = direction;
			UID = direction.UID;
			BaseObjectType = GKBaseObjectType.Direction;
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
			ReferenceUid = pim.DeviceUid;
			BaseObjectType = GKBaseObjectType.Pim;
		}

		public GKState(GKGuardZone guardZone)
			: this()
		{
			GuardZone = guardZone;
			UID = guardZone.UID;
			BaseObjectType = GKBaseObjectType.GuardZone;
		}

		public GKState(GKCode code)
			: this()
		{
			Code = code;
			UID = code.UID;
			BaseObjectType = GKBaseObjectType.Code;
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
		public GKZone Zone { get; private set; }
		public GKDirection Direction { get; private set; }
		public GKPumpStation PumpStation { get; private set; }
		public GKMPT MPT { get; private set; }
		public GKDelay Delay { get; private set; }
		public GKPim Pim { get; private set; }
		public GKGuardZone GuardZone { get; private set; }
		public GKCode Code { get; private set; }
		public GKDoor Door { get; private set; }
		public GKSKDZone SKDZone { get; private set; }
		public GKBaseObjectType BaseObjectType { get; private set; }

		public void CopyTo(GKState state)
		{
			state.AdditionalStates = AdditionalStates;
			state.OnDelay = OnDelay;
			state.HoldDelay = HoldDelay;
			state.OffDelay = OffDelay;
			state.RunningTime = RunningTime;
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
					case GKBaseObjectType.Device:
						switch (StateClass)
						{
							case XStateClass.Fire1:
								return "Сработка 1";
							case XStateClass.Fire2:
								return "Сработка 2";
						}
						break;

					case GKBaseObjectType.GuardZone:
						switch (StateClass)
						{
							case XStateClass.Fire1:
								return "Тревога";
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