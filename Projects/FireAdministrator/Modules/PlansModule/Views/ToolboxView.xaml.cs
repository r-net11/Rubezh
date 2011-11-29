using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FiresecAPI.Models;
using System.Diagnostics;

namespace PlansModule.Views
{
    public partial class ToolboxView : UserControl
    {
        public ToolboxView()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(ToolboxView_Loaded);
        }

        private Point? dragStartPoint = null;

        private void On_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            this.dragStartPoint = new Point?(e.GetPosition(this));
        }

        private void On_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                this.dragStartPoint = null;
            }

            if (this.dragStartPoint.HasValue)
            {
                var elementBase = CreateNewElementBase((sender as Image).Name);
                var dataObject = new DataObject("DESIGNER_ITEM", elementBase);
                DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);
            }

            e.Handled = true;
        }

        ElementBase CreateNewElementBase(string itemType)
        {
            ElementBase elementBase = null;

            switch (itemType)
            {
                case "ElementRectangle":
                    elementBase = new ElementRectangle();
                    break;

                case "ElementEllipse":
                    elementBase = new ElementEllipse();
                    break;

                case "ElementPolygon":
                    elementBase = new ElementPolygon();
                    break;

                case "ElementTextBlock":
                    elementBase = new ElementTextBlock();
                    break;

                case "ElementRectangleZone":
                    elementBase = new ElementRectangleZone();
                    break;

                case "ElementPolygonZone":
                    elementBase = new ElementPolygonZone();
                    break;

                case "ElementSubPlan":
                    elementBase = new ElementSubPlan();
                    break;
            }

            return elementBase;
        }

        void ToolboxView_Loaded(object sender, RoutedEventArgs e)
        {
            FocusManager.SetIsFocusScope(_stackPanel, true);
        }

        private void StackPanel_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Trace.WriteLine("StackPanel_GotKeyboardFocus");
            FocusManager.SetIsFocusScope(_stackPanel, true);
        }

        private void StackPanel_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Trace.WriteLine("StackPanel_LostKeyboardFocus");
            FocusManager.SetIsFocusScope(_stackPanel, true);
        }
    }
}
