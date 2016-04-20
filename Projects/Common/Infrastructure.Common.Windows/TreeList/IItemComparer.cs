namespace Infrastructure.Common.Windows.TreeList
{
	public interface IItemComparer
	{
		int Compare(TreeNodeViewModel x, TreeNodeViewModel y);
	}
}