using System;
using System.Linq;
using FireAdministrator.Views;
using Infrastructure;
using Infrastructure.Common;
using System.Collections.Generic;
using System.Windows;
using DevicesModule;
using System.Windows.Controls;

namespace FireAdministrator
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
            bool isNew = ViewModelsCash.Any(x => x.ViewModel == viewModel) == false;

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

        public void ShowMenu(object model)
        {
            Shell.Menu = model;
        }

        public void ShowValidationArea(object model)
        {
            Shell.ValidatoinArea = model;
        }
    }

    public class ViewModelCash
    {
        public string ViewModeName;
        public IViewPart ViewModel;
        public ContentControl ContentControl;
    }
}