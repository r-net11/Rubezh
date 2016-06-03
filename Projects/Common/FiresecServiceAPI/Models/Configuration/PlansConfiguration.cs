using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	[KnownType(typeof(Plan))]
	[KnownType(typeof(PlanFolder))]
	public class PlansConfiguration : VersionedConfiguration
	{
		public PlansConfiguration()
		{
			Plans = new List<Plan>();
		}

		[DataMember]
		public List<Plan> Plans { get; set; }

		[XmlIgnore]
		public List<Plan> AllPlans { get; set; }

		private Dictionary<Guid, Plan> _planMap;

		[XmlIgnore]
		public Plan this[Guid uid]
		{
			get
			{
				return _planMap.ContainsKey(uid) ? _planMap[uid] : null;
			}
		}

		public void Update()
		{
			AllPlans = new List<Plan>();
			_planMap = new Dictionary<Guid, Plan>();
			AddChildren(Plans, null);
		}

		private void AddChildren(IEnumerable<Plan> plans, Plan parent)
		{
			foreach (var plan in plans)
			{
				plan.Parent = parent;
				var realPlan = plan;
				AllPlans.Add(realPlan);
				_planMap.Add(realPlan.UID, realPlan);

				if (plan.Children == null)
					plan.Children = new List<Plan>();

				AddChildren(plan.Children, plan);
			}
		}

		public override bool ValidateVersion()
		{
			var result = true;
			Update();
			result &= ValidateVersion(AllPlans);
			return result;
		}

		private bool ValidateVersion(IEnumerable<Plan> folders)
		{
			var result = true;
			foreach (var folder in folders)
			{
				var plan = folder;
				if (plan != null)
					result &= ValidateVersion(plan);
			}
			return result;
		}

		private bool ValidateVersion(Plan plan) //TODO: implement this method or remove
		{
			return true;
		}
	}
}