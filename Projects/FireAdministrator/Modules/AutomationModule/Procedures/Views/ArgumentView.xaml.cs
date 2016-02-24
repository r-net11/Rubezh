using System.Windows;

namespace AutomationModule.Views
{
	public partial class ArgumentView
	{
		public ArgumentView()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty EditorEnabledDependencyProperty = DependencyProperty.Register("ArgumentEnabled", typeof(bool), typeof(ArgumentView), new PropertyMetadata(true));

		public bool ArgumentEnabled
		{
			get { return (bool)GetValue(EditorEnabledDependencyProperty); }
			set { SetValue(EditorEnabledDependencyProperty, value); }
		}
	}
}