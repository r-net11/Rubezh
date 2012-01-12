using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Infrastructure.Common
{
    public partial class DialogWindow : Window
    {
        public object ViewModel { get; private set; }

        void Header_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                this.DragMove();
        }

        public DialogWindow()
        {
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
        }

        void OnSourceInitialized(object sender, EventArgs e)
        {
            UserControl userControl = FindUserControl(this);
            if (userControl != null)
            {
                var oldHeight = ActualHeight;
                var oldWidth = ActualWidth;

                var newHeight = userControl.MinHeight + 30; // + dialog window title heigh
                var newWidth = userControl.MinWidth;
                MinHeight = newHeight + 30 + 30;
                MinWidth = newWidth + 30;

                if (double.IsNaN(Height))
                {
                    Height = MinHeight;
                    Width = MinWidth;

                    Left += ((oldWidth - ActualWidth) / 2);
                    Top += ((oldHeight - ActualHeight) / 2);
                }
            }
        }

        static UserControl FindUserControl(DependencyObject obj)
        {
            UserControl result = null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); ++i)
            {
                DependencyObject childObj = VisualTreeHelper.GetChild(obj, i);
                if (childObj != null)
                {
                    result = childObj as UserControl;
                    if (result != null)
                        break;

                    result = FindUserControl(childObj);
                    if (result != null)
                        break;
                }
            }
            return result;
        }

        public void SetContent(IDialogContent content)
        {
            if (!string.IsNullOrEmpty(content.Title))
            {
                Title = content.Title;
                _captionTextBlock.Text = content.Title;
            }

            if (content is SaveCancelDialogContent)
                _okCancelStackPanel.Visibility = System.Windows.Visibility.Visible;
            else
                _okCancelStackPanel.Visibility = System.Windows.Visibility.Collapsed;
            _okCancelStackPanel.DataContext = content;

            _content.Content = content.InternalViewModel;
            content.Surface = this;

            ViewModel = content.InternalViewModel;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CloseContent();
            }
        }

        void OnCloseButton(object sender, RoutedEventArgs e)
        {
            Close();
            CloseContent();
        }

        void CloseContent()
        {
            var content = Content as IDialogContent;
            if (content != null)
                content.Close(false);
        }

        void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Width + e.HorizontalChange > 10)
                this.Width += e.HorizontalChange;
            if (this.Height + e.VerticalChange > 10)
                this.Height += e.VerticalChange;
        }
    }
}