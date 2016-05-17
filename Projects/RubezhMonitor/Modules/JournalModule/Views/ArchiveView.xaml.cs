using System.Windows.Controls;
using System.Windows.Input;

namespace JournalModule.Views
{
	public partial class ArchiveView : UserControl
	{
		public ArchiveView()
		{
			InitializeComponent();
		}

		void pageNumberTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				var bindingExpression = pageNumberTextBox.GetBindingExpression(TextBox.TextProperty);
				bindingExpression.UpdateSource();
			}
		}
	}
}