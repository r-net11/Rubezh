using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;

namespace PlansModule.ViewModels
{
    public class SelectedDeviceViewModel : BaseViewModel
    {
        string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                OnPropertyChanged("IsActive");
            }
        }
    }
}
