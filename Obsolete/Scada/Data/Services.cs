using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControlBase;

namespace Data
{
    public class Services
    {
        public static Action<UserControlBase, string> ShowBindingWindow;
        public static void OnShowBindingWindow(UserControlBase source, string propertyName)
        {
            if (ShowBindingWindow != null)
                ShowBindingWindow(source, propertyName);
        }

        public static Action<UserControlBase, string> ShowEventBindingWindow;
        public static void OnShowEventBindingWindow(UserControlBase source, string eventName)
        {
            if (ShowEventBindingWindow != null)
                ShowEventBindingWindow(source, eventName);
        }
    }
}
