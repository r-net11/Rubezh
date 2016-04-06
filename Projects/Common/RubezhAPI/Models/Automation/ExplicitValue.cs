using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using System;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Xml.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	[XmlInclude(typeof(ObjectReference))]
	public class ExplicitValue
	{
		public ExplicitValue()
		{
			DateTimeValue = DateTime.Now;
			StringValue = "";
		}

		[DataMember]
		public ExplicitType ExplicitType { get; set; }

		[DataMember]
		public EnumType EnumType { get; set; }

		[XmlIgnore]
		public int IntValue { get; set; }
		[XmlIgnore]
		public double FloatValue { get; set; }
		[XmlIgnore]
		public bool BoolValue { get; set; }
		[XmlIgnore]
		public DateTime DateTimeValue { get; set; }
		[XmlIgnore]
		public ObjectReference ObjectReferenceValue { get; set; }
		[XmlIgnore]
		public string StringValue { get; set; }
		[XmlIgnore]
		public XStateClass StateTypeValue { get; set; }
		[XmlIgnore]
		public GKDriverType DriverTypeValue { get; set; }
		[XmlIgnore]
		public PermissionType PermissionTypeValue { get; set; }
		[XmlIgnore]
		public JournalEventNameType JournalEventNameTypeValue { get; set; }
		[XmlIgnore]
		public JournalEventDescriptionType JournalEventDescriptionTypeValue { get; set; }
		[XmlIgnore]
		public JournalObjectType JournalObjectTypeValue { get; set; }
		[XmlIgnore]
		public Color ColorValue { get; set; }

		[DataMember]
		public object Value
		{
			get
			{
				switch (ExplicitType)
				{
					case ExplicitType.Integer: return IntValue;
					case ExplicitType.Float: return FloatValue;
					case ExplicitType.Boolean: return BoolValue;
					case ExplicitType.DateTime: return DateTimeValue;
					case ExplicitType.String: return StringValue;
					case ExplicitType.Object: return ObjectReferenceValue;
					case ExplicitType.Enum:
						switch (EnumType)
						{
							case EnumType.StateType: return StateTypeValue;
							case EnumType.DriverType: return DriverTypeValue;
							case EnumType.PermissionType: return PermissionTypeValue;
							case EnumType.JournalEventNameType: return JournalEventNameTypeValue;
							case EnumType.JournalEventDescriptionType: return JournalEventDescriptionTypeValue;
							case EnumType.JournalObjectType: return JournalObjectTypeValue;
							case EnumType.ColorType: return ColorValue;
						}
						break;
				}

				return null;
			}

			set
			{
				if (value == null)
					return;

				if (value is int)
				{
					IntValue = (int)value;
					return;
				}

				if (value is double)
				{
					FloatValue = (double)value;
					return;
				}

				if (value is bool)
				{
					BoolValue = (bool)value;
					return;
				}

				if (value is DateTime)
				{
					DateTimeValue = (DateTime)value;
					return;
				}

				if (value is ObjectReference)
				{
					ObjectReferenceValue = (ObjectReference)value;
					return;
				}

				if (value is string)
				{
					StringValue = (string)value;
					return;
				}

				if (value is XStateClass)
				{
					StateTypeValue = (XStateClass)value;
					return;
				}

				if (value is GKDriverType)
				{
					DriverTypeValue = (GKDriverType)value;
					return;
				}

				if (value is PermissionType)
				{
					PermissionTypeValue = (PermissionType)value;
					return;
				}

				if (value is JournalEventNameType)
				{
					JournalEventNameTypeValue = (JournalEventNameType)value;
					return;
				}

				if (value is JournalEventDescriptionType)
				{
					JournalEventDescriptionTypeValue = (JournalEventDescriptionType)value;
					return;
				}

				if (value is JournalObjectType)
				{
					JournalObjectTypeValue = (JournalObjectType)value;
					return;
				}

				if (value is Color)
				{
					ColorValue = (Color)value;
					return;
				}

#if DEBUG
				throw new InvalidCastException("Попытка записи значения неизвестного типа в ExplicitValue");
#endif
			}
		}

		public override string ToString()
		{
			switch (ExplicitType)
			{
				case ExplicitType.Integer: return IntValue.ToString();
				case ExplicitType.Float: return FloatValue.ToString();
				case ExplicitType.Boolean: return BoolValue ? "Да" : "Нет";
				case ExplicitType.DateTime: return DateTimeValue.ToString();
				case ExplicitType.String: return StringValue;
				case ExplicitType.Object: return ObjectReferenceValue.ToString();
				case ExplicitType.Enum:
					switch (EnumType)
					{
						case EnumType.StateType: return StateTypeValue.ToDescription();
						case EnumType.DriverType: return DriverTypeValue.ToDescription();
						case EnumType.PermissionType: return PermissionTypeValue.ToDescription();
						case EnumType.JournalEventNameType: return JournalEventNameTypeValue.ToDescription();
						case EnumType.JournalEventDescriptionType: return JournalEventDescriptionTypeValue.ToDescription();
						case EnumType.JournalObjectType: return JournalObjectTypeValue.ToDescription();
						case EnumType.ColorType: return ColorValue.ToString();
					}
					break;
			}

			return null;
		}
	}
}