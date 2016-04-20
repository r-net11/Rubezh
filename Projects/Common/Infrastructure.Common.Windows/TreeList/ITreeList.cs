using Common;

namespace Infrastructure.Common.TreeList
{
	public interface ITreeList
	{
		ObservableCollectionAdv<TreeNodeViewModel> Rows { get; }
		object SelectedTreeNode { get; set; }
		void SuspendSelection();
		void ResumeSelection();
	}
}
