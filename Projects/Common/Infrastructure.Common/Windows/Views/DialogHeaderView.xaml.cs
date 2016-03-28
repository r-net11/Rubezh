using System;
using Infrastructure.Common.Windows.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Infrastructure.Common.Windows.Views
{
	public partial class DialogHeaderView : UserControl
	{
		public DialogHeaderView()
		{
			InitializeComponent();
		}

		private void OnCloseButton(object sender, RoutedEventArgs e)
		{
			(((DialogHeaderViewModel)DataContext).Content).Close(false);
		}
	}
}