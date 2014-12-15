using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using System.Xml.Serialization;
using System.Linq;

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
			State = new GKState();
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
		public List<GKBase> InputGKBases { get; set; }
		[XmlIgnore]
		public List<GKBase> OutputGKBases { get; set; }

		[XmlIgnore]
		public GKDevice KauDatabaseParent { get; set; }
		[XmlIgnore]
		public GKDevice GkDatabaseParent { get; set; }

		[XmlIgnore]
		public virtual bool IsLogicOnKau { get; set; }
		[XmlIgnore]
		public ushort GKDescriptorNo { get; set; }
		[XmlIgnore]
		public ushort KAUDescriptorNo { get; set; }

		void PrepareInputOutputDependences()
		{
			ClearDescriptor();
			var device = this as GKDevice;
			var zone = this as GKZone;
			var direction = this as GKDirection;
			var pumpStation = this as GKPumpStation;
			var mpt = this as GKMPT;
			var delay = this as GKDelay;
			var guardZone = this as GKGuardZone;
			var door = this as GKDoor;

			if (device != null)
			{
				LinkLogic(device, device.Logic.OnClausesGroup);
				LinkLogic(device, device.Logic.OffClausesGroup);
				LinkLogic(device, device.Logic.StopClausesGroup);
			}

			if (zone != null)
			{
				foreach (var zoneDevice in zone.Devices)
				{
					zone.LinkGKBases(zoneDevice);
				}
				zone.LinkGKBases(zone);
			}

			if (direction != null)
			{
				LinkLogic(direction, direction.Logic.OnClausesGroup);
				LinkLogic(direction, direction.Logic.OffClausesGroup);
			}

			if (pumpStation != null)
			{
				LinkLogic(pumpStation, pumpStation.StartLogic.OnClausesGroup);
				LinkLogic(pumpStation, pumpStation.StopLogic.OnClausesGroup);
				LinkLogic(pumpStation, pumpStation.AutomaticOffLogic.OnClausesGroup);
			}

			if (mpt != null)
			{
				LinkLogic(mpt, mpt.StartLogic.OnClausesGroup);
			}

			if (delay != null)
			{
				LinkLogic(delay, delay.Logic.OnClausesGroup);
				LinkLogic(delay, delay.Logic.OffClausesGroup);
			}

			if (guardZone != null)
			{
				foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
				{
					if (guardZoneDevice.ActionType != GKGuardZoneDeviceActionType.ChangeGuard)
					{
						guardZone.LinkGKBases(guardZoneDevice.Device);
						if (guardZoneDevice.Device.DriverType == GKDriverType.RSR2_GuardDetector || guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CodeReader)
						{
							guardZoneDevice.Device.LinkGKBases(guardZone);
						}
					}
				}
				guardZone.LinkGKBases(guardZone);
			}

			if (door != null)
			{
				if (door.EnterDevice != null)
					door.LinkGKBases(door.EnterDevice);
				if (door.ExitDevice != null)
					door.LinkGKBases(door.ExitDevice);
				if (door.LockDevice != null)
					door.LockDevice.LinkGKBases(door);
				if (door.LockControlDevice != null)
					door.LinkGKBases(door.LockControlDevice);
			}
		}

		void LinkLogic(GKBase gkBase, GKClauseGroup clauseGroup)
		{
			if (clauseGroup.Clauses != null)
			{
				foreach (var clause in clauseGroup.Clauses)
				{
					foreach (var clauseDevice in clause.Devices)
						gkBase.LinkGKBases(clauseDevice);
					foreach (var zone in clause.Zones)
						gkBase.LinkGKBases(zone);
					foreach (var guardZone in clause.GuardZones)
						gkBase.LinkGKBases(guardZone);
					foreach (var direction in clause.Directions)
						gkBase.LinkGKBases(direction);
					foreach (var mpt in clause.MPTs)
						gkBase.LinkGKBases(mpt);
					foreach (var delay in clause.Delays)
						gkBase.LinkGKBases(delay);
				}
			}
			if (clauseGroup.ClauseGroups != null)
			{
				foreach (var group in clauseGroup.ClauseGroups)
				{
					LinkLogic(gkBase, group);
				}
			}
		}

		public void LinkGKBases(GKBase dependsOn)
		{
			AddInputOutputObject(this.InputGKBases, dependsOn);
			AddInputOutputObject(dependsOn.OutputGKBases, this);
		}

		void AddInputOutputObject(List<GKBase> objects, GKBase newObject)
		{
			if (!objects.Any(x => x.UID == newObject.UID))
				objects.Add(newObject);
		}


		public void ClearDescriptor()
		{
			InputGKBases = new List<GKBase>();
			OutputGKBases = new List<GKBase>();
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

		GKDevice GetDataBaseParent(List<GKDevice> devices)
		{
			var allDependentDevices = new List<GKDevice>(devices);
			foreach (var device in devices)
			{

			}
			List<GKDevice> kauParents = allDependentDevices.Select(x => x.KAUParent).ToList();
			if (kauParents != null && kauParents.Count == 1)
				return kauParents.FirstOrDefault();
			else
				return devices.FirstOrDefault().GKParent;
		}

		public GKDevice GetDataBaseParent()
		{
			PrepareInputOutputDependences();
			var allDependentDevices = GetFullTree(this);
			var kauParents = allDependentDevices.Select(x => x.KAUParent).Distinct().ToList();
			if (kauParents != null && kauParents.Count == 1 && kauParents.FirstOrDefault() != null)
				return kauParents.FirstOrDefault();
			else
			{
				if (this is GKDevice)
					return (this as GKDevice).GKParent;
				else
				{
					if (allDependentDevices != null && allDependentDevices.Count > 0)
						return allDependentDevices.FirstOrDefault().GKParent;
					else
						return null;
				}
			}
		}

		List<GKDevice> GetFullTree(GKBase gkBase)
		{
			return GetAllDependentObjects(gkBase).Where(x => x is GKDevice).Cast<GKDevice>().ToList();
		}

		List<GKBase> GetAllDependentObjects(GKBase gkBase) // TODO FIX IT
		{
			var result = new List<GKBase>();
			var inputObjects = gkBase.InputGKBases;
			inputObjects.RemoveAll(x => x == gkBase);
			result.AddRange(inputObjects);
			foreach (var inputObject in inputObjects)
			{
				var list = GetAllDependentObjects(inputObject);
				result.AddRange(list.FindAll(x => !result.Contains(x)));
			}
			return result;
		}

		//List<GKBase> GetAllDependentObjects(GKBase gkBase, List<GKBase> currentList)
		//{
		//    var result = new List<GKBase>();
		//    var inputObjects = gkBase.InputGKBases;
		//    inputObjects.RemoveAll(x => x == gkBase);
		//    foreach (var current in currentList)
		//        inputObjects.RemoveAll(x => x == current);
		//    result.AddRange(inputObjects);
		//    foreach (var inputObject in inputObjects)
		//        result.AddRange(GetAllDependentObjects(inputObject, inputObjects));
		//    return result;
		//}
	}
}