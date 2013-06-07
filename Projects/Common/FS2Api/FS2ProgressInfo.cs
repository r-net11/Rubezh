using System.Runtime.Serialization;

namespace FS2Api
{
	[DataContract]
	public class FS2ProgressInfo
	{
		public FS2ProgressInfo()
		{

		}

		public FS2ProgressInfo(string comment, int percentComplete = 0)
		{
			Comment = comment;
			PercentComplete = percentComplete;
		}

		[DataMember]
		public int Stage { get; set; }

		[DataMember]
		public string Comment { get; set; }

		[DataMember]
		public int PercentComplete { get; set; }

		[DataMember]
		public int BytesRW { get; set; }
	}
}