using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Markup;
using System.Reflection;
using System.Windows;
using System.Windows.Resources;

namespace ActiveX
{
    public static class ResourceHelper
    {
        public static object BamlReader(Stream stream)
        {
            ParserContext pc = new ParserContext();
            MethodInfo loadBamlMethod = typeof(XamlReader).GetMethod("LoadBaml",
                BindingFlags.NonPublic | BindingFlags.Static);
            return loadBamlMethod.Invoke(null, new object[] { stream, pc, null, false });
        }
        public static StreamResourceInfo sri = System.Windows.Application.GetResourceStream(
            new Uri("pack://application:,,,/Controls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;component/Themes/DataGridStyle.xaml"));
        public static ResourceDictionary resources = (ResourceDictionary)BamlReader(sri.Stream);
    }
}
