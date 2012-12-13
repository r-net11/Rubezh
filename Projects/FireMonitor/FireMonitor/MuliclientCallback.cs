using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MuliclientAPI;
using Infrastructure.Common.Windows;

namespace FireMonitor
{
    public class MuliclientCallback : IMuliclientCallback
    {
        public void Show()
        {
            ApplicationService.ApplicationWindow.Show();
            ApplicationService.ApplicationWindow.Activate();
        }

        public void Hide()
        {
            ApplicationService.ApplicationWindow.Hide();
        }

        public WindowSize GetWindowSize()
        {
            var windowSize = new WindowSize()
            {
                Left = ApplicationService.ApplicationWindow.Left,
                Top = ApplicationService.ApplicationWindow.Top,
                Width = ApplicationService.ApplicationWindow.Width,
                Height = ApplicationService.ApplicationWindow.Height
            };
            //MessageBoxService.Show("ApplicationService.ApplicationWindow.Left = " + ApplicationService.ApplicationWindow.Left.ToString());
            return windowSize;
        }

        public void SetWindowSize(WindowSize windowSize)
        {
            ApplicationService.ApplicationWindow.Left = windowSize.Left;
            ApplicationService.ApplicationWindow.Top = windowSize.Top;
            ApplicationService.ApplicationWindow.Width = windowSize.Width;
            ApplicationService.ApplicationWindow.Height = windowSize.Height;
        }
    }
}