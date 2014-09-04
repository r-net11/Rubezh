using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class Argument
	{
		public Argument()
		{

		}

		public Argument(Variable variable)
		{
			VariableUid = variable.Uid;
			ValueType = variable.ValueType;
			BoolValue = variable.DefaultBoolValue;
			DateTimeValue = variable.DefaultDateTimeValue;
			UidValue = variable.DefaultUidValue;
			IntValue = variable.DefaultIntValue;
			ObjectType = variable.ObjectType;
			StringValue = variable.DefaultStringValue;
			ValueType = variable.ValueType;
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
		public Guid UidValue { get; set; }

		[DataMember]
		public int IntValue { get; set; }
		
		[DataMember]
		public ObjectType ObjectType { get; set; }

		[DataMember]
		public string StringValue { get; set; }

		[DataMember]
		public Guid VariableUid { get; set; }

		[DataMember]
		public ValueType ValueType { get; set; }

		[DataMember]
		public VariableType VariableType { get; set; }
	}
}