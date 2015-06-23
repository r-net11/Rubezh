using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Данные для обновления контроллера
	/// </summary>
	[DataContract]
	public class HEXFileInfo
	{
		public HEXFileInfo()
		{
			Lines = new List<string>();
		}

		[DataMember]
		public string FileName { get; set; }

		[DataMember]
		public List<string> Lines { get; set; }
	}
}