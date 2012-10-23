using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace Infrastructure.Common.Theme
{
    public enum Theme
    {
        [DescriptionAttribute("Синяя тема")]
        BlueTheme,
        
        [DescriptionAttribute("Серая тема")]
        GrayTheme
    }
}