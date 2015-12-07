using Infrastructure.Common.TreeList;

namespace AutomationModule.ViewModels
{
	public class OpcDaEditingTagsTreeElementViewModel :
		TreeNodeViewModel<OpcDaEditingTagsTreeElementViewModel>
	{
		public bool IsTag { get; protected set; }
		public string ElementName { get; protected set; }
	}
}
