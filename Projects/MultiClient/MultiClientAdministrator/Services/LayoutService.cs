using System;

namespace MultiClient.Services
{
    public class LayoutService
    {
        public void Initialize(ShellView shellView)
        {
            _shellView = shellView;
        }

        ShellView _shellView;

        ShellView Shell
        {
            get { return _shellView; }
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
    }
}
