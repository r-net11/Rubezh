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
    public class DGInputBindings : UIElement
    {
        public static InputBindingCollection GetInputBindings(UIElement element)
        {
            return (InputBindingCollection)element.GetValue(InputBindingsProperty);
        }

        public static void SetInputBindings(
          UIElement element, InputBindingCollection value)
        {
            element.SetValue(InputBindingsProperty, value);
        }

        public static readonly DependencyProperty InputBindingsProperty =
            DependencyProperty.RegisterAttached(
            "InputBindings",
            typeof(InputBindingCollection),
            typeof(UIElement), new FrameworkPropertyMetadata());
    }
}
