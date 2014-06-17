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
			BoolValue = false;
			DateTimeValue = DateTime.Now;
			IntValue = 0;
			ObjectValue = ObjectType.Card;
			StringValue = "";
		}

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
		public ObjectType ObjectValue { get; set; }

		[DataMember]
		public string StringValue { get; set; }

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public VariableType VariableType { get; set; }
	}
}