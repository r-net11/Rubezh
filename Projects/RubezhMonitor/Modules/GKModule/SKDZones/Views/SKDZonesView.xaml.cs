using System.Windows.Controls;
using System.Windows.Input;
using Controls;

namespace GKModule.Views
{
	public partial class SKDZonesView : UserControl
	{
		public SKDZonesView()
		{
			InitializeComponent();
			_zonesListBox.SelectionChanged += new SelectionChangedEventHandler(_zonesListBox_SelectionChanged);

			_scrollViewer.PreviewMouseDown += OnMouseMiddleDown;
			_scrollViewer.PreviewMouseUp += OnMouseMiddleUp;
			_scrollViewer.PreviewMouseMove += OnMiddleMouseMove;
			_scrollViewer.MouseLeave += OnMiddleMouseLeave;

			_scrollViewer.PreviewMouseWheel += new MouseWheelEventHandler(_scrollViewer_PreviewMouseWheel);
		}

		void _scrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (e.Delta > 0)
			{
				_scroll(e.Delta, 7);
			}
			if (e.Delta < 0)
			{
				_scroll(e.Delta, 7);
			}

		}

		void _scroll(int delta, int count)
		{
			if (delta > 0)
			{
				for (int i = 0; i < count; i++)
					_scrollViewer.LineUp();
				return;
			}

			if (delta < 0)
			{
				for (int i = 0; i < count; i++)
					_scrollViewer.LineDown();
				return;
			}
		}

		void _zonesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_zonesListBox.SelectedItem != null)
				_zonesListBox.ScrollIntoView(_zonesListBox.SelectedItem);
		}

		void OnMouseMiddleDown(object sender, MouseButtonEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				MiddleButtonScrollHelper.StartScrolling(_scrollViewer, e);
			}
		}

		void OnMouseMiddleUp(object sender, MouseButtonEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Released)
			{
				MiddleButtonScrollHelper.StopScrolling();
			}
		}

		void OnMiddleMouseMove(object sender, MouseEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				MiddleButtonScrollHelper.UpdateScrolling(e);
			}
		}

		void OnMiddleMouseLeave(object sender, MouseEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				MiddleButtonScrollHelper.StopScrolling();
			}
		}
	}
}