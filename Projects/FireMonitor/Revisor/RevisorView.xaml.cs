using System.Windows;

namespace Revisor
{
	public partial class RevisorView : Window
	{
		public RevisorView()
		{
			InitializeComponent();
			var revisorViewModel = new RevisorViewModel();
			DataContext = revisorViewModel;
		}
	}
}