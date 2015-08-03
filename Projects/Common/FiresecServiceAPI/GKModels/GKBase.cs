using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Common;
using FiresecClient;

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
			InputGKBases = new List<GKBase>();
			OutputGKBases = new List<GKBase>();
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
		[XmlIgnore]
		public List<GKDoor> ClauseInputDoors { get; set; }
		[XmlIgnore]
		public List<GKPumpStation> ClauseInputPumpStations { get; set; }

		public void ClearClauseDependencies()
		{
			InputObjects = new List<GKBase>();
			OutputObjects = new List<GKBase>();
			InputGKBases = new List<GKBase>();
			OutputGKBases = new List<GKBase>();
			ClauseInputDevices = new List<GKDevice>();
			ClauseInputZones = new List<GKZone>();
			ClauseInputGuardZones = new List<GKGuardZone>();
			ClauseInputDirections = new List<GKDirection>();
			ClauseInputMPTs = new List<GKMPT>();
			ClauseInputDelays = new List<GKDelay>();
			ClauseInputDoors = new List<GKDoor>();
			ClauseInputPumpStations = new List<GKPumpStation>();
		}

		[XmlIgnore]
		protected List<GKBase> InputObjects { get; set; }
		[XmlIgnore]
		protected List<GKBase> OutputObjects { get; set; }
		[XmlIgnore]
		public List<GKBase> InputGKBases { get; set; }
		[XmlIgnore]
		public List<GKBase> OutputGKBases { get; set; }

		[XmlIgnore]
		public GKDevice KauDatabaseParent { get; set; }
		[XmlIgnore]
		public GKDevice GkDatabaseParent { get; set; }

		[XmlIgnore]
		public GKDevice DataBaseParent 
		{
			get { return KauDatabaseParent ?? GkDatabaseParent; }
			set
			{
				if (value != null && value.DriverType == GKDriverType.GK)
					GkDatabaseParent = value;
				if (value != null && value.DriverType == GKDriverType.RSR2_KAU)
					KauDatabaseParent = value;
			}
		}

		[XmlIgnore]
		public virtual bool IsLogicOnKau { get; set; }

		[XmlIgnore]
		public ushort GKDescriptorNo { get; set; }
		[XmlIgnore]
		public ushort KAUDescriptorNo { get; set; }

		public abstract void Update(GKDevice device);

		public abstract void Update(GKDirection direction);

		public void LinkObject(GKBase gkBase)
		{
			if (gkBase == null)
				return;
			if (!InputObjects.Contains(gkBase))
				InputObjects.Add(gkBase);
			if (!gkBase.OutputObjects.Contains(this))
				gkBase.OutputObjects.Add(this);
		}

		public void UnLinkObject(GKBase gkBase)
		{
			if (gkBase == null)
				return;
			if (InputObjects.Contains(gkBase))
				InputObjects.Remove(gkBase);
			if (gkBase.OutputObjects.Contains(this))
				gkBase.OutputObjects.Remove(this);
		}

		public virtual string GetGKDescriptorName(GKNameGenerationType gkNameGenerationType)
		{
			var result = PresentationName;
			if (result.Length > 32)
				result = result.Substring(0, 32);
			return result.TrimEnd(' ');
		}

		public void PrepareInputOutputDependences()
		{
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
						device.LinkGKBases(deviceMPT);
					}
				}

				if (!device.Driver.IsAm)
				{
					foreach (var deviceGuardZone in device.GuardZones)
					{
						device.LinkGKBases(deviceGuardZone);
					}
				}

				var deviceDoors = GKManager.Doors.FindAll(x => x.LockDevice == device);
				foreach (var deviceDoor in deviceDoors)
				{
					device.LinkGKBases(deviceDoor);
				}

				var devicePumpStations = GKManager.PumpStations.FindAll(x => x.NSDevices.Contains(device));
				foreach (var devicePumpStation in devicePumpStations)
				{
					device.LinkGKBases(devicePumpStation);
				}
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
				LinkLogic(direction, direction.Logic.StopClausesGroup);
			}

			if (pumpStation != null)
			{
				LinkLogic(pumpStation, pumpStation.StartLogic.OnClausesGroup);
				LinkLogic(pumpStation, pumpStation.StopLogic.OnClausesGroup);
				LinkLogic(pumpStation, pumpStation.AutomaticOffLogic.OnClausesGroup);
			}

			if (mpt != null)
			{
				LinkLogic(mpt, mpt.MptLogic.OnClausesGroup);
				LinkLogic(mpt, mpt.MptLogic.OffClausesGroup);
				LinkLogic(mpt, mpt.MptLogic.StopClausesGroup);
				foreach (var mptDevice in mpt.MPTDevices.FindAll(x => x.MPTDeviceType == GKMPTDeviceType.HandStart || x.MPTDeviceType == GKMPTDeviceType.HandStop
					|| x.MPTDeviceType == GKMPTDeviceType.HandAutomaticOn || x.MPTDeviceType == GKMPTDeviceType.HandAutomaticOff || x.MPTDeviceType == GKMPTDeviceType.Bomb))
				{
					mpt.LinkGKBases(mptDevice.Device);
				}
			}

			if (delay != null)
			{
				LinkLogic(delay, delay.Logic.OnClausesGroup);
				LinkLogic(delay, delay.Logic.OffClausesGroup);
				LinkLogic(delay, delay.Logic.StopClausesGroup);
			}

			if (guardZone != null)
			{
				foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
				{
					if (guardZoneDevice.ActionType != GKGuardZoneDeviceActionType.ChangeGuard)
					{
						guardZone.LinkGKBases(guardZoneDevice.Device);
						if (guardZoneDevice.Device.DriverType == GKDriverType.RSR2_GuardDetector || guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CodeReader || guardZoneDevice.Device.DriverType == GKDriverType.RSR2_CardReader)
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
				if (door.EnterButton != null)
					door.LinkGKBases(door.EnterButton);
				if (door.ExitButton != null)
					door.LinkGKBases(door.ExitButton);
				if (door.LockDevice != null)
					door.LockDevice.LinkGKBases(door);
				if (door.LockDeviceExit != null)
					door.LockDeviceExit.LinkGKBases(door);
				if (door.LockControlDevice != null)
					door.LinkGKBases(door.LockControlDevice);
				if (door.LockControlDeviceExit != null)
					door.LinkGKBases(door.LockControlDeviceExit);
				LinkLogic(door, door.OpenRegimeLogic.OnClausesGroup);
				LinkLogic(door, door.NormRegimeLogic.OnClausesGroup);
				LinkLogic(door, door.CloseRegimeLogic.OnClausesGroup);
				door.LinkGKBases(door);
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
					foreach (var door in clause.Doors)
						gkBase.LinkGKBases(door);
					foreach (var pumpStation in clause.PumpStations)
						gkBase.LinkGKBases(pumpStation);
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
			AddInputOutputObject(InputGKBases, dependsOn);
			AddInputOutputObject(dependsOn.OutputGKBases, this);
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

		//public GKDevice GetDataBaseParent()
		//{
		//	PrepareInputOutputDependences();
		//	var allDependentObjects = GetFullTree(this);
		//	var allDependentDoors = allDependentObjects.Where(x => x is GKDoor).Cast<GKDoor>().ToList();
		//	if (allDependentDoors.Count > 0)
		//		return allDependentDoors.FirstOrDefault().GkDatabaseParent;
		//	var allDependentDevices = allDependentObjects.Where(x => x is GKDevice).Cast<GKDevice>().ToList();
		//	var allDependentGuardZones = allDependentObjects.Where(x => x is GKGuardZone).Cast<GKGuardZone>().ToList();
		//	allDependentGuardZones.ForEach(x => allDependentDevices.AddRange(GetGuardZoneDependetnDevicesByCodes(x)));
		//	var kauParents = allDependentDevices.Select(x => x.KAUParent).ToList();
		//	kauParents = kauParents.Distinct().ToList();
		//	if (this is GKDevice && allDependentGuardZones.Any(x => x.HasAccessLevel))
		//		return (this as GKDevice).GKParent;
		//	if (kauParents.Count == 1 && kauParents.FirstOrDefault() != null)
		//		return kauParents.FirstOrDefault();
		//	if (this is GKDevice)
		//		return (this as GKDevice).GKParent;
		//	if (allDependentDevices != null && allDependentDevices.Count > 0)
		//		return allDependentDevices.FirstOrDefault().GKParent;
		//	return null;
		//}

		public void GetDataBaseParent()
		{
			PrepareInputOutputDependences();
			var dataBaseParent = GetDataBaseParent(this, new List<GKBase>(), new List<GKDevice>());
			if (dataBaseParent == null)
				return;
			IsLogicOnKau = dataBaseParent.Driver.IsKau;
			if (dataBaseParent.Driver.IsKau)
			{
				KauDatabaseParent = dataBaseParent;
				GkDatabaseParent = dataBaseParent.GKParent;
			}
			else
			{
				IsLogicOnKau = false;
				GkDatabaseParent = dataBaseParent;
			}
			if (this is GKGuardZone && (this as GKGuardZone).HasAccessLevel)
			{
				IsLogicOnKau = false;
				KauDatabaseParent = null;
			}
		}

		public GKDevice GetDataBaseParent(GKBase gkBase, List<GKBase> result, List<GKDevice> dataBaseParents)
		{
			var gkParent = new GKDevice();
			var kauParents = new List<GKDevice>();
			if (gkBase.DataBaseParent == null)
			{
				gkBase.PrepareInputOutputDependences();
			}
			else
			{
				dataBaseParents.Add(gkBase.DataBaseParent);
				gkParent = dataBaseParents.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
				if (gkParent != null)
					return null;
				kauParents = dataBaseParents.FindAll(x => x.DriverType == GKDriverType.RSR2_KAU).Distinct().ToList();
				if (kauParents.Count > 1)
					return null;
			}
			var inputObjects = new List<GKBase>(gkBase.InputGKBases);
			inputObjects.RemoveAll(x => x.UID == gkBase.UID);
			inputObjects.RemoveAll(result.Contains);
			foreach (var inputObject in new List<GKBase>(inputObjects))
			{
				if (result.Any(x => x.UID == inputObject.UID))
					continue;
				if (inputObject is GKGuardZone)
					result.AddRange(GKManager.GuardZones.FindAll(x => x.GetCodeUids().Intersect((inputObject as GKGuardZone).GetCodeUids()).Any() && !result.Contains(x)));
				if (!result.Contains(inputObject))
					result.Add(inputObject);
				if(!result.Contains(null))
					GetDataBaseParent(inputObject, result, dataBaseParents);
			}
			result.RemoveAll(x => x == null);
			dataBaseParents.AddRange(result.Distinct().ToList().FindAll(x => x is GKDevice).Select(y => (y as GKDevice).DataBaseParent).ToList());
			dataBaseParents.RemoveAll(x => x == null);
			gkParent = dataBaseParents.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
			if (gkParent != null)
				return gkParent;
			kauParents = dataBaseParents.FindAll(x => x.DriverType == GKDriverType.RSR2_KAU).Distinct().ToList();
			if (kauParents.Count > 1)
				return kauParents[0].GKParent;
			return kauParents.Count ==1 ? kauParents[0] : null;
		}
	}
}