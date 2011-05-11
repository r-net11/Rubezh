using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeviceEditor
{
    public static class Helper
    {
        public static string EmptyFrame = "<?xml version=\"1.0\" encoding=\"utf-16\"?><Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"/>";
        public static string ErrorFrame = "<?xml version=\"1.0\" encoding=\"utf-16\"?><Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"><TextBlock Text=\"Error Xaml Code\" FontSize=\"100\" /> </Canvas>";
        public static string IconsPath = @"C:\Program Files\Firesec\Icons\";
    }
}
