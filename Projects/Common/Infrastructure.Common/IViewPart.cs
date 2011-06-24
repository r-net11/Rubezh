using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Infrastructure.Common
{
    public interface IViewPart :
        INotifyPropertyChanged
    {
        void OnShow();
        void OnHide();
    }
}
