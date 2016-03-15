using Infrastructure.Common;
using KeyGenerator;
using System;
using System.Windows;

namespace FiresecService.Views
{
	/// <summary>
	/// Interaction logic for RegistrationWindow.xaml
	/// </summary>
	public partial class RegistrationWindow : Window
	{
		public RegistrationWindow()
		{
			InitializeComponent();
			Title = "Регистрация продукта A.C. Tech";
		}

		private void RegistrationWindow_OnClosed(object sender, EventArgs e)
		{
			Bootstrapper.Close();
		}
	}
}
