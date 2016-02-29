using RubezhAPI.Automation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Automation
{
	public static class OpcDaHelper
	{
		static object _locker = new object();
		static List<OpcDaTagConcept> _tags = new List<OpcDaTagConcept>();

		static ReadWriteTagValueDelegate _readTagValue;
		static ReadWriteTagValueDelegate _writeTagValue;

		public static void Initialize(List<OpcDaServer> opcDaServers, ReadWriteTagValueDelegate readTagValue, ReadWriteTagValueDelegate writeTagValue)
		{
			UpdateTagList(opcDaServers);
			_readTagValue = readTagValue;
			_writeTagValue = writeTagValue;
		}

		public static void UpdateTagList(List<OpcDaServer> opcDaServers)
		{
			var inTags = opcDaServers.SelectMany(server => server.Tags.Select(tag => new OpcDaTagConcept(tag.Uid, tag.TypeNameOfValue))).ToList();

			lock (_locker)
			{
				foreach (var inTag in inTags)
				{
					var tag = _tags.FirstOrDefault(x => x.UID == inTag.UID);
					if (tag != null)
						inTag.SetValue(tag.Value);
				}
				_tags = inTags;
			}
		}

		public static object GetTagValue(Guid tagUID)
		{
			var tag = _tags.FirstOrDefault(x => x.UID == tagUID);
			return tag == null ? null : tag.Value;
		}

		public static void SetTagValue(Guid tagUID, object value)
		{
			lock (_locker)
			{
				var tagValue = _tags.FirstOrDefault(x => x.UID == tagUID);
				if (tagValue != null)
					tagValue.SetValue(value);
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
				case "System.Int16":
				case "System.Int32":
					return ExplicitType.Integer;
				case "System.String":
					return ExplicitType.String;
				default:
					return null;
			}
		}

		public static void OnReadTagValue(Guid tagUID, object value)
		{
			if (_readTagValue != null)
				_readTagValue(tagUID, value);
		}

		public static void OnWriteTagValue(Guid tagUID, object value)
		{
			if (_writeTagValue != null)
				_writeTagValue(tagUID, value);
		}
	}

	public delegate void ReadWriteTagValueDelegate(Guid tagUID, object value);

	public class OpcDaTagConcept
	{
		public Guid UID { get; private set; }
		public ExplicitType ExplicitType { get; private set; }
		public bool IsArray { get; private set; }
		public object Value { get; private set; }

		public OpcDaTagConcept(Guid uid, string typeName)
		{
			UID = uid;
			IsArray = typeName.EndsWith("[]");
			if (IsArray)
				typeName = typeName.Remove(typeName.Length - 2);
			var explicitType = OpcDaHelper.GetExplicitType(typeName);
			ExplicitType = explicitType.HasValue ? explicitType.Value : ExplicitType.String;
		}

		public void SetValue(object value)
		{
			object result;
			var isOk = TryConvert(value, ExplicitType, IsArray, out result);
			if (isOk)
			{
				Value = result;
			}
		}

		bool TryConvert(object value, ExplicitType resultType, bool resultIsArray, out object result)
		{
			result = null;
			if (value == null)
				return true;

			var valueTypeName = value.GetType().ToString();

			var valueIsArray = valueTypeName.EndsWith("[]");
			if (valueIsArray != resultIsArray)
				return false;

			if (valueIsArray)
				valueTypeName = valueTypeName.Remove(valueTypeName.Length - 2);

			switch (resultType)
			{
				case ExplicitType.Boolean:
					if (valueTypeName == "System.Boolean")
						result = value;
					break;
				case ExplicitType.DateTime:
					if (valueTypeName == "System.DateTime")
						result = value;
					break;
				case ExplicitType.Integer:
					if (valueTypeName == "System.Int16" || valueTypeName == "System.Int32")
						result = resultIsArray ?
							(object)((IEnumerable)value).Cast<Int32>() :
							Convert.ToInt32(value);
					break;
				case ExplicitType.Float:
					if (valueTypeName == "System.Double" || valueTypeName == "System.Single")
						result = resultIsArray ?
							(object)((IEnumerable)value).Cast<Double>() :
							Convert.ToDouble(value);
					break;
				case ExplicitType.String:
					result = resultIsArray ?
							(object)((IEnumerable)value).Cast<String>() :
							value.ToString();
					break;
				default:
					return false;
			}

			return true;
		}
	}
}