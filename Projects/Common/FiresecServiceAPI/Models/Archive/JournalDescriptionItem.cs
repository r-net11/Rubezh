using System.Runtime.Serialization;
using System.Data.Linq.Mapping;

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