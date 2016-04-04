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

		ExplicitType _explicitType;
		[DataMember]
		public ExplicitType ExplicitType
		{
			get { return _explicitType; }
			set
			{
				_explicitType = value;
				ExplicitValue.ExplicitType = value;
			}
		}

		ObjectType _objectType;
		[DataMember]
		public ObjectType ObjectType
		{
			get { return _objectType; }
			set
			{
				_objectType = value;
				ExplicitValue.ObjectReferenceValue = new ObjectReference { ObjectType = value, UID = ExplicitValue.ObjectReferenceValue.UID };
			}
		}

		EnumType _enumType;
		[DataMember]
		public EnumType EnumType
		{
			get { return _enumType; }
			set
			{
				_enumType = value;
				ExplicitValue.EnumType = value;
			}
		}

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
						case ExplicitType.Boolean: return ExplicitValues.Select(x => x.BoolValue).ToArray();
						case ExplicitType.DateTime: return ExplicitValues.Select(x => x.DateTimeValue).ToArray();
						case ExplicitType.Integer: return ExplicitValues.Select(x => x.IntValue).ToArray();
						case ExplicitType.Float: return ExplicitValues.Select(x => x.FloatValue).ToArray();
						case ExplicitType.String: return ExplicitValues.Select(x => x.StringValue).ToArray();
						case ExplicitType.Object: return ExplicitValues.Select(x => x.ObjectReferenceValue).ToArray();
						case ExplicitType.Enum:
							switch (EnumType)
							{
								case EnumType.DriverType: return ExplicitValues.Select(x => x.DriverTypeValue).ToArray();
								case EnumType.StateType: return ExplicitValues.Select(x => x.StateTypeValue).ToArray();
								case EnumType.PermissionType: return ExplicitValues.Select(x => x.PermissionTypeValue).ToArray();
								case EnumType.JournalEventNameType: return ExplicitValues.Select(x => x.JournalEventNameTypeValue).ToArray();
								case EnumType.JournalEventDescriptionType: return ExplicitValues.Select(x => x.JournalEventDescriptionTypeValue).ToArray();
								case EnumType.JournalObjectType: return ExplicitValues.Select(x => x.JournalObjectTypeValue).ToArray();
								case EnumType.ColorType: return ExplicitValues.Select(x => x.ColorValue).ToArray();
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
						case ExplicitType.Object: return ExplicitValue.ObjectReferenceValue;
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