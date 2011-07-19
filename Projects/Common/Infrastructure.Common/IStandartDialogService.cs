using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common
{
    public interface IStandartDialogService
    {
        void ShowError(string message);
        void ShowWarning(string message);
        void ShowInfo(string message);
        bool ShowConfirmation(string message);
    }
}
