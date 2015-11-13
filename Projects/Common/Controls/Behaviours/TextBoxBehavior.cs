using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Controls.Behaviours
{
	public abstract class TextBoxBehavior : Behavior<TextBox>
	{
		protected abstract Func<string, bool> Validator { get; }

		protected override void OnAttached()
		{
			AssociatedObject.PreviewTextInput += PreviewLimitedTextInput;
			AssociatedObject.PreviewKeyDown += PreviewLimitedKeyDown;
			DataObject.AddPastingHandler(AssociatedObject, OnLimitedPasteHandler);
		}

		private void PreviewLimitedKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Space) return;

			var handled = false;
			PreviewLimitedInputHelper(sender, " ", ref handled);
			e.Handled = handled;
		}

		private void PreviewLimitedInputHelper(object sender, string inputText, ref bool handled)
		{
			var textBox = sender as TextBox;
			var fullText = InputValidatorHelper.GetFullText(textBox, inputText);
			var actualText = InputValidatorHelper.GetActualText(fullText);

			var isTextValid = Validator(actualText);

			handled = !isTextValid;
		}

		private void PreviewLimitedTextInput(object sender, TextCompositionEventArgs e)
		{
			bool handled = false;
			PreviewLimitedInputHelper(sender, e.Text, ref handled);
			e.Handled = handled;
		}

		private void OnLimitedPasteHandler(object sender, DataObjectPastingEventArgs e)
		{
			var textBox = sender as TextBox;

			var isText = e.SourceDataObject.GetDataPresent(DataFormats.Text, true);
			if (!isText) return;

			var clipboardText = (string)e.SourceDataObject.GetData(DataFormats.Text);

			var fullText = InputValidatorHelper.GetFullText(textBox, clipboardText);
			var actualText = InputValidatorHelper.GetActualText(fullText);

			var isValid = Validator(actualText);

			if (!isValid) e.CancelCommand();
		}
	}
}
