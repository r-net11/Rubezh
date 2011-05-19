using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using Firesec.CoreConfig;

namespace SecurityModule.ViewModels
{
    public class GroupViewModel : BaseViewModel
    {
        public GroupViewModel(userGroupType group)
        {
            Name = group.name;
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
    }
}
