using System.Runtime.Serialization;

namespace RubezhAPI.AutomationCallback
{
	[DataContract]
	public class CloseDialogCallbackData : UIAutomationCallbackData
	{
		[DataMember]
		public string WindowID { get; set; }
	}
}
