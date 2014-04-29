using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class AccessTemplateViewModelNameComparer : TreeNodeComparer<AccessTemplateViewModel>
	{
		protected override int Compare(AccessTemplateViewModel x, AccessTemplateViewModel y)
		{
			return string.Compare(x.Name, y.Name);
		}
	}

	public class AccessTemplateViewModelDescriptionComparer : TreeNodeComparer<AccessTemplateViewModel>
	{
		protected override int Compare(AccessTemplateViewModel x, AccessTemplateViewModel y)
		{
			return string.Compare(x.Description, y.Description);
		}
	}
}