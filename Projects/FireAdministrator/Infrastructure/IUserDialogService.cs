using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Controls;

namespace Infrastructure
{
    public interface IUserDialogService
    {
        bool ShowModalWindow(IDialogContent model);
    }
}
