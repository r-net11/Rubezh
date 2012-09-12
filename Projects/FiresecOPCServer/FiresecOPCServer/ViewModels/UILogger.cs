using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecOPCServer.ViewModels
{
    public static class UILogger
    {
        public static void Log(string message)
        {
            if (MainViewModel.Current != null)
            {
                MainViewModel.Current.AddLog(message);
            }
        }
    }
}