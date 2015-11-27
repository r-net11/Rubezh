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

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			if (Generator.VerifyProductKey(ProductKeyBox.Text))
			{
				Generator.SaveToFile(ProductKeyBox.Text, UserKeyBox.Text, AppDataFolderHelper.GetFile("LicData.dat"));
				MessageBox.Show("Регистрация прошла успешно. Перезапустите приложение для того, что бы изменения вступили в силу.");
				Bootstrapper.Close();
			}
			else
			{
				MessageBox.Show("Неверный ключ продукта.");
			}
		}

		private void RegistrationWindow_OnClosed(object sender, EventArgs e)
		{
			Bootstrapper.Close();
		}
	}
}
