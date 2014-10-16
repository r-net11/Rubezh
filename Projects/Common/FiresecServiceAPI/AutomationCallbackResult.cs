using System;
using System.Runtime.Serialization;
using FiresecAPI.Automation;

namespace FiresecAPI
{
	[DataContract]
	public class AutomationCallbackResult
	{
		public AutomationCallbackResult()
		{
			AutomationUIElementProperty = new AutomationUIElementProperty();
			ProcedureLayoutCollection = new ProcedureLayoutCollection();
		}

		[DataMember]
		public AutomationCallbackType AutomationCallbackType { get; set; }

		[DataMember]
		public string Message { get; set; }

		[DataMember]
		public bool IsModalWindow { get; set; }

		[DataMember]
		public Guid SoundUID { get; set; }

		[DataMember]
		public AutomationUIElementProperty AutomationUIElementProperty { get; set; }

		[DataMember]
		public ProcedureLayoutCollection ProcedureLayoutCollection { get; set; }
	}

	[DataContract]
	public class AutomationUIElementProperty
	{
		[DataMember]
		public string ElementName { get; set; }

		[DataMember]
		public string PropertyName { get; set; }

		[DataMember]
		public string PropertyValue { get; set; }
	}

	public enum AutomationCallbackType
	{
		Message,
		Sound,
		UIElementPropertyChanged
	}
}