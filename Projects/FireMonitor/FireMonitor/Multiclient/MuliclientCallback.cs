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
			ApplicationService.ApplicationWindow.ShowInTaskbar = true;
        }

        public void Hide()
        {
            ApplicationService.ApplicationWindow.Hide();
			ApplicationService.ApplicationWindow.ShowInTaskbar = false;
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