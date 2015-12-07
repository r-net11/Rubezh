using System;
using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class OpcDaEditingTagsTagViewModel : OpcDaEditingTagsTreeElementViewModel
	{
		public OpcDaEditingTagsTagViewModel(OpcDaTag tag)
		{
			IsTag = true;
			Tag = tag;
			ElementName = tag.TagName;
		}
		public OpcDaTag Tag { get; private set; }

		bool _isChecked;
		public bool IsChecked 
		{
			get {return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}
	}
}