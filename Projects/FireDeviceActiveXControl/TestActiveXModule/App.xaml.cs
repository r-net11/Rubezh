using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace TestActiveXModule
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ResourceDictionary rd = new ResourceDictionary() { Source = new System.Uri("pack://application:,,,/Infrastructure.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;component/Themes/DataGridStyle.xaml") };
            //ResourceDictionary rd = new ResourceDictionary() { Source = new System.Uri("pack://application:,,,/CurrentDeviceModule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;component/DataTemplates/Dictionary.xaml") };
            Resources.MergedDictionaries.Add(rd);
        }
    }
}
