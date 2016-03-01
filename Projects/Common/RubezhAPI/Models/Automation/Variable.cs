using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class Variable : Bindable
	{
		public Variable()
		{
			Uid = Guid.NewGuid();
			ExplicitValue = new ExplicitValue();
			ExplicitValues = new List<ExplicitValue>();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public bool IsList { get; set; }

		[DataMember]
		public ContextType ContextType { get; set; }

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
		public ExplicitValue ExplicitValue { get; set; }

		[DataMember]
		public List<ExplicitValue> ExplicitValues { get; set; }

		[DataMember]
		public bool IsReference { get; set; }

		public object Value
		{
			get
			{
				if (IsList)
				{
					switch (ExplicitType)
					{
						case ExplicitType.Boolean: return ExplicitValues.Select(x => (object)x.BoolValue).ToArray();
						case ExplicitType.DateTime: return ExplicitValues.Select(x => (object)x.DateTimeValue).ToArray();
						case ExplicitType.Integer: return ExplicitValues.Select(x => (object)x.IntValue).ToArray();
						case ExplicitType.Float: return ExplicitValues.Select(x => (object)x.FloatValue).ToArray();
						case ExplicitType.String: return ExplicitValues.Select(x => (object)x.StringValue).ToArray();
						case ExplicitType.Object: return ExplicitValues.Select(x => (object)x.UidValue).ToArray();
						case ExplicitType.Enum:
							switch (EnumType)
							{
								case EnumType.DriverType: return ExplicitValues.Select(x => (object)x.DriverTypeValue).ToArray();
								case EnumType.StateType: return ExplicitValues.Select(x => (object)x.StateTypeValue).ToArray();
								case EnumType.PermissionType: return ExplicitValues.Select(x => (object)x.PermissionTypeValue).ToArray();
								case EnumType.JournalEventNameType: return ExplicitValues.Select(x => (object)x.JournalEventNameTypeValue).ToArray();
								case EnumType.JournalEventDescriptionType: return ExplicitValues.Select(x => (object)x.JournalEventDescriptionTypeValue).ToArray();
								case EnumType.JournalObjectType: return ExplicitValues.Select(x => (object)x.JournalObjectTypeValue).ToArray();
								case EnumType.ColorType: return ExplicitValues.Select(x => (object)x.ColorValue).ToArray();
							}
							break;
					}
					return new object[0];
				}
				else
				{
					switch (ExplicitType)
					{
						case ExplicitType.Boolean: return ExplicitValue.BoolValue;
						case ExplicitType.DateTime: return ExplicitValue.DateTimeValue;
						case ExplicitType.Integer: return ExplicitValue.IntValue;
						case ExplicitType.Float: return ExplicitValue.FloatValue;
						case ExplicitType.String: return ExplicitValue.StringValue;
						case ExplicitType.Object: return ExplicitValue.UidValue;
						case ExplicitType.Enum:
							switch (EnumType)
							{
								case EnumType.ColorType: return ExplicitValue.ColorValue;
								case EnumType.DriverType: return ExplicitValue.DriverTypeValue;
								case EnumType.JournalEventDescriptionType: return ExplicitValue.JournalEventDescriptionTypeValue;
								case EnumType.JournalEventNameType: return ExplicitValue.JournalEventNameTypeValue;
								case EnumType.JournalObjectType: return ExplicitValue.JournalObjectTypeValue;
								case EnumType.PermissionType: return ExplicitValue.PermissionTypeValue;
								case EnumType.StateType: return ExplicitValue.StateTypeValue;
							}
							break;
					}
					return new object();
				}
			}
		}
	}
}