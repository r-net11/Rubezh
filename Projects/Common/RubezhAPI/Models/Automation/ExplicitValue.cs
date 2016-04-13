using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Xml.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	[XmlInclude(typeof(ObjectReference))]
	[XmlInclude(typeof(List<int>))]
	[XmlInclude(typeof(List<double>))]
	[XmlInclude(typeof(List<bool>))]
	[XmlInclude(typeof(List<DateTime>))]
	[XmlInclude(typeof(List<string>))]
	[XmlInclude(typeof(List<XStateClass>))]
	[XmlInclude(typeof(List<GKDriverType>))]
	[XmlInclude(typeof(List<PermissionType>))]
	[XmlInclude(typeof(List<JournalEventNameType>))]
	[XmlInclude(typeof(List<JournalEventDescriptionType>))]
	[XmlInclude(typeof(List<JournalObjectType>))]
	[XmlInclude(typeof(List<Color>))]
	[XmlInclude(typeof(List<Guid>))]
	[KnownType(typeof(ObjectReference))]
	[KnownType(typeof(List<int>))]
	[KnownType(typeof(List<double>))]
	[KnownType(typeof(List<bool>))]
	[KnownType(typeof(List<DateTime>))]
	[KnownType(typeof(List<string>))]
	[KnownType(typeof(List<XStateClass>))]
	[KnownType(typeof(List<GKDriverType>))]
	[KnownType(typeof(List<PermissionType>))]
	[KnownType(typeof(List<JournalEventNameType>))]
	[KnownType(typeof(List<JournalEventDescriptionType>))]
	[KnownType(typeof(List<JournalObjectType>))]
	[KnownType(typeof(List<Color>))]
	[KnownType(typeof(List<Guid>))]
	[KnownType(typeof(XStateClass))]
	[KnownType(typeof(GKDriverType))]
	[KnownType(typeof(PermissionType))]
	[KnownType(typeof(JournalEventNameType))]
	[KnownType(typeof(JournalEventDescriptionType))]
	[KnownType(typeof(JournalObjectType))]
	[KnownType(typeof(Color))]
	[KnownType(typeof(Guid))]
	public abstract class ExplicitValue : Bindable
	{
		public ExplicitValue()
		{
			DateTimeValue = DateTime.Now;
			StringValue = "";

			IntValueList = new List<int>();
			FloatValueList = new List<double>();
			BoolValueList = new List<bool>();
			DateTimeValueList = new List<DateTime>();
			UidValueList = new List<Guid>();
			StringValueList = new List<string>();
			StateTypeValueList = new List<XStateClass>();
			DriverTypeValueList = new List<GKDriverType>();
			PermissionTypeValueList = new List<PermissionType>();
			JournalEventNameTypeValueList = new List<JournalEventNameType>();
			JournalEventDescriptionTypeValueList = new List<JournalEventDescriptionType>();
			JournalObjectTypeValueList = new List<JournalObjectType>();
			ColorValueList = new List<Color>();
		}

		[DataMember]
		public bool IsList { get; set; }

		[DataMember]
		public ExplicitType ExplicitType { get; set; }

		[DataMember]
		public ObjectType ObjectType { get; set; }

		[DataMember]
		public EnumType EnumType { get; set; }

		#region Single values
		[XmlIgnore]
		public int IntValue { get; set; }
		[XmlIgnore]
		public double FloatValue { get; set; }
		[XmlIgnore]
		public bool BoolValue { get; set; }
		[XmlIgnore]
		public DateTime DateTimeValue { get; set; }
		[XmlIgnore]
		public Guid UidValue { get; set; }
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
		#endregion

		#region List values
		[XmlIgnore]
		public List<int> IntValueList { get; set; }
		[XmlIgnore]
		public List<double> FloatValueList { get; set; }
		[XmlIgnore]
		public List<bool> BoolValueList { get; set; }
		[XmlIgnore]
		public List<DateTime> DateTimeValueList { get; set; }
		[XmlIgnore]
		public List<Guid> UidValueList { get; set; }
		[XmlIgnore]
		public List<string> StringValueList { get; set; }
		[XmlIgnore]
		public List<XStateClass> StateTypeValueList { get; set; }
		[XmlIgnore]
		public List<GKDriverType> DriverTypeValueList { get; set; }
		[XmlIgnore]
		public List<PermissionType> PermissionTypeValueList { get; set; }
		[XmlIgnore]
		public List<JournalEventNameType> JournalEventNameTypeValueList { get; set; }
		[XmlIgnore]
		public List<JournalEventDescriptionType> JournalEventDescriptionTypeValueList { get; set; }
		[XmlIgnore]
		public List<JournalObjectType> JournalObjectTypeValueList { get; set; }
		[XmlIgnore]
		public List<Color> ColorValueList { get; set; }
		#endregion


		[DataMember]
		public object Value
		{
			get
			{
				if (IsList)
				{
					switch (ExplicitType)
					{
						case ExplicitType.Integer: return IntValueList;
						case ExplicitType.Float: return FloatValueList;
						case ExplicitType.Boolean: return BoolValueList;
						case ExplicitType.DateTime: return DateTimeValueList;
						case ExplicitType.String: return StringValueList;
						case ExplicitType.Object: return UidValueList;
						case ExplicitType.Enum:
							switch (EnumType)
							{
								case EnumType.StateType: return StateTypeValueList;
								case EnumType.DriverType: return DriverTypeValueList;
								case EnumType.PermissionType: return PermissionTypeValueList;
								case EnumType.JournalEventNameType: return JournalEventNameTypeValueList;
								case EnumType.JournalEventDescriptionType: return JournalEventDescriptionTypeValueList;
								case EnumType.JournalObjectType: return JournalObjectTypeValueList;
								case EnumType.ColorType: return ColorValueList;
							}
							break;
					}
				}
				else
				{
					switch (ExplicitType)
					{
						case ExplicitType.Integer: return IntValue;
						case ExplicitType.Float: return FloatValue;
						case ExplicitType.Boolean: return BoolValue;
						case ExplicitType.DateTime: return DateTimeValue;
						case ExplicitType.String: return StringValue;
						case ExplicitType.Object: return new ObjectReference { ObjectType = this.ObjectType, UID = UidValue };
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
				}

				return null;
			}

			set
			{
				if (value == null) return;

				if (value is IList)
				{
					if (value is List<int>)
					{
						IntValueList = (List<int>)value;
						IsList = true;
						ExplicitType = ExplicitType.Integer;
						return;
					}
					if (value is List<double>)
					{
						FloatValueList = (List<double>)value;
						IsList = true;
						ExplicitType = ExplicitType.Float;
						return;
					}
					if (value is List<bool>)
					{
						BoolValueList = (List<bool>)value;
						IsList = true;
						ExplicitType = ExplicitType.Boolean;
						return;
					}
					if (value is List<DateTime>)
					{
						DateTimeValueList = (List<DateTime>)value;
						IsList = true;
						ExplicitType = ExplicitType.DateTime;
						return;
					}
					if (value is List<string>)
					{
						StringValueList = (List<string>)value;
						IsList = true;
						ExplicitType = ExplicitType.String;
						return;
					}

					if (value is List<XStateClass>)
					{
						StateTypeValueList = (List<XStateClass>)value;
						IsList = true;
						ExplicitType = ExplicitType.Enum;
						EnumType = EnumType.StateType;
						return;
					}
					if (value is List<GKDriverType>)
					{
						DriverTypeValueList = (List<GKDriverType>)value;
						IsList = true;
						ExplicitType = ExplicitType.Enum;
						EnumType = EnumType.DriverType;
						return;
					}
					if (value is List<PermissionType>)
					{
						PermissionTypeValueList = (List<PermissionType>)value;
						IsList = true;
						ExplicitType = ExplicitType.Enum;
						EnumType = EnumType.PermissionType;
						return;
					}
					if (value is List<JournalEventNameType>)
					{
						JournalEventNameTypeValueList = (List<JournalEventNameType>)value;
						IsList = true;
						ExplicitType = ExplicitType.Enum;
						EnumType = EnumType.JournalEventNameType;
						return;
					}
					if (value is List<JournalEventDescriptionType>)
					{
						JournalEventDescriptionTypeValueList = (List<JournalEventDescriptionType>)value;
						IsList = true;
						ExplicitType = ExplicitType.Enum;
						EnumType = EnumType.JournalEventDescriptionType;
						return;
					}
					if (value is List<JournalObjectType>)
					{
						JournalObjectTypeValueList = (List<JournalObjectType>)value;
						IsList = true;
						ExplicitType = ExplicitType.Enum;
						EnumType = EnumType.JournalObjectType;
						return;
					}
					if (value is List<Color>)
					{
						ColorValueList = (List<Color>)value;
						IsList = true;
						ExplicitType = ExplicitType.Enum;
						EnumType = EnumType.ColorType;
						return;
					}

					if (value is List<ObjectReference>)
					{
						var objRefList = (List<ObjectReference>)value;
						UidValueList = objRefList.Select(x => x.UID).ToList();
						IsList = true;
						ExplicitType = ExplicitType.Object;
						if (objRefList.Count > 0)
							ObjectType = objRefList[0].ObjectType;
						return;
					}

					if (value is List<Guid>)
					{
						UidValueList = (List<Guid>)value;
						IsList = true;
						ExplicitType = ExplicitType.Object;
						return;
					}

					if (value is List<GKDevice>)
					{
						UidValueList = ((List<GKDevice>)value).Select(x => x.UID).ToList();
						IsList = true;
						ExplicitType = ExplicitType.Object;
						ObjectType = ObjectType.Device;
						return;
					}
					if (value is List<GKZone>)
					{
						UidValueList = ((List<GKZone>)value).Select(x => x.UID).ToList();
						IsList = true;
						ExplicitType = ExplicitType.Object;
						ObjectType = ObjectType.Zone;
						return;
					}
					if (value is List<GKDirection>)
					{
						UidValueList = ((List<GKDirection>)value).Select(x => x.UID).ToList();
						IsList = true;
						ExplicitType = ExplicitType.Object;
						ObjectType = ObjectType.Direction;
						return;
					}
					if (value is List<GKDelay>)
					{
						UidValueList = ((List<GKDelay>)value).Select(x => x.UID).ToList();
						IsList = true;
						ExplicitType = ExplicitType.Object;
						ObjectType = ObjectType.Delay;
						return;
					}
					if (value is List<GKGuardZone>)
					{
						UidValueList = ((List<GKGuardZone>)value).Select(x => x.UID).ToList();
						IsList = true;
						ExplicitType = ExplicitType.Object;
						ObjectType = ObjectType.GuardZone;
						return;
					}
					if (value is List<GKPumpStation>)
					{
						UidValueList = ((List<GKPumpStation>)value).Select(x => x.UID).ToList();
						IsList = true;
						ExplicitType = ExplicitType.Object;
						ObjectType = ObjectType.PumpStation;
						return;
					}
					if (value is List<GKMPT>)
					{
						UidValueList = ((List<GKMPT>)value).Select(x => x.UID).ToList();
						IsList = true;
						ExplicitType = ExplicitType.Object;
						ObjectType = ObjectType.MPT;
						return;
					}
					if (value is List<Camera>)
					{
						UidValueList = ((List<Camera>)value).Select(x => x.UID).ToList();
						IsList = true;
						ExplicitType = ExplicitType.Object;
						ObjectType = ObjectType.VideoDevice;
						return;
					}
					if (value is List<GKDoor>)
					{
						UidValueList = ((List<GKDoor>)value).Select(x => x.UID).ToList();
						IsList = true;
						ExplicitType = ExplicitType.Object;
						ObjectType = ObjectType.GKDoor;
						return;
					}
					if (value is List<Organisation>)
					{
						UidValueList = ((List<Organisation>)value).Select(x => x.UID).ToList();
						IsList = true;
						ExplicitType = ExplicitType.Object;
						ObjectType = ObjectType.Organisation;
						return;
					}

				}
				else
				{
					if (value is int)
					{
						IntValue = (int)value;
						IsList = false;
						ExplicitType = ExplicitType.Integer;
						return;
					}
					if (value is double)
					{
						FloatValue = (double)value;
						IsList = false;
						ExplicitType = ExplicitType.Float;
						return;
					}
					if (value is bool)
					{
						BoolValue = (bool)value;
						IsList = false;
						ExplicitType = ExplicitType.Boolean;
						return;
					}
					if (value is DateTime)
					{
						DateTimeValue = (DateTime)value;
						IsList = false;
						ExplicitType = ExplicitType.DateTime;
						return;
					}
					if (value is string)
					{
						StringValue = (string)value;
						IsList = false;
						ExplicitType = ExplicitType.String;
						return;
					}

					if (value is XStateClass)
					{
						StateTypeValue = (XStateClass)value;
						IsList = false;
						ExplicitType = ExplicitType.Enum;
						EnumType = EnumType.StateType;
						return;
					}
					if (value is GKDriverType)
					{
						DriverTypeValue = (GKDriverType)value;
						IsList = false;
						ExplicitType = ExplicitType.Enum;
						EnumType = EnumType.DriverType;
						return;
					}
					if (value is PermissionType)
					{
						PermissionTypeValue = (PermissionType)value;
						IsList = false;
						ExplicitType = ExplicitType.Enum;
						EnumType = EnumType.PermissionType;
						return;
					}
					if (value is JournalEventNameType)
					{
						JournalEventNameTypeValue = (JournalEventNameType)value;
						IsList = false;
						ExplicitType = ExplicitType.Enum;
						EnumType = EnumType.JournalEventNameType;
						return;
					}
					if (value is JournalEventDescriptionType)
					{
						JournalEventDescriptionTypeValue = (JournalEventDescriptionType)value;
						IsList = false;
						ExplicitType = ExplicitType.Enum;
						EnumType = EnumType.JournalEventDescriptionType;
						return;
					}
					if (value is JournalObjectType)
					{
						JournalObjectTypeValue = (JournalObjectType)value;
						IsList = false;
						ExplicitType = ExplicitType.Enum;
						EnumType = EnumType.JournalObjectType;
						return;
					}
					if (value is Color)
					{
						ColorValue = (Color)value;
						IsList = false;
						ExplicitType = ExplicitType.Enum;
						EnumType = EnumType.ColorType;
						return;
					}

					if (value is ObjectReference)
					{
						var objRef = (ObjectReference)value;
						UidValue = objRef.UID;
						IsList = false;
						ExplicitType = ExplicitType.Object;
						ObjectType = objRef.ObjectType;
						return;
					}

					if (value is GKDevice)
					{
						UidValue = ((GKDevice)value).UID;
						IsList = false;
						ExplicitType = ExplicitType.Object;
						ObjectType = ObjectType.Device;
						return;
					}
					if (value is GKZone)
					{
						UidValue = ((GKZone)value).UID;
						IsList = false;
						ExplicitType = ExplicitType.Object;
						ObjectType = ObjectType.Zone;
						return;
					}
					if (value is GKDirection)
					{
						UidValue = ((GKDirection)value).UID;
						IsList = false;
						ExplicitType = ExplicitType.Object;
						ObjectType = ObjectType.Direction;
						return;
					}
					if (value is GKDelay)
					{
						UidValue = ((GKDelay)value).UID;
						IsList = false;
						ExplicitType = ExplicitType.Object;
						ObjectType = ObjectType.Delay;
						return;
					}
					if (value is GKGuardZone)
					{
						UidValue = ((GKGuardZone)value).UID;
						IsList = false;
						ExplicitType = ExplicitType.Object;
						ObjectType = ObjectType.GuardZone;
						return;
					}
					if (value is GKPumpStation)
					{
						UidValue = ((GKPumpStation)value).UID;
						IsList = false;
						ExplicitType = ExplicitType.Object;
						ObjectType = ObjectType.PumpStation;
						return;
					}
					if (value is GKMPT)
					{
						UidValue = ((GKMPT)value).UID;
						IsList = false;
						ExplicitType = ExplicitType.Object;
						ObjectType = ObjectType.MPT;
						return;
					}
					if (value is Camera)
					{
						UidValue = ((Camera)value).UID;
						IsList = false;
						ExplicitType = ExplicitType.Object;
						ObjectType = ObjectType.VideoDevice;
						return;
					}
					if (value is GKDoor)
					{
						UidValue = ((GKDoor)value).UID;
						IsList = false;
						ExplicitType = ExplicitType.Object;
						ObjectType = ObjectType.GKDoor;
						return;
					}
					if (value is Organisation)
					{
						UidValue = ((Organisation)value).UID;
						IsList = false;
						ExplicitType = ExplicitType.Object;
						ObjectType = ObjectType.Organisation;
						return;
					}
				}

#if DEBUG
				throw new InvalidCastException("Попытка записи значения неизвестного типа в ExplicitValue");
#endif
			}
		}

		public object ResolvedValue
		{
			get
			{
				if (ExplicitType != ExplicitType.Object)
					return Value;
				return IsList ?
					UidValueList.Select(x => GetObjectValue(x)).ToList() :
					GetObjectValue(UidValue);
			}
		}
		public override string ToString()
		{
			if (IsList)
			{
				switch (ExplicitType)
				{
					case ExplicitType.Integer: return string.Join("; ", IntValueList);
					case ExplicitType.Float: return string.Join("; ", FloatValueList);
					case ExplicitType.Boolean: return string.Join("; ", BoolValueList.Select(x => GetBoolString(x)));
					case ExplicitType.DateTime: return string.Join("; ", DateTimeValueList);
					case ExplicitType.String: return string.Join("; ", StringValueList);
					case ExplicitType.Object: return string.Join("; ", UidValueList.Select(x => GetObjectString(x)));
					case ExplicitType.Enum:
						switch (EnumType)
						{
							case EnumType.StateType: return string.Join("; ", StateTypeValueList.Select(x => x.ToDescription()));
							case EnumType.DriverType: return string.Join("; ", DriverTypeValueList.Select(x => x.ToDescription()));
							case EnumType.PermissionType: return string.Join("; ", PermissionTypeValueList.Select(x => x.ToDescription()));
							case EnumType.JournalEventNameType: return string.Join("; ", JournalEventNameTypeValueList.Select(x => x.ToDescription()));
							case EnumType.JournalEventDescriptionType: return string.Join("; ", JournalEventDescriptionTypeValueList.Select(x => x.ToDescription()));
							case EnumType.JournalObjectType: return string.Join("; ", JournalObjectTypeValueList.Select(x => x.ToDescription()));
							case EnumType.ColorType: return string.Join("; ", ColorValueList.Select(x => x.ToString()));
						}
						break;
				}
			}
			else
			{
				switch (ExplicitType)
				{
					case ExplicitType.Integer: return IntValue.ToString();
					case ExplicitType.Float: return FloatValue.ToString();
					case ExplicitType.Boolean: return GetBoolString(BoolValue);
					case ExplicitType.DateTime: return DateTimeValue.ToString();
					case ExplicitType.String: return StringValue;
					case ExplicitType.Object: return GetObjectString(UidValue);
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
			}
			return null;
		}

		public void AddToList(object value)
		{
			if (value == null) return;

			if (value is int)
			{
				IntValueList.Add((int)value);
				IsList = true;
				ExplicitType = ExplicitType.Integer;
				return;
			}
			if (value is double)
			{
				FloatValueList.Add((double)value);
				IsList = true;
				ExplicitType = ExplicitType.Float;
				return;
			}
			if (value is bool)
			{
				BoolValueList.Add((bool)value);
				IsList = true;
				ExplicitType = ExplicitType.Boolean;
				return;
			}
			if (value is DateTime)
			{
				DateTimeValueList.Add((DateTime)value);
				IsList = true;
				ExplicitType = ExplicitType.DateTime;
				return;
			}
			if (value is string)
			{
				StringValueList.Add((string)value);
				IsList = true;
				ExplicitType = ExplicitType.String;
				return;
			}

			if (value is XStateClass)
			{
				StateTypeValueList.Add((XStateClass)value);
				IsList = true;
				ExplicitType = ExplicitType.Enum;
				EnumType = EnumType.StateType;
				return;
			}
			if (value is GKDriverType)
			{
				DriverTypeValueList.Add((GKDriverType)value);
				IsList = true;
				ExplicitType = ExplicitType.Enum;
				EnumType = EnumType.DriverType;
				return;
			}
			if (value is PermissionType)
			{
				PermissionTypeValueList.Add((PermissionType)value);
				IsList = true;
				ExplicitType = ExplicitType.Enum;
				EnumType = EnumType.PermissionType;
				return;
			}
			if (value is JournalEventNameType)
			{
				JournalEventNameTypeValueList.Add((JournalEventNameType)value);
				IsList = true;
				ExplicitType = ExplicitType.Enum;
				EnumType = EnumType.JournalEventNameType;
				return;
			}
			if (value is JournalEventDescriptionType)
			{
				JournalEventDescriptionTypeValueList.Add((JournalEventDescriptionType)value);
				IsList = true;
				ExplicitType = ExplicitType.Enum;
				EnumType = EnumType.JournalEventDescriptionType;
				return;
			}
			if (value is JournalObjectType)
			{
				JournalObjectTypeValueList.Add((JournalObjectType)value);
				IsList = true;
				ExplicitType = ExplicitType.Enum;
				EnumType = EnumType.JournalObjectType;
				return;
			}
			if (value is Color)
			{
				ColorValueList.Add((Color)value);
				IsList = true;
				ExplicitType = ExplicitType.Enum;
				EnumType = EnumType.ColorType;
				return;
			}

			if (value is ObjectReference)
			{
				var objRef = (ObjectReference)value;
				UidValueList.Add(objRef.UID);
				IsList = true;
				ExplicitType = ExplicitType.Object;
				ObjectType = objRef.ObjectType;
				return;
			}

			if (value is GKDevice)
			{
				UidValueList.Add(((GKDevice)value).UID);
				IsList = true;
				ExplicitType = ExplicitType.Object;
				ObjectType = ObjectType.Device;
				return;
			}
			if (value is GKZone)
			{
				UidValueList.Add(((GKZone)value).UID);
				IsList = true;
				ExplicitType = ExplicitType.Object;
				ObjectType = ObjectType.Zone;
				return;
			}
			if (value is GKDirection)
			{
				UidValueList.Add(((GKDirection)value).UID);
				IsList = true;
				ExplicitType = ExplicitType.Object;
				ObjectType = ObjectType.Direction;
				return;
			}
			if (value is GKDelay)
			{
				UidValueList.Add(((GKDelay)value).UID);
				IsList = true;
				ExplicitType = ExplicitType.Object;
				ObjectType = ObjectType.Delay;
				return;
			}
			if (value is GKGuardZone)
			{
				UidValueList.Add(((GKGuardZone)value).UID);
				IsList = true;
				ExplicitType = ExplicitType.Object;
				ObjectType = ObjectType.GuardZone;
				return;
			}
			if (value is GKPumpStation)
			{
				UidValueList.Add(((GKPumpStation)value).UID);
				IsList = true;
				ExplicitType = ExplicitType.Object;
				ObjectType = ObjectType.PumpStation;
				return;
			}
			if (value is GKMPT)
			{
				UidValueList.Add(((GKMPT)value).UID);
				IsList = true;
				ExplicitType = ExplicitType.Object;
				ObjectType = ObjectType.MPT;
				return;
			}
			if (value is Camera)
			{
				UidValueList.Add(((Camera)value).UID);
				IsList = true;
				ExplicitType = ExplicitType.Object;
				ObjectType = ObjectType.VideoDevice;
				return;
			}
			if (value is GKDoor)
			{
				UidValueList.Add(((GKDoor)value).UID);
				IsList = true;
				ExplicitType = ExplicitType.Object;
				ObjectType = ObjectType.GKDoor;
				return;
			}
			if (value is Organisation)
			{
				UidValueList.Add(((Organisation)value).UID);
				IsList = true;
				ExplicitType = ExplicitType.Object;
				ObjectType = ObjectType.Organisation;
				return;
			}


#if DEBUG
			throw new InvalidCastException("Попытка добавления в список ExplicitValue неизвестного типа");
#endif
		}

		public void RemoveFromList(object value)
		{
			if (value == null) return;

			if (value is int)
			{
				IntValueList.Remove((int)value);
				return;
			}
			if (value is double)
			{
				FloatValueList.Remove((double)value);
				return;
			}
			if (value is bool)
			{
				BoolValueList.Remove((bool)value);
				return;
			}
			if (value is DateTime)
			{
				DateTimeValueList.Remove((DateTime)value);
				return;
			}
			if (value is string)
			{
				StringValueList.Remove((string)value);
				return;
			}

			if (value is XStateClass)
			{
				StateTypeValueList.Remove((XStateClass)value);
				return;
			}
			if (value is GKDriverType)
			{
				DriverTypeValueList.Remove((GKDriverType)value);
				return;
			}
			if (value is PermissionType)
			{
				PermissionTypeValueList.Remove((PermissionType)value);
				return;
			}
			if (value is JournalEventNameType)
			{
				JournalEventNameTypeValueList.Remove((JournalEventNameType)value);
				return;
			}
			if (value is JournalEventDescriptionType)
			{
				JournalEventDescriptionTypeValueList.Remove((JournalEventDescriptionType)value);
				return;
			}
			if (value is JournalObjectType)
			{
				JournalObjectTypeValueList.Remove((JournalObjectType)value);
				return;
			}
			if (value is Color)
			{
				ColorValueList.Remove((Color)value);
				return;
			}

			if (value is ObjectReference)
			{
				var objRef = (ObjectReference)value;
				UidValueList.Remove(objRef.UID);
				return;
			}

			if (value is GKDevice)
			{
				UidValueList.Remove(((GKDevice)value).UID);
				return;
			}
			if (value is GKZone)
			{
				UidValueList.Remove(((GKZone)value).UID);
				return;
			}
			if (value is GKDirection)
			{
				UidValueList.Remove(((GKDirection)value).UID);
				return;
			}
			if (value is GKDelay)
			{
				UidValueList.Remove(((GKDelay)value).UID);
				return;
			}
			if (value is GKGuardZone)
			{
				UidValueList.Remove(((GKGuardZone)value).UID);
				return;
			}
			if (value is GKPumpStation)
			{
				UidValueList.Remove(((GKPumpStation)value).UID);
				return;
			}
			if (value is GKMPT)
			{
				UidValueList.Remove(((GKMPT)value).UID);
				return;
			}
			if (value is Camera)
			{
				UidValueList.Remove(((Camera)value).UID);
				return;
			}
			if (value is GKDoor)
			{
				UidValueList.Remove(((GKDoor)value).UID);
				return;
			}
			if (value is Organisation)
			{
				UidValueList.Remove(((Organisation)value).UID);
				return;
			}

#if DEBUG
			throw new InvalidCastException("Попытка добавления в список ExplicitValue неизвестного типа");
#endif
		}


		public object GetObjectValue(Guid uid)
		{
			return GetObjectValue(new ObjectReference { UID = uid, ObjectType = this.ObjectType });
		}

		public static object GetObjectValue(ObjectReference objRef)
		{
			return ResolveObjectValue == null ?
				objRef :
				ResolveObjectValue(objRef.UID, objRef.ObjectType);
		}

		string GetObjectString(Guid uid)
		{
			return GetObjectString(new ObjectReference { UID = uid, ObjectType = this.ObjectType });
		}

		public static string GetObjectString(ObjectReference objRef)
		{
			return ResolveObjectName == null ?
				string.Format("{0} ({1})", objRef.UID.ToString(), objRef.ObjectType.ToDescription()) :
				ResolveObjectName(objRef.UID, objRef.ObjectType);
		}

		string GetBoolString(bool value)
		{
			return value ? "Да" : "Нет";
		}

		public static event ResolveObjectValue ResolveObjectValue;
		public static event ResolveObjectName ResolveObjectName;
	}

	public delegate object ResolveObjectValue(Guid objectUID, ObjectType objectType);
	public delegate string ResolveObjectName(Guid objectUID, ObjectType objectType);
}