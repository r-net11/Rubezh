using Common;
using StrazhAPI.Plans.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StrazhAPI.Models.Automation;

namespace StrazhAPI.Automation
{
	[DataContract]
	[KnownType(typeof(GlobalVariable))]
	[KnownType(typeof(LocalVariable))]
	public class Procedure : IPlanPresentable
	{
		public Procedure()
		{
			Name = Resources.Language.Models.Automation.Procedure.Name;
			Variables = new List<IVariable>();
			Arguments = new List<IVariable>();
			Steps = new List<ProcedureStep>();
			Uid = Guid.NewGuid();
			FiltersUids = new List<Guid>();
			IsActive = true;
			PlanElementUIDs = new List<Guid>();
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
		public List<IVariable> Variables { get; set; }

		[DataMember]
		public List<IVariable> Arguments { get; set; }

		[DataMember]
		public List<Guid> FiltersUids { get; set; }

		[DataMember]
		public bool IsActive { get; set; }

		[DataMember]
		public bool StartWithServer { get; set; }

		[DataMember]
		public int TimeOut { get; set; }

		[DataMember]
		public TimeType TimeType { get; set; }

		#region IIdentity Members

		Guid IIdentity.UID
		{
			get { return Uid; }
		}

		#endregion IIdentity Members

		#region IPlanPresentable Members

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		#endregion IPlanPresentable Members

		#region IChangedNotification Members

		public void OnChanged()
		{
			if (Changed != null)
				Changed();
		}

		public event Action Changed;

		#endregion IChangedNotification Members
	}
}