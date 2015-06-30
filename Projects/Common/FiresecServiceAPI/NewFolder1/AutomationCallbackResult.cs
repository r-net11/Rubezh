using System;
using System.Runtime.Serialization;
using FiresecAPI.Automation;

namespace FiresecAPI.AutomationCallback
{
	[DataContract]
	public class AutomationCallbackResult
	{
		public AutomationCallbackResult()
		{
		}

		[DataMember]
		public AutomationCallbackType AutomationCallbackType { get; set; }

		[DataMember]
		public AutomationCallbackData Data { get; set; }

		//[DataMember]
		//public AutomationUIElementProperty AutomationUIElementProperty { get; set; }
	}

	//[DataContract]
	//public class AutomationUIElementProperty
	//{
	//    [DataMember]
	//    public string ElementName { get; set; }

	//    [DataMember]
	//    public string PropertyName { get; set; }

	//    [DataMember]
	//    public string PropertyValue { get; set; }
	//}
}