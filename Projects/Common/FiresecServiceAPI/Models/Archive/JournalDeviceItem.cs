using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class JournalDeviceItem
	{
		[Column(DbType = "NVarChar(MAX)")]
		[DataMember]
		public string PanelName { get; set; }

		[Column(DbType = "NVarChar(MAX)")]
		[DataMember]
		public string PanelDatabaseId { get; set; }
	}
}