using System.Runtime.Serialization;
using StrazhAPI.Automation;

namespace StrazhAPI.AutomationCallback
{
	[DataContract]
	[KnownType(typeof(MessageCallbackData))]
	[KnownType(typeof(SoundCallbackData))]
	[KnownType(typeof(VisualPropertyCallbackData))]
	[KnownType(typeof(PlanCallbackData))]
	[KnownType(typeof(DialogCallbackData))]
	[KnownType(typeof(PropertyCallBackData))]
	public class AutomationCallbackData
	{
		public AutomationCallbackData()
		{
			LayoutFilter = new ProcedureLayoutCollection();
		}

		[DataMember]
		public ProcedureLayoutCollection LayoutFilter { get; set; }
	}
}