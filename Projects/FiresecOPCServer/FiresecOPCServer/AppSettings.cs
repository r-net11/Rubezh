using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecOPCServer
{
    public static class AppSettings
    {
        public static string FS_Address { get; set; }
        public static int FS_Port { get; set; }
        public static string FS_Login { get; set; }
        public static string FS_Password { get; set; }
        public static string ServerAddress { get; set; }
        public static string Login { get; set; }
        public static string Password { get; set; }
        public static bool IsImitatorEnabled { get; set; }
    }
}