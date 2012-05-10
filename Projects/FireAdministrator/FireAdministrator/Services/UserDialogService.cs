using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using Infrastructure;
using Infrastructure.Common;

namespace FireAdministrator
{
	public class UserDialogService : IUserDialogService
	{
		public void ShowWindow(IDialogContent model, bool isTopMost = false)
		{
			var dialogWindow = new DialogWindow()
			{
				Topmost = isTopMost,
			};
			dialogWindow.SetContent(model);

			dialogWindow.Show();
		}

		public bool ShowModalWindow(IDialogContent model)
		{
			return Infrastructure.Common.MessageBox.UserDialogService.ShowModalWindow(model, LoadSizeView);
		}

		private void LoadSizeView(DialogWindow dialogWindow)
		{
			var viewModel = dialogWindow.ViewModel;
			var isSaveSize = viewModel.GetType().GetCustomAttributes(true).Any(x => x is SaveSizeAttribute);
			if (isSaveSize)
			{
				var typeName = viewModel.GetType().Name;

				try
				{
					var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
					string stringWidth = configuration.AppSettings.Settings[typeName + "_Width"].Value;
					string stringHeight = ConfigurationManager.AppSettings[typeName + "_Height"];
					string stringLeft = ConfigurationManager.AppSettings[typeName + "_Left"];
					string stringTop = ConfigurationManager.AppSettings[typeName + "_Top"];

					dialogWindow.Width = double.Parse(stringWidth);
					dialogWindow.Height = double.Parse(stringHeight);
					dialogWindow.Left = double.Parse(stringLeft);
					dialogWindow.Top = double.Parse(stringTop);
				}
				catch (Exception) { ;}

				dialogWindow.Closed += new EventHandler(OnSaveSizeViewClosed);
			}
		}
		private void OnSaveSizeViewClosed(object sender, System.EventArgs e)
		{
			DialogWindow dialogWindow = sender as DialogWindow;
			var viewModel = dialogWindow.ViewModel;
			var typeName = viewModel.GetType().Name;

			var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			configuration.AppSettings.Settings.Remove(typeName + "_Width");
			configuration.AppSettings.Settings.Remove(typeName + "_Height");
			configuration.AppSettings.Settings.Remove(typeName + "_Left");
			configuration.AppSettings.Settings.Remove(typeName + "_Top");
			configuration.AppSettings.Settings.Add(typeName + "_Width", dialogWindow.Width.ToString());
			configuration.AppSettings.Settings.Add(typeName + "_Height", dialogWindow.Height.ToString());
			configuration.AppSettings.Settings.Add(typeName + "_Left", dialogWindow.Left.ToString());
			configuration.AppSettings.Settings.Add(typeName + "_Top", dialogWindow.Top.ToString());
			configuration.Save();
		}
	}
}