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

		public FS2ProgressInfo(string comment, int percentComplete, int currentStage, int stageCount)
		{
			Comment = comment;
			PercentComplete = percentComplete;
			CurrentStage = currentStage;
			StageCount = stageCount;
		}

		[DataMember]
		public int CurrentStage { get; set; }

		[DataMember]
		public int StageCount { get; set; }

		[DataMember]
		public string Comment { get; set; }

		[DataMember]
		public int PercentComplete { get; set; }

		[DataMember]
		public int BytesRW { get; set; }
	}
}