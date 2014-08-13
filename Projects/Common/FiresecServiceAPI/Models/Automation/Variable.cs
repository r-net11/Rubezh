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
			Copy (variable);
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
		public List<bool> DefaultBoolValues { get; set; }
		public List<bool> BoolValues { get; set; }

		[DataMember]
		public bool DefaultBoolValue { get; set; }
		public bool BoolValue { get; set; }

		[DataMember]
		public List<DateTime> DefaultDateTimeValues { get; set; }
		public List<DateTime> DateTimeValues { get; set; }

		[DataMember]
		public DateTime DefaultDateTimeValue { get; set; }
		public DateTime DateTimeValue { get; set; }

		[DataMember]
		public List<int> DefaultIntValues { get; set; }
		public List<int> IntValues { get; set; }

		[DataMember]
		public int DefaultIntValue { get; set; }
		public int IntValue { get; set; }

		[DataMember]
		public List<string> DefaultStringValues { get; set; }
		public List<string> StringValues { get; set; }

		[DataMember]
		public string DefaultStringValue { get; set; }
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

		public string CurrentValue
		{
			get
			{
				if (!IsList && VariableType == VariableType.Boolean)
					return BoolValue.ToString();
				if (!IsList && VariableType == VariableType.DateTime)
					return DateTimeValue.ToString();
				if (!IsList && VariableType == VariableType.Integer)
					return IntValue.ToString();
				if (!IsList && VariableType == VariableType.String)
					return StringValue;

				if (IsList && VariableType == VariableType.Boolean)
					return string.Join("\n", BoolValues.ToArray());
				if (IsList && VariableType == VariableType.DateTime)
					return string.Join("\n", DateTimeValues.ToArray());
				if (IsList && VariableType == VariableType.Integer)
					return string.Join("\n", IntValues.ToArray());
				if (IsList && VariableType == VariableType.String)
					return string.Join("\n", StringValues.ToArray());

				if (VariableType == VariableType.Object)
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
	}
}