using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Infrastructure.Common
{
    public interface IUserDialogService
    {
        bool ShowModalWindow(IDialogContent model);
        bool ShowModalWindow(IDialogContent model, Window parentWindow);
    }
}
