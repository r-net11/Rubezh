using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using Infrustructure.Plans.Interfaces;
using System.Linq;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class Procedure : IIdentity, IPlanPresentable
	{
		public Procedure()
		{
			Name = "Новая процедура";
			Variables = new List<Variable>();
			Arguments = new List<Variable>();
			Steps = new List<ProcedureStep>();
			Uid = Guid.NewGuid();
			FiltersUids = new List<Guid>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<ProcedureStep> Steps { get; set; }

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public List<Variable> Variables { get; set; }

		[DataMember]
		public List<Variable> Arguments { get; set; }

		[DataMember]
		public List<Guid> FiltersUids { get; set; }

		[DataMember]
		public bool IsActive { get; set; }

		public void ResetVaraibles(List<Argument> arguments)
		{
			foreach (var variable in Variables)
				variable.ResetValue();
			foreach (var variable in Arguments)
			{
				var argument = arguments.FirstOrDefault(x => x.ArgumentUid == variable.Uid);
				if (argument == null)
					variable.ResetValue();
				else
					variable.ResetValue(argument);
			}
		}

		#region IIdentity Members

		Guid IIdentity.UID
		{
			get { return Uid; }
		}

		#endregion

		#region IPlanPresentable Members

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		#endregion

		#region IChangedNotification Members

		public void OnChanged()
		{
			if (Changed != null)
				Changed();
		}
		public event Action Changed;

		#endregion
	}
}