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
			ClearClauseDependencies();
			State = new GKState();
			InputGKBases = new List<GKBase>();
			OutputGKBases = new List<GKBase>();
		}

		[XmlIgnore]
		public List<GKDevice> ClauseInputDevices { get; set; }

		public void ClearClauseDependencies()
		{
			InputGKBases = new List<GKBase>();
			OutputGKBases = new List<GKBase>();
			ClauseInputDevices = new List<GKDevice>();
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

			if (device != null)
			{
				LinkLogic(device, device.Logic.OnClausesGroup);
				LinkLogic(device, device.Logic.OffClausesGroup);
				LinkLogic(device, device.Logic.OnNowClausesGroup);
				LinkLogic(device, device.Logic.OffNowClausesGroup);
				LinkLogic(device, device.Logic.StopClausesGroup);
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

		[XmlIgnore]
		public abstract GKBaseObjectType ObjectType { get; }

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