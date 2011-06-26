using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using Infrastructure;

namespace FireAdministrator
{
    public class LayoutService : ILayoutService
    {
        private ShellView _shellView;

        private ShellView Shell
        {
            get { return _shellView ?? (_shellView = ServiceFactory.Get<ShellView>()); }
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

        private void Replace(IViewPart model)
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
    }
}
