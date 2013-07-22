using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AdministratorTestClientFS2.Views
{
	public partial class PasswordView : UserControl
	{
		public PasswordView()
		{
			InitializeComponent();
		}

		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !IsTextAllowed(e.Text);
		}

		private static bool IsTextAllowed(string text)
		{
			var regex = new Regex("[^0-9]+"); //regex that matches disallowed text
			return !regex.IsMatch(text);
		}

		private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
		{
			if (e.DataObject.GetDataPresent(typeof(String)))
			{
				var text = (String)e.DataObject.GetData(typeof(String));
				if (!IsTextAllowed(text))
				{
					e.CancelCommand();
				}
			}
			else
			{
				e.CancelCommand();
			}
		}

		private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			var ke = e;
			if (ke.Key == Key.Space)
			{
				ke.Handled = true;
			}
		}
	}
}