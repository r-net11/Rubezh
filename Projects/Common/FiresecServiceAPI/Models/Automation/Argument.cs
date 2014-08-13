using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class Argument
	{
		public Argument(Variable variable)
		{
			Uid = Guid.NewGuid();
			ArgumentUid = variable.Uid;
			VariableType = variable.VariableType;
			BoolValue = variable.DefaultBoolValue;
			DateTimeValue = variable.DefaultDateTimeValue;
			IntValue = variable.DefaultIntValue;
			ObjectType = variable.ObjectType;
			StringValue = variable.DefaultStringValue;
			VariableType = variable.VariableType;
			IsList = variable.IsList;
		}

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public bool IsList { get; set; }

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
		public Guid ArgumentUid { get; set; }

		[DataMember]
		public VariableType VariableType { get; set; }
	}
}