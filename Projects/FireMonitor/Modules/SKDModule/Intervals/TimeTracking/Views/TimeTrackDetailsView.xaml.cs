using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SKDModule.Views
{
	public partial class TimeTrackDetailsView : UserControl
	{
		public TimeTrackDetailsView()
		{
			InitializeComponent();
			Loaded += new System.Windows.RoutedEventHandler(TimeTrackDetailsView_Loaded);
		}

		void TimeTrackDetailsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			_grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
			_grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
			_grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });

			Rectangle rectangle1 = new Rectangle();
			rectangle1.ToolTip = "Интервал 1";
			rectangle1.Fill = new SolidColorBrush(Colors.Green);
			rectangle1.Stroke = new SolidColorBrush(Colors.Black);
			Grid.SetRow(rectangle1, 0);
			Grid.SetColumn(rectangle1, 0);
			_grid.Children.Add(rectangle1);

			Rectangle rectangle2 = new Rectangle();
			rectangle2.ToolTip = "Интервал 2";
			rectangle2.Fill = new SolidColorBrush(Colors.Green);
			rectangle2.Stroke = new SolidColorBrush(Colors.Black);
			Grid.SetRow(rectangle2, 0);
			Grid.SetColumn(rectangle2, 2);
			_grid.Children.Add(rectangle2);

			for (int i = 0; i < 24; i++)
			{
				_timeGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
				if (i > 0)
				{
					TextBlock timeTextBlock = new TextBlock();
					timeTextBlock.Text = i.ToString();
					timeTextBlock.Foreground = new SolidColorBrush(Colors.Black);
					Grid.SetRow(timeTextBlock, 0);
					Grid.SetColumn(timeTextBlock, i);
					_timeGrid.Children.Add(timeTextBlock);
				}
			}
		}
	}
}