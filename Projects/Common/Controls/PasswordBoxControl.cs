using System.Windows;
using System.Windows.Controls;

namespace Controls
{	
	public class PasswordBoxControl : Decorator
	{
		public static readonly DependencyProperty PasswordProperty =
			DependencyProperty.Register("Password", typeof(string), typeof(PasswordBoxControl),
			new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnPasswordPropertyChanged)));

		public PasswordBoxControl()
		{
			savedCallback = HandlePasswordChanged;

			var passwordBox = new PasswordBox();
			passwordBox.ContextMenu = null;
			passwordBox.PasswordChanged += savedCallback;
			Child = passwordBox;
		}

		private bool isPreventCallback;
		private RoutedEventHandler savedCallback;

		public string Password
		{
			get { return GetValue(PasswordProperty) as string; }
			set { SetValue(PasswordProperty, value); }
		}

		private static void OnPasswordPropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			PasswordBoxControl passwordBoxControl = dp as PasswordBoxControl;
			if (passwordBoxControl == null)
			{
				return;
			}
			PasswordBox passwordBox = passwordBoxControl.Child as PasswordBox;

			if (passwordBoxControl.isPreventCallback)
			{
				return;
			}

			passwordBox.PasswordChanged -= passwordBoxControl.savedCallback;
			passwordBox.Password = (e.NewValue != null) ? e.NewValue.ToString() : "";
			passwordBox.PasswordChanged += passwordBoxControl.savedCallback;
		}

		private void HandlePasswordChanged(object sender, RoutedEventArgs e)
		{
			PasswordBox passwordBox = sender as PasswordBox;
			if (passwordBox == null)
			{
				return;
			}
			isPreventCallback = true;
			Password = passwordBox.Password;
			isPreventCallback = false;
		}
	}
}