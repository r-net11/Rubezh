using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Infrastructure.Common;

namespace FireMonitor
{
	public class UserDialogService : IUserDialogService
	{
		List<DialogWindow> ActiveWindows = new List<DialogWindow>();

		public void ShowWindow(IDialogContent viewModel, bool isTopMost = false)
		{
			var dialog = new DialogWindow()
			{
				Topmost = isTopMost
			};
			dialog.SetContent(viewModel);

			var guidContent = viewModel as IDialogContentGuid;
			if (guidContent != null)
			{
				DialogWindow existingWindow = ActiveWindows.FirstOrDefault(x => x.ViewModel is IDialogContentGuid && ((IDialogContentGuid)x.ViewModel).Guid == guidContent.Guid);
				if (existingWindow != null)
				{
					existingWindow.Activate();
					return;
				}
				dialog.Closed += new System.EventHandler(dialog_Closed);
				ActiveWindows.Add(dialog);
			}
			dialog.Show();
		}

		void dialog_Closed(object sender, System.EventArgs e)
		{
			ActiveWindows.Remove((DialogWindow)sender);
		}

		public bool ShowModalWindow(IDialogContent model)
		{
			return Infrastructure.Common.MessageBox.UserDialogService.ShowModalWindow(model, null);
		}
	}
}