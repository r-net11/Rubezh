using System;
using Infrastructure;
using Infrastructure.Common;
using FireAdministrator.Views;

namespace FireAdministrator
{
    public class LayoutService : ILayoutService
    {
        ShellView Shell
        {
            get { return (ShellView)ServiceFactory.ShellView; }
        }

        public void Show(IViewPart model)
        {
            Replace(model);
            model.OnShow();
        }

        public void Close()
        {
            Replace(null);
        }

        void Replace(IViewPart model)
        {
            if (ActiveViewModel != null)
            {
                IViewPart temp = ActiveViewModel;
                try
                {
                    temp.OnHide();
                }
                catch (Exception)
                {
                }
            }
            ActiveViewModel = model;
        }

        IViewPart ActiveViewModel
        {
            get { return Shell.MainContent; }
            set
            {
                Shell.MainContent = value;
            }
        }

        public void ShowMenu(object model)
        {
            Shell.Menu = model;
        }

        public void ShowValidationArea(object model)
        {
            Shell.ValidatoinArea = model;
        }
    }
}