using System.Windows.Controls;
using Controls.TreeList;

namespace SKDModule.PassCardDesigner.Views
{
	public partial class TemplatesView : UserControl, IWithDeletedView
	{
		public TemplatesView()
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
