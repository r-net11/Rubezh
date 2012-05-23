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

namespace Infrastructure.Client.Login.Views
{
	public partial class LoginView : UserControl
	{
		public LoginView()
		{
			InitializeComponent();
			Loaded += new System.Windows.RoutedEventHandler(LoginView_Loaded);
		}

		void LoginView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			_btnOK.Focus();
		}
	}
}
