using Common;

namespace Infrastructure.Common.Windows.TreeList
{
	public interface ITreeList
	{
		ObservableCollectionAdv<TreeNodeViewModel> Rows { get; }
		object SelectedTreeNode { get; set; }
		void SuspendSelection();
		void ResumeSelection();
	}
}
