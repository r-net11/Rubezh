using System;

namespace AutomationModule.ViewModels
{
	public class OpcDaEditingTagsGroupViewModel : OpcDaEditingTagsTreeElementViewModel
	{
		public OpcDaEditingTagsGroupViewModel(string groupName)
		{
			IsTag = false;

			if (groupName == null)
			{
				throw new ArgumentNullException("groupName");
			}
			ElementName = groupName;
		}
	}
}