using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using FireAdministrator.ViewModels;

namespace FireAdministrator.Services
{
    public class ProgressService
    {
        public ProgressService()
        {

        }

        ProgressViewModel ProgressViewModel;

        public void Show()
        {
            ProgressViewModel = new ProgressViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(ProgressViewModel);
        }

        public void Hide()
        {
            ProgressViewModel.CloseProgress();
        }
    }
}
