using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using Firesec.CoreConfig;
using FiresecClient.Models;

namespace SecurityModule.ViewModels
{
    public class GroupViewModel : BaseViewModel
    {
        public GroupViewModel(UserGroup group)
        {
            Name = group.Name;
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
