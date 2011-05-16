using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;

namespace FireMonitor
{
    public class UserDialogService : IUserDialogService
    {
        public bool ShowModalWindow(IDialogContent model)
        {
            DialogWindow dialog = new DialogWindow();
            dialog.SetContent(model);

            dialog.ShowDialog();
            return true;
        }
    }
}
