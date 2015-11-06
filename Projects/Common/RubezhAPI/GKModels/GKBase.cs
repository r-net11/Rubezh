﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Common;
using RubezhClient;

namespace RubezhAPI.GK
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
			InputDescriptors = new List<GKBase>();
			OutputDescriptors = new List<GKBase>();
			InputDependentElements = new List<GKBase>();
			OutputDependentElements = new List<GKBase>();
		}

		/// <summary>
		/// Коллекция объектов от которых данный объекто зависит. Например, объекты в логике
		/// </summary>
		[XmlIgnore]
		public List<GKBase> InputDependentElements { get; set; }

		/// <summary>
		/// Коллекция объектов, которые зависят от данного объекта. Например, объекты, в логике которых участвует данный объект
		/// </summary>
		[XmlIgnore]
		public List<GKBase> OutputDependentElements { get; set; }


		public void ClearClauseDependencies()
		{
			InputDescriptors = new List<GKBase>();
			OutputDescriptors = new List<GKBase>();
			InputDependentElements = new List<GKBase>();
			OutputDependentElements = new List<GKBase>();
		}

		[XmlIgnore]
		public List<GKBase> InputDescriptors { get; set; }
		[XmlIgnore]
		public List<GKBase> OutputDescriptors { get; set; }

		public virtual void Invalidate(GKDeviceConfiguration deviceConfiguration)
		{
		}

		public virtual void UpdateLogic(GKDeviceConfiguration deviceConfiguration)
		{
		}

		public void ChangedLogic()
		{
			InputDependentElements.ForEach(x =>
				{
					x.OutputDependentElements.Remove(this);
				});
			InputDependentElements = new List<GKBase>();
			this.Invalidate(GKManager.DeviceConfiguration);
		}

		public void AddDependentElement(GKBase gkBase)
		{
			if (InputDependentElements.All(x => x.UID != gkBase.UID) && gkBase.UID != UID)
				InputDependentElements.Add(gkBase);
			if (gkBase.OutputDependentElements.All(x => x.UID != UID) && gkBase.UID != UID)
				gkBase.OutputDependentElements.Add(this);
		}

		public virtual string GetGKDescriptorName(GKNameGenerationType gkNameGenerationType)
		{
			var result = PresentationName;
			if (result.Length > 32)
				result = result.Substring(0, 32);
			return result.TrimEnd(' ');
		}

		[XmlIgnore]
		public abstract GKBaseObjectType ObjectType { get; }
		[XmlIgnore]
		public string DescriptorPresentationName
		{
			get { return ObjectType.ToDescription() + " " + PresentationName; }
		}

		[XmlIgnore]
		public abstract string ImageSource { get; }

		[XmlIgnore]
		public GKBaseInternalState InternalState { get; set; }
		[XmlIgnore]
		public GKState State { get; set; }

		#region IStateProvider Members

		IDeviceState IStateProvider.StateClass
		{
			get { return State; }
		}

		Guid IIdentity.UID
		{
			get { return UID; }
		}

		#endregion

		#region Descriptors

		[XmlIgnore]
		public HashSet<GKDevice> KauParents { get; set; }
		[XmlIgnore]
		public HashSet<GKDevice> GkParents { get; set; }

		[XmlIgnore]
		public GKDevice KauDatabaseParent { get; set; }
		[XmlIgnore]
		public GKDevice GkDatabaseParent { get; set; }

		[XmlIgnore]
		public bool MagnetToGK { get; set; }

		[XmlIgnore]
		public virtual bool IsLogicOnKau { get; set; }

		[XmlIgnore]
		public ushort GKDescriptorNo { get; set; }
		[XmlIgnore]
		public ushort KAUDescriptorNo { get; set; }

		public void PrepareInputOutputDependences()
		{
			var device = this as GKDevice;
			if (device != null)
			{
				LinkLogic(device, device.Logic.OnClausesGroup);
				if (!device.Logic.UseOffCounterLogic)
					LinkLogic(device, device.Logic.OffClausesGroup);
				LinkLogic(device, device.Logic.OnNowClausesGroup);
				LinkLogic(device, device.Logic.OffNowClausesGroup);
				LinkLogic(device, device.Logic.StopClausesGroup);
				if (device.IsInMPT)
				{
					var deviceMPTs = new List<GKMPT>(GKManager.MPTs.FindAll(x => x.MPTDevices.FindAll(y => y.MPTDeviceType == GKMPTDeviceType.AutomaticOffBoard
						|| y.MPTDeviceType == GKMPTDeviceType.Bomb || y.MPTDeviceType == GKMPTDeviceType.DoNotEnterBoard || y.MPTDeviceType == GKMPTDeviceType.ExitBoard
						|| y.MPTDeviceType == GKMPTDeviceType.Speaker).Any(z => z.Device == device)));
					foreach (var deviceMPT in deviceMPTs)
					{
						device.LinkToDescriptor(deviceMPT);
					}
				}

				if (!device.Driver.IsAm)
				{
					foreach (var deviceGuardZone in device.GuardZones)
					{
						device.LinkToDescriptor(deviceGuardZone);
					}
				}

				foreach (var deviceDoor in GKManager.Doors.Where(x => x.LockDevice == device))
				{
					device.LinkToDescriptor(deviceDoor);
				}

				foreach (var devicePumpStation in GKManager.PumpStations.Where(x => x.NSDevices.Contains(device)))
				{
					device.LinkToDescriptor(devicePumpStation);
					LinkLogic(device, device.NSLogic.OnClausesGroup);
				}
			}

			var zone = this as GKZone;
			if (zone != null)
			{
				foreach (var zoneDevice in zone.Devices)
				{
					zone.LinkToDescriptor(zoneDevice);
				}
				zone.LinkToDescriptor(zone);
			}

			var direction = this as GKDirection;
			if (direction != null)
			{
				LinkLogic(direction, direction.Logic.OnClausesGroup);
				if (!direction.Logic.UseOffCounterLogic)
					LinkLogic(direction, direction.Logic.OffClausesGroup);
				LinkLogic(direction, direction.Logic.StopClausesGroup);
			}

			var pumpStation = this as GKPumpStation;
			if (pumpStation != null)
			{
				LinkLogic(pumpStation, pumpStation.StartLogic.OnClausesGroup);
				LinkLogic(pumpStation, pumpStation.StopLogic.OnClausesGroup);
				LinkLogic(pumpStation, pumpStation.AutomaticOffLogic.OnClausesGroup);
			}

			var mpt = this as GKMPT;
			if (mpt != null)
			{
				LinkLogic(mpt, mpt.MptLogic.OnClausesGroup);
				if (!mpt.MptLogic.UseOffCounterLogic)
					LinkLogic(mpt, mpt.MptLogic.OffClausesGroup);
				LinkLogic(mpt, mpt.MptLogic.StopClausesGroup);
				foreach (var mptDevice in mpt.MPTDevices.FindAll(x => x.Device != null && (x.MPTDeviceType == GKMPTDeviceType.HandStart || x.MPTDeviceType == GKMPTDeviceType.HandStop
					|| x.MPTDeviceType == GKMPTDeviceType.HandAutomaticOn || x.MPTDeviceType == GKMPTDeviceType.HandAutomaticOff || x.MPTDeviceType == GKMPTDeviceType.Bomb)))
				{
					mpt.LinkToDescriptor(mptDevice.Device);
				}
			}

			var delay = this as GKDelay;
			if (delay != null)
			{
				LinkLogic(delay, delay.Logic.OnClausesGroup);
				if (!delay.Logic.UseOffCounterLogic)
					LinkLogic(delay, delay.Logic.OffClausesGroup);
				LinkLogic(delay, delay.Logic.StopClausesGroup);
			}

			var guardZone = this as GKGuardZone;
			if (guardZone != null)
			{
				foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
				{
					guardZone.LinkToDescriptor(guardZoneDevice.Device);
					if (guardZoneDevice.Device.DriverType == GKDriverType.RSR2_GuardDetector || guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CodeReader || guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CardReader)
					{
						guardZoneDevice.Device.LinkToDescriptor(guardZone);
					}
				}
			}

			var door = this as GKDoor;
			if (door != null)
			{
				if (door.EnterDevice != null)
					door.LinkToDescriptor(door.EnterDevice);
				if (door.ExitDevice != null)
					door.LinkToDescriptor(door.ExitDevice);
				if (door.EnterButton != null)
					door.LinkToDescriptor(door.EnterButton);
				if (door.ExitButton != null)
					door.LinkToDescriptor(door.ExitButton);
				if (door.LockDevice != null)
					door.LockDevice.LinkToDescriptor(door);
				if (door.LockDeviceExit != null)
					door.LockDeviceExit.LinkToDescriptor(door);
				if (door.LockControlDevice != null)
					door.LinkToDescriptor(door.LockControlDevice);
				if (door.LockControlDeviceExit != null)
					door.LinkToDescriptor(door.LockControlDeviceExit);
				LinkLogic(door, door.OpenRegimeLogic.OnClausesGroup);
				LinkLogic(door, door.NormRegimeLogic.OnClausesGroup);
				LinkLogic(door, door.CloseRegimeLogic.OnClausesGroup);
				door.LinkToDescriptor(door);
			}
		}

		void LinkLogic(GKBase gkBase, GKClauseGroup clauseGroup)
		{
			if (clauseGroup.Clauses != null)
			{
				foreach (var clause in clauseGroup.Clauses)
				{
					foreach (var clauseDevice in clause.Devices)
						gkBase.LinkToDescriptor(clauseDevice);
					foreach (var zone in clause.Zones)
						gkBase.LinkToDescriptor(zone);
					foreach (var guardZone in clause.GuardZones)
						gkBase.LinkToDescriptor(guardZone);
					foreach (var direction in clause.Directions)
						gkBase.LinkToDescriptor(direction);
					foreach (var mpt in clause.MPTs)
						gkBase.LinkToDescriptor(mpt);
					foreach (var delay in clause.Delays)
						gkBase.LinkToDescriptor(delay);
					foreach (var door in clause.Doors)
						gkBase.LinkToDescriptor(door);
					foreach (var pumpStation in clause.PumpStations)
						gkBase.LinkToDescriptor(pumpStation);
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

		public void LinkToDescriptor(GKBase dependsOn)
		{
			AddInputOutputObject(InputDescriptors, dependsOn);
			AddInputOutputObject(dependsOn.OutputDescriptors, this);
		}

		void AddInputOutputObject(List<GKBase> objects, GKBase newObject)
		{
			if (objects == null)
				objects = new List<GKBase>();
			if (newObject != null)
				if (objects.All(x => x.UID != newObject.UID))
					objects.Add(newObject);
		}


		public void ClearDescriptor()
		{
			InputDependentElements = new List<GKBase>();
			OutputDependentElements = new List<GKBase>();
			InputDescriptors = new List<GKBase>();
			OutputDescriptors = new List<GKBase>();
			KauParents = new HashSet<GKDevice>();
			GkParents = new HashSet<GKDevice>();
		}

		[XmlIgnore]
		public List<GKBase> ChildDescriptors = new List<GKBase>();

		[XmlIgnore]
		public bool IsChildDescriptorsReady { get; set; }

		public void CalculateAllChildDescriptors()
		{
			var newChildDescriptors = new List<GKBase>();
			foreach (var descriptorDependentObject in ChildDescriptors)
			{
				descriptorDependentObject.SetAllDescriptorsChildren(newChildDescriptors);
			}
			foreach (var newChildDescriptor in newChildDescriptors)
			{
				if (!ChildDescriptors.Contains(newChildDescriptor))
				{
					ChildDescriptors.Add(newChildDescriptor);
				}
			}
			IsChildDescriptorsReady = true;
		}

		void SetAllDescriptorsChildren(List<GKBase> allChildren)
		{
			foreach (var childDescriptor in ChildDescriptors)
			{
				if (!allChildren.Contains(childDescriptor))
				{
					allChildren.Add(childDescriptor);
					if (!IsChildDescriptorsReady)
					{
						childDescriptor.SetAllDescriptorsChildren(allChildren);
					}
				}
			}
		}

		#endregion Descriptors
	}
}