using System;
using RubezhAPI.Automation;
using Infrastructure.Common.TreeList;

namespace AutomationModule.ViewModels
{
	public class OpcDaEditingTagsTreeElementViewModel : 
		TreeNodeViewModel<OpcDaEditingTagsTreeElementViewModel>
	{
		public bool IsTag { get; protected set; }
		public string ElementName { get; protected set; }
	}

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

	public class OpcDaEditingTagsTagViewModel : OpcDaEditingTagsTreeElementViewModel
	{
		public OpcDaEditingTagsTagViewModel(OpcDaTag tag)
		{
			IsTag = true;
			Tag = tag;
			ElementName = tag.TagName;
		}
		public OpcDaTag Tag { get; private set; }
		public bool IsChecked { get; set; }
	}
}