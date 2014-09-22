using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.GK;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class Variable
	{
		public Variable()
		{
			Uid = Guid.NewGuid();
			DefaultExplicitValue = new ExplicitValue();
			ExplicitValue = new ExplicitValue();
			DefaultExplicitValues = new List<ExplicitValue>();
			ExplicitValues = new List<ExplicitValue>();
		}

		public Variable(string name) : base()
		{
			Name = name;
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public bool IsList { get; set; }

		[DataMember]
		public ExplicitType ExplicitType { get; set; }

		[DataMember]
		public ObjectType ObjectType { get; set; }

		[DataMember]
		public EnumType EnumType { get; set; }
		
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public bool IsGlobal { get; set; }

		[DataMember]
		public ExplicitValue DefaultExplicitValue { get; set; }

		[DataMember]
		public ExplicitValue ExplicitValue { get; set; }

		[DataMember]
		public List<ExplicitValue> ExplicitValues { get; set; }

		[DataMember]
		public List<ExplicitValue> DefaultExplicitValues { get; set; }

		[DataMember]
		public VariableScope VariableScope { get; set; }

		[DataMember]
		public Guid VariableUid { get; set; }

		[DataMember]
		public Guid ArgumentUid { get; set; }

		public void ResetValue()
		{
			PropertyCopy.Copy<ExplicitValue, ExplicitValue>(DefaultExplicitValue, ExplicitValue);
			ExplicitValues = new List<ExplicitValue>(DefaultExplicitValues);
		}

		public void CopyValue(Variable variable)
		{
			PropertyCopy.Copy<ExplicitValue, ExplicitValue>(variable.ExplicitValue, ExplicitValue);
			ExplicitValues = new List<ExplicitValue>(variable.ExplicitValues);
		}
	}
}