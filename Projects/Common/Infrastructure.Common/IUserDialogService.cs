using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common
{
    public interface IUserDialogService
    {
        bool ShowModalWindow(IDialogContent model);
    }
}
