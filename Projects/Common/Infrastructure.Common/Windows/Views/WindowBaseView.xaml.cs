using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Infrastructure.Common.Windows.Views
{
	public partial class WindowBaseView : Window
	{
		public WindowBaseView()
		{
			InitializeComponent();
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
		}
		private void Window_Closed(object sender, System.EventArgs e)
		{
		}
	}
}
