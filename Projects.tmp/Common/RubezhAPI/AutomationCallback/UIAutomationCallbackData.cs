using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.AutomationCallback
{
	[DataContract]
	[KnownType(typeof(MessageCallbackData))]
	[KnownType(typeof(SoundCallbackData))]
	[KnownType(typeof(VisualPropertyCallbackData))]
	[KnownType(typeof(PlanCallbackData))]
	[KnownType(typeof(ShowDialogCallbackData))]
	[KnownType(typeof(CloseDialogCallbackData))]
	[KnownType(typeof(PropertyCallBackData))]
	public class UIAutomationCallbackData : AutomationCallbackData
	{
		public UIAutomationCallbackData()
		{
			LayoutFilter = new List<Guid>();
		}

		[DataMember]
		public List<Guid> LayoutFilter { get; set; }
	}
}
