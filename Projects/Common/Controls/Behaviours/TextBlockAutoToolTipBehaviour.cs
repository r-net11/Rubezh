using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Controls.Behaviours
{
	public class TextBlockAutoToolTipBehaviour
	{
		/// <summary>
		///Получить значение AutoTooltipProperty свойства зависимости
		/// </summary>
		public static bool GetAutoTooltip(DependencyObject obj)
		{
			return (bool)obj.GetValue(AutoTooltipProperty);
		}

		/// <summary>
		/// Установить значение AutoTooltipProperty свойства зависимости
		/// </summary>
		public static void SetAutoTooltip(DependencyObject obj, bool value)
		{
			obj.SetValue(AutoTooltipProperty, value);
		}

		/// <summary>
		/// Когда true - установить TextBlock.TextTrimming
		/// свойство в WordEllipsis, и отобразить подсказку когда текст обрезан.
		/// </summary>
		public static readonly DependencyProperty AutoTooltipProperty = DependencyProperty.RegisterAttached("AutoTooltip",
				typeof(bool), typeof(TextBlockAutoToolTipBehaviour), new PropertyMetadata(false, OnAutoTooltipPropertyChanged));

		private static void OnAutoTooltipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var textBlock = d as TextBlock;
			if (textBlock == null)
				return;

			if (e.NewValue.Equals(true))
			{
				textBlock.TextTrimming = TextTrimming.CharacterEllipsis;
				ComputeAutoTooltip(textBlock);
				textBlock.SizeChanged += TextBlock_SizeChanged;
			}
			else
			{
				textBlock.SizeChanged -= TextBlock_SizeChanged;
			}
		}

		private static void TextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var textBlock = sender as TextBlock;
			ComputeAutoTooltip(textBlock);
		}

		/// <summary>
		/// Назначает подсказку, в зависимости от того, обрезан ли текст
		/// </summary>
		private static void ComputeAutoTooltip(TextBlock textBlock)
		{
			textBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
			var width = textBlock.DesiredSize.Width;

			ToolTipService.SetToolTip(textBlock, textBlock.ActualWidth < width ? textBlock.Text : null);
		}
	}
}
