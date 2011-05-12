using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Controls
{
    public interface IDialogContent
    {
        string Title { get; }
        void Close(bool result);
        Window Surface { set; }
        object InternalViewModel { get; }
    }
}
