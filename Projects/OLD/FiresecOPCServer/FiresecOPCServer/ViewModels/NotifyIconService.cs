﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace FiresecOPCServer.ViewModels
{
    public static class NotifyIconService
    {
        private static System.Windows.Forms.NotifyIcon _notifyIcon = null;
        public static void Start(EventHandler onShow, EventHandler onClose)
        {
            RefreshTaskbarNotificationArea();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/FiresecOPCServer;component/OPC.ico")).Stream;
            _notifyIcon.Icon = new System.Drawing.Icon(iconStream);
            _notifyIcon.Visible = true;

            _notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
            var menuItem1 = new System.Windows.Forms.MenuItem();
            menuItem1.Text = "Показать";
            menuItem1.Click += new EventHandler(onShow);
            _notifyIcon.ContextMenu.MenuItems.Add(menuItem1);

            var menuItem2 = new System.Windows.Forms.MenuItem();
            menuItem2.Text = "Выход";
            menuItem2.Click += new EventHandler(onClose);
            _notifyIcon.ContextMenu.MenuItems.Add(menuItem2);
        }
        public static void Stop()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
                _notifyIcon = null;
            }
        }
        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Stop();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        public static void RefreshTaskbarNotificationArea()
        {
            IntPtr systemTrayContainerHandle = FindWindow("Shell_TrayWnd", null);
            IntPtr systemTrayHandle = FindWindowEx(systemTrayContainerHandle, IntPtr.Zero, "TrayNotifyWnd", null);
            IntPtr sysPagerHandle = FindWindowEx(systemTrayHandle, IntPtr.Zero, "SysPager", null);
            IntPtr notificationAreaHandle = FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", "Notification Area");
            if (notificationAreaHandle == IntPtr.Zero)
            {
                notificationAreaHandle = FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", "User Promoted Notification Area");
                IntPtr notifyIconOverflowWindowHandle = FindWindow("NotifyIconOverflowWindow", null);
                IntPtr overflowNotificationAreaHandle = FindWindowEx(notifyIconOverflowWindowHandle, IntPtr.Zero, "ToolbarWindow32", "Overflow Notification Area");
                RefreshTaskbarNotificationArea(overflowNotificationAreaHandle);
            }
            RefreshTaskbarNotificationArea(notificationAreaHandle);
        }
        private static void RefreshTaskbarNotificationArea(IntPtr windowHandle)
        {
            const uint wmMousemove = 0x0200;
            RECT rect;
            GetClientRect(windowHandle, out rect);
            for (var x = 0; x < rect.right; x += 5)
                for (var y = 0; y < rect.bottom; y += 5)
                    SendMessage(windowHandle, wmMousemove, 0, (y << 16) + x);
        }
    }
}