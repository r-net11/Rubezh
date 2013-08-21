using System.Windows.Controls;

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
			nameTextBox.Focus();
		}
	}
}