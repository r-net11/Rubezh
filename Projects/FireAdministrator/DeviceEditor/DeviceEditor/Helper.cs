using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DeviceEditor
{
    public static class Helper
    {
        static  Helper()
        {
            LoadBaseStates();
        }
        public static string EmptyFrame = "<?xml version=\"1.0\" encoding=\"utf-16\"?><Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"/>";
        public static string ErrorFrame = "<?xml version=\"1.0\" encoding=\"utf-16\"?><Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"><TextBlock Text=\"Error Xaml Code\" FontSize=\"100\" /> </Canvas>";
        public static string DevicesIconsPath = @"C:\Program Files\Firesec\Icons\";
        public static string StatesIconPath = @"C:\Rubezh\Projects\FireAdministrator\ActivexDevices\Library\states.png";
        public static ObservableCollection<string> BaseStatesList { get; set; }
        static void LoadBaseStates()
        {
            BaseStatesList = new
            ObservableCollection<string>
            {
                "Тревога",
                "Внимание (предтревожное)",
                "Неисправность",
                "Требуется обслуживание",
                "Обход устройств",
                "Неопределено",
                "Норма(*)",
                "Норма",
                "Базовый рисунок"
            };
        }
    }
}
