using System.Windows;
using System.Windows.Controls;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.Windows.Views
{
	public partial class ApplicationHeaderView : UserControl
	{
		public ApplicationHeaderView()
		{
			InitializeComponent();
		}

		private ApplicationHeaderViewModel ViewModel
		{
			get { return (ApplicationHeaderViewModel)DataContext; }
		}
		private ApplicationViewModel Application
		{
			get { return (ApplicationViewModel)ViewModel.Content; }
		}

		private void OnClose(object sender, RoutedEventArgs e)
		{
			Application.Close();
		}
		private void OnMinimize(object sender, RoutedEventArgs e)
		{
			Application.Minimize();
		}
		private void OnMaximize(object sender, RoutedEventArgs e)
		{
			Application.Maximize();
		}
		private void OnShowHelp(object sender, RoutedEventArgs e)
		{
			Application.ShowHelp();
		}
		private void OnShowAbout(object sender, RoutedEventArgs e)
		{
			Application.ShowAbout();
		}
	}
}