using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace LibraryModule
{
    public static class Helper
    {
        static Helper()
        {
            LoadBaseStates();
        }

        public static string EmptyFrame = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n</Canvas>";
        public static string ErrorFrame = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n<Border BorderBrush=\"Red\" BorderThickness=\"5\" Padding=\"20\">\n<TextBlock Text=\"Error Xaml Code\" FontSize=\"60\" />\n</Border>\n</Canvas>";
        public static string Svg2XamlFileName = AppDomain.CurrentDomain.BaseDirectory + "svg2xaml.xsl";
        public static ObservableCollection<string> BaseStatesList { get; set; }

        static void LoadBaseStates()
        {
            BaseStatesList = new ObservableCollection<string>
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

        public static void Draw(ICollection<Canvas> stateCanvases, ref Canvas canvas, string image, int layer)
        {
            if (stateCanvases != null && canvas != null)
            {
                stateCanvases.Remove(canvas);
                using (var stringReader = new StringReader(image))
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    canvas = (Canvas) XamlReader.Load(xmlReader);
                    Panel.SetZIndex(canvas, layer);
                    stateCanvases.Add(canvas);
                }
            }
        }
    }
}
