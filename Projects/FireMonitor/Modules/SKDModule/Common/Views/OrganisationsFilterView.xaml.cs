using System.Windows;
using System.Windows.Controls;

namespace SKDModule.Views
{
	public partial class OrganisationsFilterView : UserControl
	{
		public static readonly DependencyProperty IsCheckBoxesProperty = DependencyProperty.Register(
			"IsCheckBoxes",
			typeof(bool),
			typeof(OrganisationsFilterView),
			new FrameworkPropertyMetadata(true)
			);

		public bool IsCheckBoxes
		{
			get { return (bool)GetValue(IsCheckBoxesProperty); }
			set { SetValue(IsCheckBoxesProperty, value); }
		}

		public OrganisationsFilterView()
		{
			InitializeComponent();
		}
	}
}