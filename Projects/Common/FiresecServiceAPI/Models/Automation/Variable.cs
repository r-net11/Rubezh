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

		}

		public Variable(string name)
		{
			Name = name;
			Uid = Guid.NewGuid();
			DateTimeValue = DateTime.Now;
			IntValue = 0;
			StringValue = "";
			DefaultIntValue = 0;
			DefaultStringValue = "";
			ObjectsUids = new List<Guid>();
			BoolValues = new List<bool>();
			DateTimeValues = new List<DateTime>();
			IntValues = new List<int>();
			StringValues = new List<string>();
			DefaultBoolValues = new List<bool>();
			DefaultDateTimeValues = new List<DateTime>();
			DefaultIntValues = new List<int>();
			DefaultStringValues = new List<string>();
		}

		public Variable(Variable variable)
		{
			Copy(variable);
			Uid = Guid.NewGuid();
		}

		public void Copy(Variable variable)
		{
			Name = variable.Name;
			Description = variable.Description;
			DefaultBoolValue = variable.DefaultBoolValue;
			DefaultDateTimeValue = variable.DefaultDateTimeValue;
			DefaultIntValue = variable.DefaultIntValue;
			ObjectType = variable.ObjectType;
			DefaultStringValue = variable.DefaultStringValue;
			DefaultBoolValues = variable.DefaultBoolValues;
			DefaultDateTimeValues = variable.DefaultDateTimeValues;
			DefaultIntValues = variable.DefaultIntValues;
			DefaultStringValues = variable.DefaultStringValues;
			ValueType = variable.ValueType;
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
		public List<bool> DefaultBoolValues { get; set; }

		[DataMember]
		public List<bool> BoolValues { get; set; }

		[DataMember]
		public bool DefaultBoolValue { get; set; }

		[DataMember]
		public bool BoolValue { get; set; }

		[DataMember]
		public List<DateTime> DefaultDateTimeValues { get; set; }

		[DataMember]
		public List<DateTime> DateTimeValues { get; set; }

		[DataMember]
		public DateTime DefaultDateTimeValue { get; set; }

		[DataMember]
		public DateTime DateTimeValue { get; set; }

		[DataMember]
		public List<int> DefaultIntValues { get; set; }

		[DataMember]
		public List<int> IntValues { get; set; }

		[DataMember]
		public int DefaultIntValue { get; set; }

		[DataMember]
		public int IntValue { get; set; }

		[DataMember]
		public List<string> DefaultStringValues { get; set; }

		[DataMember]
		public List<string> StringValues { get; set; }

		[DataMember]
		public string DefaultStringValue { get; set; }

		[DataMember]
		public string StringValue { get; set; }

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public ValueType ValueType { get; set; }

		[DataMember]
		public ObjectType ObjectType { get; set; }

		[DataMember]
		public List<Guid> ObjectsUids { get; set; }

		[DataMember]
		public Guid ObjectUid { get; set; }

		[DataMember]
		public bool IsGlobal { get; set; }

		public string CurrentValue
		{
			get
			{
				if (!IsList && ValueType == ValueType.Boolean)
					return BoolValue.ToString();
				if (!IsList && ValueType == ValueType.DateTime)
					return DateTimeValue.ToString();
				if (!IsList && ValueType == ValueType.Integer)
					return IntValue.ToString();
				if (!IsList && ValueType == ValueType.String)
					return StringValue;

				if (IsList && ValueType == ValueType.Boolean)
					return string.Join("\n", BoolValues.ToArray());
				if (IsList && ValueType == ValueType.DateTime)
					return string.Join("\n", DateTimeValues.ToArray());
				if (IsList && ValueType == ValueType.Integer)
					return string.Join("\n", IntValues.ToArray());
				if (IsList && ValueType == ValueType.String)
					return string.Join("\n", StringValues.ToArray());

				if (ValueType == ValueType.Object)
					return ObjectType.ToString();

				return "Неизветсное значение";
			}
		}

		public void ResetValue()
		{
			BoolValue = DefaultBoolValue;
			DateTimeValue = DefaultDateTimeValue;
			IntValue = DefaultIntValue;
			StringValue = DefaultStringValue;
			BoolValues = new List<bool>(DefaultBoolValues);
			DateTimeValues = new List<DateTime>(DefaultDateTimeValues);
			IntValues = new List<int>(DefaultIntValues);
			StringValues = new List<string>(DefaultStringValues);
		}

		public void ResetValue(Argument argument)
		{
			BoolValue = argument.BoolValue;
			DateTimeValue = argument.DateTimeValue;
			IntValue = argument.IntValue;
			StringValue = argument.StringValue;
			BoolValues = new List<bool>(DefaultBoolValues);
			DateTimeValues = new List<DateTime>(DefaultDateTimeValues);
			IntValues = new List<int>(DefaultIntValues);
			StringValues = new List<string>(DefaultStringValues);
		}
	}
}