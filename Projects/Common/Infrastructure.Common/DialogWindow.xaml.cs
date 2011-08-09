using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Infrastructure.Common
{
    public partial class DialogWindow : Window
    {
        private FrameworkElement _title;

        private ICommand _helpContextCommand;
        private int _helpTopicId;

        public DialogWindow()
        {
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
        }

        // TODO: what we are doing here?
        private void OnSourceInitialized(object sender, EventArgs e)
        {
            UserControl userControl = FindUserControl(this);
            if (userControl != null)
            {
                var oldHeight = ActualHeight;
                var oldWidth = ActualWidth;

                var newHeight = userControl.MinHeight + 30; // + dialog window title heigh
                var newWidth = userControl.MinWidth;

                MinHeight = Height = newHeight + 30;
                MinWidth = Width = newWidth + 30;

                Left += ((oldWidth - ActualWidth) / 2);
                Top += ((oldHeight - ActualHeight) / 2);
            }
        }

        private static UserControl FindUserControl(DependencyObject obj)
        {
            UserControl result = null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
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
                Title = content.Title;

            Content = content.InternalViewModel;
            content.Surface = this;
        }

        public string Caption
        {
            get { return Title; }
            set { Title = value; }
        }

        public void SetContextHelpCommand(ICommand helpContextCommand, int helpTopicId)
        {
            _helpContextCommand = helpContextCommand;
            _helpTopicId = helpTopicId;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _title = (FrameworkElement) Template.FindName("PART_Title", this);
            if (null != _title)
            {
                _title.MouseLeftButtonDown += OnMouseLeftButtonDown;
            }

            Button closeButton = Template.FindName("PART_Close", this) as Button;
            if (closeButton != null)
            {
                closeButton.Click += OnCloseButtonClick;
            }

            if (_helpContextCommand != null)
            {
                Button helpButton = Template.FindName("PART_Help", this) as Button;
                if (helpButton != null)
                {
                    helpButton.Command = _helpContextCommand;
                    helpButton.CommandParameter = _helpTopicId;
                    helpButton.Visibility = Visibility.Visible;
                }
            }
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            CloseContent();
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CloseContent();
            }
        }

        private void CloseContent()
        {
            var content = Content as IDialogContent;
            if (content != null)
            {
                content.Close(false);
            }
        }
    }
}