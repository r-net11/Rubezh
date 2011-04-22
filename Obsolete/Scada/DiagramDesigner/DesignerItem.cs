using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;
using System;

namespace DiagramDesigner
{
    public class DesignerItem : ContentControl
    {
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected", typeof(bool),
                                      typeof(DesignerItem),
                                      new FrameworkPropertyMetadata(false));

        static DesignerItem()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerItem), new FrameworkPropertyMetadata(typeof(DesignerItem)));
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            MainViewModel.Current.ShowProperty(this.Content);

            base.OnPreviewMouseDown(e);
            DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;

            if (designer != null)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                {
                    this.IsSelected = !this.IsSelected;
                }
                else
                {
                    if (!this.IsSelected)
                    {
                        designer.DeselectAll();
                        this.IsSelected = true;
                    }
                }
            }

            e.Handled = false;
        }
    }
}
