using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Controls
{
	public class Attach
	{
		public static readonly DependencyProperty InputBindingsProperty = DependencyProperty.RegisterAttached("InputBindings", typeof(InputBindingCollection), typeof(Attach), new FrameworkPropertyMetadata(new InputBindingCollection(), OnInputBindingsChanged));

		public static InputBindingCollection GetInputBindings(UIElement element)
		{
			return (InputBindingCollection)element.GetValue(InputBindingsProperty);
		}
		public static void SetInputBindings(UIElement element, InputBindingCollection inputBindings)
		{
			element.SetValue(InputBindingsProperty, inputBindings);
		}

		private static void OnInputBindingsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var element = dependencyObject as UIElement;
			if (element == null)
				return;
			element.InputBindings.Clear();
			element.InputBindings.AddRange((InputBindingCollection)e.NewValue);
		}
	}
}
