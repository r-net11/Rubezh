using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ControlBase;

namespace DiagramDesigner
{
    /// <summary>
    /// Interaction logic for ToolBoxItem.xaml
    /// </summary>
    public partial class ToolBoxItem : UserControl
    {
        public ToolBoxItem()
        {
            InitializeComponent();
        }

        UserControlBase userControlBase;
        public UserControlBase UserControlBase
        {
            get { return userControlBase; }
            set
            {
                userControlBase = value;
                IconImage.Source = userControlBase.GetImage();
                ControlName.Text = userControlBase.GetType().Name;
            }
        }

        private Point? dragStartPoint = null;

        private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            this.dragStartPoint = new Point?(e.GetPosition(this));
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                this.dragStartPoint = null;
            }

            if (this.dragStartPoint.HasValue)
            {
                Point position = e.GetPosition(this);
                if ((SystemParameters.MinimumHorizontalDragDistance <=
                    Math.Abs((double)(position.X - this.dragStartPoint.Value.X))) ||
                    (SystemParameters.MinimumVerticalDragDistance <=
                    Math.Abs((double)(position.Y - this.dragStartPoint.Value.Y))))
                {
                    //DataObject dataObject = new DataObject("DESIGNER_ITEM", this.Content.GetType());
                    DataObject dataObject = new DataObject("DESIGNER_ITEM", this.UserControlBase.GetType());

                    if (dataObject != null)
                    {
                        DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);
                    }
                }

                e.Handled = true;
            }
        }
    }
}
