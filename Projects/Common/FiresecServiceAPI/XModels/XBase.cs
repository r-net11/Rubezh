using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public abstract class XBase : ModelBase, IStateProvider
	{
		public XBase()
		{
			ClearDescriptor();
			ClearClauseDependencies();
		}

		[XmlIgnore]
		public List<XDevice> ClauseInputDevices { get; set; }
		[XmlIgnore]
		public List<XZone> ClauseInputZones { get; set; }
		[XmlIgnore]
		public List<XGuardZone> ClauseInputGuardZones { get; set; }
		[XmlIgnore]
		public List<XDirection> ClauseInputDirections { get; set; }
		[XmlIgnore]
		public List<XMPT> ClauseInputMPTs { get; set; }
		[XmlIgnore]
		public List<XDelay> ClauseInputDelays { get; set; }

		public void ClearClauseDependencies()
		{
			ClauseInputDevices = new List<XDevice>();
			ClauseInputZones = new List<XZone>();
			ClauseInputGuardZones = new List<XGuardZone>();
			ClauseInputDirections = new List<XDirection>();
			ClauseInputMPTs = new List<XMPT>();
			ClauseInputDelays = new List<XDelay>();
		}

		[XmlIgnore]
		public List<XBase> InputXBases { get; set; }
		[XmlIgnore]
		public List<XBase> OutputXBases { get; set; }

		[XmlIgnore]
		public XDevice KauDatabaseParent { get; set; }
		[XmlIgnore]
		public XDevice GkDatabaseParent { get; set; }

		[XmlIgnore]
		public ushort GKDescriptorNo { get; set; }
		[XmlIgnore]
		public ushort KAUDescriptorNo { get; set; }

		public void ClearDescriptor()
		{
			InputXBases = new List<XBase>();
			OutputXBases = new List<XBase>();
		}

		[XmlIgnore]
		public abstract XBaseObjectType ObjectType { get; }
		[XmlIgnore]
		public string DescriptorPresentationName
		{
			get { return ObjectType.ToDescription() + " " + PresentationName; }
		}

		[XmlIgnore]
		public XBaseInternalState InternalState { get; set; }
		[XmlIgnore]
		public XState State { get; set; }

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