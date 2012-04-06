using Infrastructure;
using Infrastructure.Common;
using FireMonitor.Views;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Linq;
using System;

namespace FireMonitor
{
    public class LayoutService : ILayoutService
    {
        List<ViewModelCash> ViewModelsCash = new List<ViewModelCash>();
        IViewPart CurrentViewModel;

        ShellView Shell
        {
            get { return (ShellView)ServiceFactory.ShellView; }
        }

        public void Show(IViewPart viewModel)
        {
            ViewModelCash activeModelCash = GetActiveViewModelCash(viewModel);

            foreach (var item in Shell._itemsControl.Items)
            {
                (item as ContentControl).Visibility = Visibility.Collapsed;
            }
            activeModelCash.ContentControl.Visibility = Visibility.Visible;

            if (CurrentViewModel != null)
            {
                CurrentViewModel.OnHide();
            }
            CurrentViewModel = viewModel;
            CurrentViewModel.OnShow();
        }

        public void Close()
        {
            Shell.Dispatcher.Invoke(new Action(OnClose));
        }

        void OnClose()
        {
            foreach (var item in Shell._itemsControl.Items)
            {
                (item as ContentControl).Visibility = Visibility.Collapsed;
            }

            if (CurrentViewModel != null)
            {
                CurrentViewModel.OnHide();
            }
            CurrentViewModel = null;
        }

        ViewModelCash GetActiveViewModelCash(IViewPart viewModel)
        {
            var modelName = viewModel.GetType().ToString();
            bool isNew = ViewModelsCash.Any(x => x.ViewModeName == modelName) == false;

            ViewModelCash activeModelCash = null;

            if (isNew)
            {
                var obsoleteViewModelsCash = ViewModelsCash.FirstOrDefault(x => x.ViewModeName == modelName);
                if (obsoleteViewModelsCash != null)
                {
                    Shell._itemsControl.Items.Remove(obsoleteViewModelsCash.ContentControl);
                    ViewModelsCash.Remove(obsoleteViewModelsCash);
                }

                var contentControl = new ContentControl()
                {
                    DataContext = viewModel,
                    Content = viewModel
                };
                Shell._itemsControl.Items.Add(contentControl);

                activeModelCash = new ViewModelCash()
                {
                    ViewModel = viewModel,
                    ViewModeName = modelName,
                    ContentControl = contentControl
                };
                ViewModelsCash.Add(activeModelCash);
            }
            else
            {
                activeModelCash = ViewModelsCash.FirstOrDefault(x => x.ViewModeName == modelName);
            }

            return activeModelCash;
        }

        //ShellView Shell
        //{
        //    get { return (ShellView)ServiceFactory.ShellView; }
        //}

        //public void Show(IViewPart model)
        //{
        //    Replace(model);
        //}

        //public void Close()
        //{
        //    Replace(null);
        //}

        //void Replace(IViewPart model)
        //{
        //    if (ActiveViewModel != null)
        //    {
        //        IViewPart temp = ActiveViewModel;
        //        try
        //        {
        //            //temp.Dispose();
        //        }
        //        catch
        //        {
        //        }
        //    }
        //    ActiveViewModel = model;
        //}

        //IViewPart ActiveViewModel
        //{
        //    get { return Shell.MainContent; }
        //    set { Shell.MainContent = value; }
        //}

        public void AddAlarmGroups(IViewPart model)
        {
            Shell.AlarmGroups = model;
        }
    }

    public class ViewModelCash
    {
        public string ViewModeName;
        public IViewPart ViewModel;
        public ContentControl ContentControl;
    }
}