namespace Infrastructure.Common.TreeList
{
	public interface IItemComparer
	{
		int Compare(TreeNodeViewModel x, TreeNodeViewModel y);
	}
}