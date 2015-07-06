﻿using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Controls
{
	public static class Masking
	{
		private static readonly DependencyPropertyKey _maskExpressionPropertyKey = DependencyProperty.RegisterAttachedReadOnly("MaskExpression", typeof(Regex), typeof(Masking), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty MaskProperty = DependencyProperty.RegisterAttached("Mask", typeof(string), typeof(Masking), new FrameworkPropertyMetadata(OnMaskChanged));
		public static readonly DependencyProperty MaskExpressionProperty = _maskExpressionPropertyKey.DependencyProperty;

		public static string GetMask(TextBox textBox)
		{
			if (textBox == null)
				throw new ArgumentNullException("textBox");
			return textBox.GetValue(MaskProperty) as string;
		}

		public static void SetMask(TextBox textBox, string mask)
		{
			if (textBox == null)
				throw new ArgumentNullException("textBox");
			textBox.SetValue(MaskProperty, mask);
		}

		public static Regex GetMaskExpression(TextBox textBox)
		{
			if (textBox == null)
				throw new ArgumentNullException("textBox");
			return textBox.GetValue(MaskExpressionProperty) as Regex;
		}

		private static void SetMaskExpression(TextBox textBox, Regex regex)
		{
			textBox.SetValue(_maskExpressionPropertyKey, regex);
		}

		private static void OnMaskChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var textBox = dependencyObject as TextBox;
			var mask = e.NewValue as string;
			textBox.PreviewTextInput -= textBox_PreviewTextInput;
			textBox.PreviewKeyDown -= textBox_PreviewKeyDown;
			DataObject.RemovePastingHandler(textBox, Pasting);

			if (mask == null)
			{
				textBox.ClearValue(MaskProperty);
				textBox.ClearValue(MaskExpressionProperty);
			}
			else
			{
				textBox.SetValue(MaskProperty, mask);
				SetMaskExpression(textBox, new Regex(mask, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace));
				textBox.PreviewTextInput += textBox_PreviewTextInput;
				textBox.PreviewKeyDown += textBox_PreviewKeyDown;
				DataObject.AddPastingHandler(textBox, Pasting);
			}
		}

		private static void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			var textBox = sender as TextBox;
			var maskExpression = GetMaskExpression(textBox);
			if (maskExpression == null)
				return;
			var proposedText = GetProposedText(textBox, e.Text);
			if (!maskExpression.IsMatch(proposedText))
				e.Handled = true;
		}

		private static void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			var textBox = sender as TextBox;
			var maskExpression = GetMaskExpression(textBox);
			if (maskExpression == null)
				return;
			if (e.Key == Key.Space)
			{
				var proposedText = GetProposedText(textBox, " ");
				if (!maskExpression.IsMatch(proposedText))
					e.Handled = true;
			}
		}

		private static void Pasting(object sender, DataObjectPastingEventArgs e)
		{
			var textBox = sender as TextBox;
			var maskExpression = GetMaskExpression(textBox);
			if (maskExpression == null)
				return;
			if (e.DataObject.GetDataPresent(typeof(string)))
			{
				var pastedText = e.DataObject.GetData(typeof(string)) as string;
				var proposedText = GetProposedText(textBox, pastedText);
				if (!maskExpression.IsMatch(proposedText))
					e.CancelCommand();
			}
			else
				e.CancelCommand();
		}

		private static string GetProposedText(TextBox textBox, string newText)
		{
			var text = textBox.Text;
			if (textBox.SelectionStart != -1)
				text = text.Remove(textBox.SelectionStart, textBox.SelectionLength);
			text = text.Insert(textBox.CaretIndex, newText);
			return text;
		}
	}
}