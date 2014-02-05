using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class GKProgressCallback
	{
		public GKProgressCallback()
		{
			UID = Guid.NewGuid();
			LastActiveDateTime = DateTime.Now;
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public GKProgressCallbackType GKProgressCallbackType { get; set; }

		[DataMember]
		public string Title { get; set; }

		[DataMember]
		public string Text { get; set; }

		[DataMember]
		public int StepCount { get; set; }

		[DataMember]
		public bool CanCancel { get; set; }

		[DataMember]
		public GKProgressClientType GKProgressClientType { get; set; }

		public bool IsCanceled { get; set; }
		public DateTime CancelizationDateTime { get; set; }
		public DateTime LastActiveDateTime { get; set; }
	}

	public enum GKProgressCallbackType
	{
		Start,
		Progress,
		Stop
	}

	public enum GKProgressClientType
	{
		Administrator,
		Monitor
	}
}