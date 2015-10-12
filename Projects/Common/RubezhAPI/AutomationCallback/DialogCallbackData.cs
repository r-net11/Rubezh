using System;
using System.Runtime.Serialization;

namespace RubezhAPI.AutomationCallback
{
	[DataContract]
	public class DialogCallbackData : AutomationCallbackData
	{
		[DataMember]
		public Guid Layout { get; set; }
		[DataMember]
		public string Title { get; set; }
		[DataMember]
		public bool IsModalWindow { get; set; }
		[DataMember]
		public bool AllowClose { get; set; }
		[DataMember]
		public bool AllowMaximize { get; set; }
		[DataMember]
		public bool Sizable { get; set; }
		[DataMember]
		public bool TopMost { get; set; }
		[DataMember]
		public double Width { get; set; }
		[DataMember]
		public double Height { get; set; }
		[DataMember]
		public double MinWidth { get; set; }
		[DataMember]
		public double MinHeight { get; set; }
		[DataMember]
		public bool CustomPosition { get; set; }
		[DataMember]
		public double Left { get; set; }
		[DataMember]
		public double Top { get; set; }
	}
}
