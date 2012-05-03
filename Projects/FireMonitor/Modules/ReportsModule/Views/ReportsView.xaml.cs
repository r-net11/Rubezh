using System.Windows.Controls;
using ReportsModule.ViewModels;

namespace ReportsModule.Views
{
    public partial class ReportsView : UserControl
    {
		ReportsViewModel _reportsViewModel;

        public ReportsView()
        {
            InitializeComponent();
			
			MouseMove += new System.Windows.Input.MouseEventHandler(ReportsView_MouseMove);
        }

		void ReportsView_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (_reportsViewModel == null)
				_reportsViewModel = DataContext as ReportsViewModel;
			_reportsViewModel.Update();
		}
    }
}