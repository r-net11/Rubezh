﻿using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class OpcDaTagFilter
	{
		#region Constructors
		public OpcDaTagFilter()
		{
			UID = Guid.NewGuid();
			TagUID = Guid.Empty;
			Name = string.Empty;
			Description = string.Empty;
		}
		public OpcDaTagFilter(Guid filterGuid, string name, string description, Guid tagUid, double hysteresis, ExplicitType valueType)
		{
			UID = filterGuid;
			TagUID = tagUid;
			Hysteresis = hysteresis;
			ValueType = valueType;
			Description = description == null ? string.Empty : description;
			Name = name == null ? string.Empty : name;
			switch (valueType)
			{
				case ExplicitType.Boolean: { Value = false; break; }
				case ExplicitType.DateTime: { Value = DateTime.Now; break; }
				case ExplicitType.Integer: { Value = (Int32)0; break; }
				case ExplicitType.Float: { Value = (Double)0; break; }
				case ExplicitType.String: { Value = String.Empty; break; }
				default:
					{
						throw new NotSupportedException(String.Format(
							"Невозможно создать фильтр для типа данных", valueType));
					}
			}
		}
		#endregion

		#region Properties
		[DataMember]
		public Guid UID { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string Description { get; set; }
		[DataMember]
		public Guid TagUID { get; set; }
		[DataMember]
		public double Hysteresis { get; set; }
		[DataMember]
		public ExplicitType ValueType { get; set; }
		[XmlIgnore]
		public object Value { get; set; }
		#endregion

		/// <summary>
		/// Сравнивает предыдущее сохранённое значение с переданным
		/// и с учётом гистерезиса возвращает true - если значение изменилось
		/// </summary>
		/// <param name="tagValue"></param>
		/// <returns></returns>
		public bool CheckCondition(object tagValue)
		{
			if (tagValue == null)
				return false;
			if (Value.GetType() != tagValue.GetType())
				return false;

			var type = GetExplicitType(Value.GetType().ToString());

			if (type == null)
				return false;

			switch (type)
			{
				case ExplicitType.Integer:
					{
						var max = System.Convert.ToInt32(Value) + Hysteresis;
						var min = System.Convert.ToInt32(Value) - Hysteresis;
						var current = System.Convert.ToInt32(tagValue);

						if ((current > max) || (current < min))
						{
							Value = current;
							return true;
						}
						return false;
					}
				case ExplicitType.Float:
					{
						var max = System.Convert.ToDouble(Value) + Hysteresis;
						var min = System.Convert.ToDouble(Value) - Hysteresis;
						var current = System.Convert.ToDouble(tagValue);

						if ((current > max) || (current < min))
						{
							Value = current;
							return true;
						}
						return false;
					}
				case ExplicitType.String:
					{
						var current = (string)tagValue;
						var result = !((string)Value).Equals(current);
						if (result)
							Value = current;
						return result; 
					}
				case ExplicitType.DateTime:
					{
						var current = (DateTime)tagValue;
						var result = (DateTime)Value != current;
						if (result)
							Value = current;
						return result;
					}
				case ExplicitType.Boolean:
					{
						var current = (Boolean)tagValue;
						var result = (Boolean)Value != current;
						if (result)
							Value = current;
						return result;
					}
				default: return true;
			}

		}

		public static ExplicitType? GetExplicitType(string typeName)
		{
			switch (typeName)
			{
				case "System.Boolean":
					return ExplicitType.Boolean;
				case "System.DateTime":
					return ExplicitType.DateTime;
				case "System.Double":
				case "System.Single":
					return ExplicitType.Float;
				case "System.Int16":
				case "System.Int32":
					return ExplicitType.Integer;
				case "System.String":
					return ExplicitType.String;
				default:
					return null;
			}
		}
	}
}