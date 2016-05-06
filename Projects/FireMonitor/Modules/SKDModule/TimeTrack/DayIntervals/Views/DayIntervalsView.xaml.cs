using System.Windows.Controls;
using Controls.TreeList;
using StrazhAPI.SKD;
using SKDModule.ViewModels;

namespace SKDModule.Views
{
	public partial class DayIntervalsView : UserControl, IWithDeletedView
	{
		public DayIntervalsView()
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

		private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			var deletationType = (DataContext as DayIntervalsViewModel).IsWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active;
			_changeIsDeletedViewSubscriber = new ChangeIsDeletedViewSubscriber(this, deletationType);
		}
	}
}