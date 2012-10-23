using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace Infrastructure.Common.Theme
{
    public class ThemeHelper
    {
        public static string CurrentTheme { get; private set; }

        public static void SetThemeIntoRegister(Theme selectedTheme)
        {
            try
            {
                RegistryKey saveKey = Registry.LocalMachine.CreateSubKey("software\\rubezh");
                saveKey.SetValue("Theme", selectedTheme);
                saveKey.Close();
            }
            catch (Exception)
            {
                {}
                throw;
            }
        }

        public static void LoadThemeFromRegister()
        {
            try
            {
                RegistryKey readKey = Registry.LocalMachine.OpenSubKey("software\\rubezh");
                CurrentTheme = (string)readKey.GetValue("Theme");
                readKey.Close();
                if (String.IsNullOrEmpty(CurrentTheme))
                    CurrentTheme = "BlueTheme";
                var themePath = "pack://application:,,,/Controls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;component/Themes/" + CurrentTheme + ".xaml";
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(themePath) });
            }
            catch (Exception)
            {
                CurrentTheme = "BlueTheme";
                var themePath = "pack://application:,,,/Controls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;component/Themes/" + CurrentTheme + ".xaml";
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(themePath) });
            }
        }
    }
}
