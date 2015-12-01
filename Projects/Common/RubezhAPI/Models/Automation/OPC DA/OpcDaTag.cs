using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class OpcDaTag
	{
		[DataMember]
		public string TagId { get; set; }
		[DataMember]
		public string TagName { get; set; }
		//public OpcDaTagsGroup TagsGroups { get; set; }
		[DataMember]
		public string Path { get; set; }

		public static string[] GetGroupNamesAndCheckFormatFromPath(string path)
		{
			const int segmentsMinAmount = 2; // Минимальное количество сегментов в пути 2: .\Имя тега
			var segments = path.Split(new string[] { @"\" }, System.StringSplitOptions.None);
			
			if (segments.Length < segmentsMinAmount)
			{
				throw new Exception(string.Format("Путь тега слишком короткий: {0}", path));
			}
			if (segments[0] != ".")
			{
				throw new Exception(string.Format("Некорректный коренвой элемент пути: {0}", path));
			}

			return segments;
		}
	}
}
