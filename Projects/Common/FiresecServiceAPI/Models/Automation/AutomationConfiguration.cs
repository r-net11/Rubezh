using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class AutomationConfiguration
	{
		public AutomationConfiguration()
		{
			Procedures = new List<Procedure>();
			AutomationSchedules = new List<AutomationSchedule>();
			AutomationSounds = new List<AutomationSound>();
			GlobalVariables = new List<Variable>();
		}

		[DataMember]
		public List<Procedure> Procedures { get; set; }

		[DataMember]
		public List<AutomationSchedule> AutomationSchedules { get; set; }

		[DataMember]
		public List<AutomationSound> AutomationSounds { get; set; }

		[DataMember]
		public List<Variable> GlobalVariables { get; set; }

		public void UpdateConfiguration()
		{
			foreach (var procedure in Procedures)
			{
				foreach (var step in procedure.Steps)
				{
					UpdateStep(step, procedure);
				}
			}
		}

		public static List<Property> ObjectTypeToProperiesList(ObjectType objectType)
		{
			if (objectType == ObjectType.Device)
				return new List<Property> {Property.Name, Property.ShleifNo, Property.IntAddress, Property.DeviceState};
			if (objectType == ObjectType.Zone)
				return new List<Property> { Property.Name, Property.No, Property.ZoneType };
			if (objectType == ObjectType.Direction)
				return new List<Property> { Property.Name, Property.No, Property.Delay, Property.Hold, Property.DelayRegime };
			return new List<Property>();
		}

		public void UpdateStep(ProcedureStep step, Procedure procedure)
		{

		}
	}
}