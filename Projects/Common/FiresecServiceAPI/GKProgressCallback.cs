using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class GKProgressCallback
	{
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
	}

	public enum GKProgressCallbackType
	{
		Start,
		Progress,
		Stop
	}
}