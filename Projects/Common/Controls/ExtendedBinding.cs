using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace Controls
{
	public class ExtendedBinding : Decorator
	{
		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(object), typeof(ExtendedBinding),
		  new FrameworkPropertyMetadata(OnPropertyChanged) { BindsTwoWayByDefault = true, DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, });
		public Object Source
		{
			get { return GetValue(ExtendedBinding.SourceProperty); }
			set { SetValue(ExtendedBinding.SourceProperty, value); }
		}

		public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(object), typeof(ExtendedBinding),
		  new FrameworkPropertyMetadata(OnPropertyChanged) { BindsTwoWayByDefault = true, DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, });
		public Object Target
		{
			get { return GetValue(ExtendedBinding.TargetProperty); }
			set { SetValue(ExtendedBinding.TargetProperty, value); }
		}

		private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var sourceValue = d.GetValue(ExtendedBinding.SourceProperty);
			var targetValue = d.GetValue(ExtendedBinding.TargetProperty);
			if (e.Property == ExtendedBinding.SourceProperty && !object.ReferenceEquals(sourceValue, targetValue))
				d.SetValue(ExtendedBinding.TargetProperty, sourceValue);
			else if (e.Property == ExtendedBinding.TargetProperty && !object.ReferenceEquals(sourceValue, targetValue))
				d.SetValue(ExtendedBinding.SourceProperty, targetValue);
		}
	}
}
