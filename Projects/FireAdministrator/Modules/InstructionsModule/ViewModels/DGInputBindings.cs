using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Media;

namespace InstructionsModule.ViewModels
{
    public class DGInputBindings : UserControl
    {
        public static InputBindingCollection GetInputBindings(UserControl element)
        {
            return (InputBindingCollection)element.GetValue(InputBindingsProperty);
        }

        public static void SetInputBindings(
          UserControl element, InputBindingCollection value)
        {
            element.SetValue(InputBindingsProperty, value);
        }

        public static readonly DependencyProperty InputBindingsProperty =
            DependencyProperty.RegisterAttached(
            "InputBindings",
            typeof(InputBindingCollection),
            typeof(UserControl), new FrameworkPropertyMetadata());
    }
}
