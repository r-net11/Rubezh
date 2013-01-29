using System.Windows.Controls;
using NotificationModule.ViewModels;

namespace NotificationModule.Views
{
	/// <summary>
	/// Логика взаимодействия для ZoneSelectionView.xaml
	/// </summary>
	public partial class ZoneSelectionView : UserControl
	{
		public ZoneSelectionView()
		{
			InitializeComponent();
		}

		private void SelectedAvailableZoneDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var viewModel = DataContext as ZoneSelectionViewModel;
			if (viewModel.AddOneCommand.CanExecute(null))
				viewModel.AddOneCommand.Execute();
		}

		private void SelectedInstructionZoneDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var viewModel = DataContext as ZoneSelectionViewModel;
			if (viewModel.RemoveOneCommand.CanExecute(null))
				viewModel.RemoveOneCommand.Execute();
		}
	}
}