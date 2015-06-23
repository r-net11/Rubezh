using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecClient;

namespace FiresecAPI.GK
{
	public partial class GKDeviceConfiguration
	{
		public void UpdateConfiguration()
		{
			ValidateVersion();

			Update();

			InitializeProperties();
			Invalidate();
		}

		void InitializeProperties()
		{
			foreach (var device in Devices)
			{
				if (device.Properties == null)
					device.Properties = new List<GKProperty>();
			}
		}

		void Invalidate()
		{
			ClearAllReferences();
			InitializeLogic();
		}

		void ClearAllReferences()
		{
			foreach (var device in Devices)
			{
				device.ClearClauseDependencies();
			}
		}

		void InitializeLogic()
		{
			foreach (var device in Devices)
			{
				InvalidateOneLogic(device, device.Logic);
			}
		}

		public void InvalidateOneLogic(GKDevice device, GKLogic logic)
		{
			InvalidateInputObjectsBaseLogic(device, logic);
		}
		public void InvalidateInputObjectsBaseLogic(GKBase gkBase, GKLogic logic)
		{
			logic.OnClausesGroup = InvalidateOneInputObjectsBaseLogic(gkBase, logic.OnClausesGroup);
			logic.OffClausesGroup = InvalidateOneInputObjectsBaseLogic(gkBase, logic.OffClausesGroup);
			logic.StopClausesGroup = InvalidateOneInputObjectsBaseLogic(gkBase, logic.StopClausesGroup);
			logic.OnNowClausesGroup = InvalidateOneInputObjectsBaseLogic(gkBase, logic.OnNowClausesGroup);
			logic.OffNowClausesGroup = InvalidateOneInputObjectsBaseLogic(gkBase, logic.OffNowClausesGroup);
		}

		public GKClauseGroup InvalidateOneInputObjectsBaseLogic(GKBase gkBase, GKClauseGroup clauseGroup)
		{
			var result = new GKClauseGroup();
			result.ClauseJounOperationType = clauseGroup.ClauseJounOperationType;
			var groups = new List<GKClauseGroup>();
			foreach (var group in clauseGroup.ClauseGroups)
			{
				var _clauseGroup = InvalidateOneInputObjectsBaseLogic(gkBase, group);
				if (_clauseGroup.Clauses.Count + _clauseGroup.ClauseGroups.Count > 0)
					groups.Add(_clauseGroup);
			}
			result.ClauseGroups = groups;

			foreach (var clause in clauseGroup.Clauses)
			{
				clause.Devices = new List<GKDevice>();

				var deviceUIDs = new List<Guid>();
				foreach (var deviceUID in clause.DeviceUIDs)
				{
					var clauseDevice = Devices.FirstOrDefault(x => x.UID == deviceUID);
					if (clauseDevice != null && !clauseDevice.IsNotUsed)
					{
						deviceUIDs.Add(deviceUID);
						clause.Devices.Add(clauseDevice);
						if (!gkBase.ClauseInputDevices.Contains(clauseDevice))
							gkBase.ClauseInputDevices.Add(clauseDevice);
					}
				}
				clause.DeviceUIDs = deviceUIDs;

				if (clause.HasObjects())
					result.Clauses.Add(clause);
			}
			return result;
		}
	}
}