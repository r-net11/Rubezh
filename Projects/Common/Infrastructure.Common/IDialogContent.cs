using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Infrastructure.Common
{
    public interface IDialogContent
    {
        string Title { get; }
        void Close(bool result);
        Window Surface { set; }
        object InternalViewModel { get; }
        bool Result { get; set; }
    }
}
