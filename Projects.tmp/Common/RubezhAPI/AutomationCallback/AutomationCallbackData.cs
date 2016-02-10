using System.Runtime.Serialization;

namespace RubezhAPI.AutomationCallback
{
	[DataContract]
	[KnownType(typeof(UIAutomationCallbackData))]
	[KnownType(typeof(GlobalVariableCallBackData))]
	[KnownType(typeof(OpcDaTagCallBackData))]
	public class AutomationCallbackData { }
}
