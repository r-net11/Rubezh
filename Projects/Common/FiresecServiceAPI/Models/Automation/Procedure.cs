﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class Procedure : IPlanPresentable
	{
		public Procedure()
		{
			Name = "Новая процедура";
			Variables = new List<Variable>();
			Arguments = new List<Variable>();
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
		public List<Variable> Variables { get; set; }

		[DataMember]
		public List<Variable> Arguments { get; set; }

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

		[DataMember]
		public bool IsSync { get; set; }

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