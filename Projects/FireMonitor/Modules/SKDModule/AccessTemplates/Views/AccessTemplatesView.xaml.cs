using System.Windows.Controls;
using Controls.TreeList;

namespace SKDModule.Views
{
	public partial class AccessTemplatesView : UserControl, IWithDeletedView
	{
		public AccessTemplatesView()
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