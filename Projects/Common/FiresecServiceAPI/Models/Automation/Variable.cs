using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class Variable
	{
		public Variable()
		{
			Uid = Guid.NewGuid();
			DateTimeValue = DateTime.Now;
			IntValue = 0;
			StringValue = "";
			UidValue = Guid.Empty;
			DefaultUidValue = Guid.Empty;
			DefaultIntValue = 0;
			DefaultStringValue = "";
			VariableItems = new List<VariableItem>();
		}

		public Variable(string name) : base()
		{
			Name = name;
		}
		
		[DataMember]
		public bool IsList { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public bool DefaultBoolValue { get; set; }

		[DataMember]
		public bool BoolValue { get; set; }

		[DataMember]
		public DateTime DefaultDateTimeValue { get; set; }

		[DataMember]
		public DateTime DateTimeValue { get; set; }

		[DataMember]
		public int DefaultIntValue { get; set; }

		[DataMember]
		public int IntValue { get; set; }

		[DataMember]
		public string DefaultStringValue { get; set; }

		[DataMember]
		public string StringValue { get; set; }

		[DataMember]
		public Guid DefaultUidValue { get; set; }

		[DataMember]
		public Guid UidValue { get; set; }

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public ValueType ValueType { get; set; }

		[DataMember]
		public ObjectType ObjectType { get; set; }

		[DataMember]
		public bool IsGlobal { get; set; }

		[DataMember]
		public List<VariableItem> VariableItems { get; set; }

		//public string CurrentValue
		//{
		//    get
		//    {
		//        if (!IsList && ValueType == ValueType.Boolean)
		//            return BoolValue.ToString();
		//        if (!IsList && ValueType == ValueType.DateTime)
		//            return DateTimeValue.ToString();
		//        if (!IsList && ValueType == ValueType.Integer)
		//            return IntValue.ToString();
		//        if (!IsList && ValueType == ValueType.String)
		//            return StringValue;
		//        if (ValueType == ValueType.Object)
		//            return ObjectType.ToString();

		//        return "Неизветсное значение";
		//    }
		//}

		public void ResetValue()
		{
			BoolValue = DefaultBoolValue;
			DateTimeValue = DefaultDateTimeValue;
			IntValue = DefaultIntValue;
			StringValue = DefaultStringValue;
			UidValue = DefaultUidValue;
		}

		public void ResetValue(Argument argument)
		{
			BoolValue = argument.BoolValue;
			DateTimeValue = argument.DateTimeValue;
			IntValue = argument.IntValue;
			StringValue = argument.StringValue;
			UidValue = argument.UidValue;
		}
	}

	public class VariableItem : ICloneable
	{
		public VariableItem()
		{

		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		[DataMember]
		public int IntValue { get; set; }

		[DataMember]
		public bool BoolValue { get; set; }

		[DataMember]
		public DateTime DateTimeValue { get; set; }

		[DataMember]
		public Guid ObjectUid { get; set; }

		[DataMember]
		public string StringValue { get; set; }

		[DataMember]
		public ValueType ValueType { get; set; }
	}
}