using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace Infrastructure.Common.TreeList
{
	public interface ITreeList
	{
		ObservableCollectionAdv<TreeNodeViewModel> Rows { get; }
		TreeNodeViewModel SelectedTreeNode { get; set; }
		void SuspendSelection();
		void ResumeSelection();
	}
}
