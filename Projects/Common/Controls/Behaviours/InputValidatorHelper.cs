using System.Text;
using System.Windows.Controls;

namespace Controls.Behaviours
{
	public static class InputValidatorHelper
	{
		public static string GetActualText(string text)
		{
			var sb = new StringBuilder();
			foreach (var t in text)
			{
				sb.Append(t);
			}
			return sb.ToString();
		}

		public static string GetFullText(TextBox textBox, string inputText)
		{
			var text = textBox.Text;
			if (textBox.SelectedText.Length > 0)
			{
				text = text.Remove(textBox.CaretIndex, textBox.SelectionLength);
				if (textBox.CaretIndex >= text.Length)
				{
					text = text + inputText;
				}
				else
				{
					text = text.Insert(textBox.CaretIndex, inputText);
				}
			}
			else
			{
				var startIndex = textBox.CaretIndex;
				if (startIndex >= text.Length)
				{
					text = text + inputText;
				}
				else
				{
					text = text.Insert(startIndex, inputText);
				}
			}
			return text;
		}
	}
}
