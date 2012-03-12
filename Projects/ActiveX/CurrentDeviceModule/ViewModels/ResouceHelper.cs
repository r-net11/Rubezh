using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Reflection;
using System.IO;
using System.Windows.Resources;
using System.Windows;

namespace CurrentDeviceModule.ViewModels
{
    public class ResourceHelper
    {
        static object BamlReader(Stream stream)
        {
            ParserContext pc = new ParserContext();
            MethodInfo loadBamlMethod = typeof(XamlReader).GetMethod("LoadBaml", BindingFlags.NonPublic | BindingFlags.Static);
            return loadBamlMethod.Invoke(null, new object[] { stream, pc, null, false });
        }

        static List<string> _styleNames
        {
            get
            {
                return new List<string>(){
                    "Converters.xaml",
                    "Brushes.xaml",
                    "ActiveXScrollBarStyle.xaml",
                    "ActiveXDataGridStyle.xaml",
                    "TreeExpanderStyle.xaml",
                    "OutlookTabControlStyle.xaml",
                    "ToolBarButtonStyle.xaml",
                    "HyperlinkStyle.xaml",
                    "MenuStyle.xaml",
                    "TabControlStyle.xaml",
                    "ButtonStyle.xaml",
                    "TextBlockStyle.xaml",
                    "TextBoxStyle.xaml",
                    "PasswordBoxStyle.xaml",
                    "ListBoxStyle.xaml",
                    "TreeViewStyle.xaml",
                    "ColorPickerComboBoxStyle.xaml",
                    "ComboBoxStyle.xaml",
                    "CheckBoxStyle.xaml",
                    "RadioButtonStyle.xaml",
                    "SliderStyle.xaml",
                    "ProgressBarStyle.xaml",
                    "ContextMenuStyle.xaml",
                    "ResizeThumbStyle.xaml"
                };
            }
        }
        
        public static ResourceDictionary GetResourceDictionary()
        {
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            foreach (var styleName in _styleNames)
            {
                var sri = System.Windows.Application.GetResourceStream(
                    new System.Uri("pack://application:,,,/Controls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;component/Themes/" + styleName, UriKind.Absolute));
                resourceDictionary.MergedDictionaries.Add((ResourceDictionary)BamlReader(sri.Stream));
            }

            return resourceDictionary;
        }

    }
}
