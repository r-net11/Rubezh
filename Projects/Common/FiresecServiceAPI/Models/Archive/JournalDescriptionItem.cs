using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class JournalDescriptionItem
	{
		[Column(DbType = "Int")]
		[DataMember]
		public StateType StateType { get; set; }

		[Column(DbType = "NVarChar(MAX)")]
		[DataMember]
		public string Description { get; set; }
	}
}