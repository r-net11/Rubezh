using System;
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
			ObjectType = ObjectType.Card;
			StringValue = "";
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
			VariableType = variable.VariableType;
			IsList = variable.IsList;
		}

		[DataMember]
		public bool IsList { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public bool BoolValue { get; set; }

		[DataMember]
		public DateTime DateTimeValue { get; set; }

		[DataMember]
		public int IntValue { get; set; }
		
		[DataMember]
		public ObjectType ObjectType { get; set; }

		[DataMember]
		public string StringValue { get; set; }

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public VariableType VariableType { get; set; }
	}
}