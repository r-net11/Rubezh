using RubezhAPI.Automation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Automation
{
	public static class OpcDaHelper
	{
		static object _locker = new object();
		static List<OpcDaTagValue> _tagValues = new List<OpcDaTagValue>();

		static WriteTagValueDelegate _writeTagValue;

		public static void Initialize(List<OpcDaServer> opcDaServers, WriteTagValueDelegate writeTagValue)
		{
			UpdateTagList(opcDaServers);
			_writeTagValue = writeTagValue;
		}

		public static void UpdateTagList(List<OpcDaServer> opcDaServers)
		{
			var inTagValues = opcDaServers.SelectMany(server => server.Tags.Select(tag => new OpcDaTagValue
				{
					ElementName = tag.ElementName,
					TagId = tag.TagId,
					Path = tag.Path,
					TypeNameOfValue = tag.TypeNameOfValue,
					AccessRights = tag.AccessRights,
					ScanRate = tag.ScanRate,
					ServerId = server.Uid,
					ServerName = server.ServerName
				})).ToList();

			lock (_locker)
			{
				foreach (var inTagValue in inTagValues)
				{
					var tagValue = _tagValues.FirstOrDefault(x => x.Uid == inTagValue.Uid);
					if (tagValue != null)
						inTagValue.Value = tagValue.Value;
				}
				_tagValues = inTagValues;
			}
		}

		public static OpcDaTagValue GetTagValue(Guid tagUID)
		{
			return _tagValues.FirstOrDefault(x => x.Uid == tagUID);
		}

		public static void SetTagValue(Guid tagUID, object value)
		{
			lock (_locker)
			{
				var tagValue = _tagValues.FirstOrDefault(x => x.Uid == tagUID);
				if (tagValue != null)
					tagValue.Value = value;
			}
		}

		public static void OnWriteTagValue(Guid tagUID, object value)
		{
			if (_writeTagValue != null)
				_writeTagValue(tagUID, value);
		}
	}

	public delegate void WriteTagValueDelegate(Guid tagUID, object value);
}
