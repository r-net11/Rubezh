using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VideoModule.RVI_VSS.Views
{
	/// <summary>
	/// Логика взаимодействия для LayoutPartPropertyCameraPageView.xaml
	/// </summary>
	public partial class LayoutPartPropertyCameraPageView : UserControl
	{
		public LayoutPartPropertyCameraPageView()
		{
			InitializeComponent();
		}
	}

	public static class ComboUtil
	{
		private static readonly CommandBinding DeleteCommandBinding = new CommandBinding(ApplicationCommands.Delete, HandleExecuteDeleteCommand);

		private static void HandleExecuteDeleteCommand(object sender, ExecutedRoutedEventArgs e)
		{
			var combo = e.Source as ComboBox;
			if (combo != null)
				combo.SelectedIndex = -1;
		}

		#region AllowNull Property

		public static bool GetAllowNull(ComboBox combo)
		{
			if (combo == null)
				throw new ArgumentNullException("combo");

			return (bool)combo.GetValue(AllowNullProperty);
		}

		public static void SetAllowNull(ComboBox combo, bool value)
		{
			if (combo == null)
				throw new ArgumentNullException("combo");

			combo.SetValue(AllowNullProperty, value);
		}

		public static readonly DependencyProperty AllowNullProperty =
			DependencyProperty.RegisterAttached(
				"AllowNull",
				typeof(bool),
				typeof(ComboUtil),
				new UIPropertyMetadata(HandleAllowNullPropertyChanged));

		private static void HandleAllowNullPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var combo = (ComboBox)o;
			if (true.Equals(e.NewValue))
			{
				if (!combo.CommandBindings.Contains(DeleteCommandBinding))
					combo.CommandBindings.Add(DeleteCommandBinding);
			}
			else
			{
				combo.CommandBindings.Remove(DeleteCommandBinding);
			}
		}

		#endregion
	}
}
