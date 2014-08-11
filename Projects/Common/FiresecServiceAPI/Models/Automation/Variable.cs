using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class Variable
	{
		public Variable(string name)
		{
			Name = name;
			Uid = Guid.NewGuid();
			DateTimeValue = DateTime.Now;
			IntValue = 0;
			StringValue = "";
			ObjectsUids = new List<Guid>();
			BoolValues = new List<bool>();
			DateTimeValues = new List<DateTime>();
			IntValues = new List<int>();
			StringValues = new List<string>();
		}

		public Variable(Variable variable)
		{
			Copy (variable);
			Uid = Guid.NewGuid();
		}

		public void Copy(Variable variable)
		{
			Name = variable.Name;
			Description = variable.Description;
			BoolValue = variable.BoolValue;
			DateTimeValue = variable.DateTimeValue;
			IntValue = variable.IntValue;
			ObjectType = variable.ObjectType;
			StringValue = variable.StringValue;
			BoolValues = variable.BoolValues;
			DateTimeValues = variable.DateTimeValues;
			IntValues = variable.IntValues;
			StringValues = variable.StringValues;
			VariableType = variable.VariableType;
			IsList = variable.IsList;
			ObjectsUids = variable.ObjectsUids;
			ObjectUid = variable.ObjectUid;
		}

		[DataMember]
		public bool IsList { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<bool> BoolValues { get; set; }

		[DataMember]
		public bool BoolValue { get; set; }

		[DataMember]
		public List<DateTime> DateTimeValues { get; set; }
		
		[DataMember]
		public DateTime DateTimeValue { get; set; }

		[DataMember]
		public List<int> IntValues { get; set; }
		
		[DataMember]
		public int IntValue { get; set; }

		[DataMember]
		public List<string> StringValues { get; set; }

		[DataMember]
		public string StringValue { get; set; }

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public VariableType VariableType { get; set; }

		[DataMember]
		public ObjectType ObjectType { get; set; }

		[DataMember]
		public List<Guid> ObjectsUids { get; set; }

		[DataMember]
		public Guid ObjectUid { get; set; }
	}
}