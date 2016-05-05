using RubezhAPI.Automation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RubezhAPI.Models;
using RubezhClient;
using RubezhAPI.Journal;

namespace AutomationModule.Validation
{
	public partial class Validator
	{
		public static ResearchDependencyResult GetDependency(Variable variable)
		{
			ResearchDependencyResult result;

			result.Variables = null;
			result.Procedures = ClientManager.SystemConfiguration.AutomationConfiguration.Procedures
				.Where(x => x.Steps.Any(y => y.Arguments.Any(a => a.VariableUid == variable.Uid))).ToArray();
			result.Plans = ClientManager.PlansConfiguration.AllPlans
				.Where(x => x.AllElements.Any(z => result.Procedures.Any(m => m.PlanElementUIDs.Contains(z.UID)))).ToArray();
			result.OpcDaTagFilters = ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTagFilters
				.Where(x => result.Procedures.Any(z => z.OpcDaTagFiltersUids.Any(m => m == x.UID))).ToArray();
			result.OpcDaTags = (from tag in ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTsServers.SelectMany(x => x.Tags)
							   join filter in result.OpcDaTagFilters on tag.Uid equals filter.TagUID select tag).ToArray();
			result.OpcDaServers = ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTsServers
				.Where(x => x.Tags.Any(y => result.OpcDaTags.Contains(y))).Distinct().ToArray();
			result.Filters = ClientManager.SystemConfiguration.JournalFilters
				.Where(x => result.Procedures.Any(z => z.FiltersUids.Any(m => m == x.UID))).ToArray();
			return result;
		}
	}

	public struct ResearchDependencyResult
	{
		public Plan[] Plans;
		public Procedure[] Procedures;
		public Variable[] Variables;
		public OpcDaTag[] OpcDaTags;
		public OpcDaServer[] OpcDaServers;
		public OpcDaTagFilter[] OpcDaTagFilters;
		public JournalFilter[] Filters;

		public bool HasDependency
		{
			get
			{
				if ((Plans != null) && (Plans.Length > 0))
					return true;
				if ((Procedures != null) && (Procedures.Length > 0))
					return true;
				if ((Variables != null) && (Variables.Length > 0))
					return true;
				if ((OpcDaTags != null) && (OpcDaTags.Length > 0))
					return true;
				if ((OpcDaServers != null) && (OpcDaServers.Length > 0))
					return true;
				if ((OpcDaTagFilters != null) && (OpcDaTagFilters.Length > 0))
					return true;
				if ((Filters != null) && (Filters.Length > 0))
					return true;

				return false;
			}
		}
	}
}