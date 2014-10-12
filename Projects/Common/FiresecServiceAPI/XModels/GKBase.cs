using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Базоый класс объектов ГК
	/// </summary>
	[DataContract]
	public abstract class GKBase : ModelBase, IStateProvider
	{
		public GKBase()
		{
			ClearDescriptor();
			ClearClauseDependencies();
		}

		[XmlIgnore]
		public List<GKDevice> ClauseInputDevices { get; set; }
		[XmlIgnore]
		public List<GKZone> ClauseInputZones { get; set; }
		[XmlIgnore]
		public List<GKGuardZone> ClauseInputGuardZones { get; set; }
		[XmlIgnore]
		public List<GKDirection> ClauseInputDirections { get; set; }
		[XmlIgnore]
		public List<GKMPT> ClauseInputMPTs { get; set; }
		[XmlIgnore]
		public List<GKDelay> ClauseInputDelays { get; set; }

		public void ClearClauseDependencies()
		{
			ClauseInputDevices = new List<GKDevice>();
			ClauseInputZones = new List<GKZone>();
			ClauseInputGuardZones = new List<GKGuardZone>();
			ClauseInputDirections = new List<GKDirection>();
			ClauseInputMPTs = new List<GKMPT>();
			ClauseInputDelays = new List<GKDelay>();
		}

		[XmlIgnore]
		public List<GKBase> InputXBases { get; set; }
		[XmlIgnore]
		public List<GKBase> OutputXBases { get; set; }

		[XmlIgnore]
		public GKDevice KauDatabaseParent { get; set; }
		[XmlIgnore]
		public GKDevice GkDatabaseParent { get; set; }

		[XmlIgnore]
		public ushort GKDescriptorNo { get; set; }
		[XmlIgnore]
		public ushort KAUDescriptorNo { get; set; }

		public void ClearDescriptor()
		{
			InputXBases = new List<GKBase>();
			OutputXBases = new List<GKBase>();
		}

		[XmlIgnore]
		public abstract GKBaseObjectType ObjectType { get; }
		[XmlIgnore]
		public string DescriptorPresentationName
		{
			get { return ObjectType.ToDescription() + " " + PresentationName; }
		}

		[XmlIgnore]
		public GKBaseInternalState InternalState { get; set; }
		[XmlIgnore]
		public GKState State { get; set; }

		#region IStateProvider Members

		IDeviceState<XStateClass> IStateProvider.StateClass
		{
			get { return State; }
		}

		Guid IIdentity.UID
		{
			get { return UID; }
		}

		#endregion
	}
}