using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceVisualizer
{
    public static class WindowManager
    {
        public static void Show()
        {
            View view = new View();
            ViewModel viewModel = new ViewModel();
            view.DataContext = viewModel;
            view.ShowDialog();
        }
    }
}
