using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;

namespace DevicesModule.ViewModels
{
    public class GuardViewModel : RegionViewModel
    {
        public void Initialize()
        {
            GuardDevicesViewModel = new ViewModels.GuardDevicesViewModel();
            GuardDevicesViewModel.Initialize();

            GuardUsersViewModel = new ViewModels.GuardUsersViewModel();
            GuardUsersViewModel.Initialize();

            GuardLevelsViewModel = new GuardLevelsViewModel();
            GuardLevelsViewModel.Initialize();
        }

        public GuardDevicesViewModel GuardDevicesViewModel { get; private set; }
        public GuardUsersViewModel GuardUsersViewModel { get; private set; }
        public GuardLevelsViewModel GuardLevelsViewModel { get; private set; }

        bool _isGuardUsersSelected;
        public bool IsGuardUsersSelected
        {
            get { return _isGuardUsersSelected; }
            set
            {
                _isGuardUsersSelected = value;
                OnPropertyChanged("IsGuardUsersSelected");

                if (value)
                {
                    GuardUsersViewModel.OnShow();
                }
                else
                {
                    GuardUsersViewModel.OnHide();
                }
            }
        }
    }
}
