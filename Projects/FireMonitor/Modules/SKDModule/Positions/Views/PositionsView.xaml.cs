using System.Windows.Controls;
using Controls.TreeList;

namespace SKDModule.Views
{
	public partial class PositionsView : UserControl, IWithDeletedView
	{
		public PositionsView()
		{
			InitializeComponent();
			_changeIsDeletedViewSubscriber = new ChangeIsDeletedViewSubscriber(this);
		}

		ChangeIsDeletedViewSubscriber _changeIsDeletedViewSubscriber;

		public TreeList TreeList
		{
			get { return _treeList; }
			set { _treeList = value; }
		}
	}
}