using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using System.Xml.Serialization;
using System.Linq;
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
		public List<GKMPT> ClauseInputMPTs { get; set; }
		[XmlIgnore]
		public List<GKDelay> ClauseInputDelays { get; set; }
		[XmlIgnore]
		public List<GKDoor> ClauseInputDoors { get; set; }

		public void ClearClauseDependencies()
		{
			InputGKBases = new List<GKBase>();
			OutputGKBases = new List<GKBase>();
			ClauseInputDevices = new List<GKDevice>();
			ClauseInputMPTs = new List<GKMPT>();
			ClauseInputDelays = new List<GKDelay>();
			ClauseInputDoors = new List<GKDoor>();
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

		public void PrepareInputOutputDependences()
		{
			var device = this as GKDevice;
			var pumpStation = this as GKPumpStation;
			var mpt = this as GKMPT;
			var delay = this as GKDelay;
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
					var deviceMPTs = new List<GKMPT>(GKManager.MPTs.FindAll(x => x.MPTDevices.FindAll(y => y.MPTDeviceType == GKMPTDeviceType.Bomb).Any(z => z.Device == device)));
					foreach (var deviceMPT in deviceMPTs)
					{
						if (deviceMPT.SuspendLogic.OnClausesGroup.GetObjects().Count > 0)
						{
							LinkLogic(device, deviceMPT.SuspendLogic.OnClausesGroup);
						}
					}
				}
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
				LinkLogic(mpt, mpt.StopLogic.OnClausesGroup);
				LinkLogic(mpt, mpt.SuspendLogic.OnClausesGroup);
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
					foreach (var mpt in clause.MPTs)
						gkBase.LinkGKBases(mpt);
					foreach (var delay in clause.Delays)
						gkBase.LinkGKBases(delay);
					foreach (var door in clause.Doors)
						gkBase.LinkGKBases(door);
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
			if(objects == null)
				objects = new List<GKBase>();
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

		public GKDevice GetDataBaseParent()
		{
			PrepareInputOutputDependences();
			var allDependentObjects = GetFullTree(this);
			var allDependentDoors = allDependentObjects.Where(x => x is GKDoor).Cast<GKDoor>().ToList();
			if (allDependentDoors.Count > 0)
				return allDependentDoors.FirstOrDefault().GkDatabaseParent;
			var allDependentDevices = allDependentObjects.Where(x => x is GKDevice).Cast<GKDevice>().ToList();
			var kauParents = allDependentDevices.Select(x => x.KAUParent).ToList();
			kauParents = kauParents.Distinct().ToList();
			if (kauParents.Count == 1 && kauParents.FirstOrDefault() != null)
				return kauParents.FirstOrDefault();
			if (this is GKDevice)
				return (this as GKDevice).GKParent;
			if (allDependentDevices != null && allDependentDevices.Count > 0)
				return allDependentDevices.FirstOrDefault().GKParent;
			return null;
		}

		List<GKBase> GetFullTree(GKBase gkBase)
		{
			return GetAllDependentObjects(gkBase, new List<GKBase>()).ToList();
		}

		List<GKBase> GetAllDependentObjects(GKBase gkBase, List<GKBase> result)
		{
			var inputObjects = new List<GKBase>(gkBase.InputGKBases);
			inputObjects.RemoveAll(x => x.UID == gkBase.UID);
			foreach (var inputObject in new List<GKBase>(inputObjects))
			{
				inputObject.PrepareInputOutputDependences();
				if (result.All(x => x.UID != inputObject.UID))
					result.Add(inputObject);
				else
					continue;
				result.AddRange(GetAllDependentObjects(inputObject, result).FindAll(x => !result.Contains(x)));
			}
			return result;
		}
	}
}