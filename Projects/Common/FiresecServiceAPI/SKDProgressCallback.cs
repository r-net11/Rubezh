using System;
using System.Runtime.Serialization;

namespace StrazhAPI
{
	[DataContract]
	public class SKDProgressCallback
	{
		public SKDProgressCallback()
		{
			UID = Guid.NewGuid();
			LastActiveDateTime = DateTime.Now;
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public SKDProgressCallbackType SKDProgressCallbackType { get; set; }

		[DataMember]
		public string Title { get; set; }

		[DataMember]
		public string Text { get; set; }

		[DataMember]
		public int StepCount { get; set; }

		[DataMember]
		public int CurrentStep { get; set; }

		[DataMember]
		public bool CanCancel { get; set; }

		[DataMember]
		public SKDProgressClientType SKDProgressClientType { get; set; }

		public bool IsCanceled { get; set; }

		public DateTime CancelizationDateTime { get; set; }

		public DateTime LastActiveDateTime { get; set; }
	}

	public enum SKDProgressCallbackType
	{
		Start,
		Progress,
		Stop
	}

	public enum SKDProgressClientType
	{
		Administrator,
		Monitor
	}
}