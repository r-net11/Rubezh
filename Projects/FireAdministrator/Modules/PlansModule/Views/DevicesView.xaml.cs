using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FiresecAPI.Models;
using PlansModule.Designer;
using PlansModule.ViewModels;

namespace PlansModule.Views
{
    public partial class DevicesView : UserControl
    {
        public DevicesView()
        {
            InitializeComponent();
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
                var device = ((sender as Image).DataContext as DeviceViewModel).Device;
                if (device.Driver.IsPlaceable == false)
                    return;

                ElementBase plansElement = new ElementDevice()
                {
                    DeviceUID = device.UID
                };

                var designerItemData = new DesignerItemData()
                {
                    PlansElement = plansElement
                };

                DataObject dataObject = new DataObject("DESIGNER_ITEM", designerItemData);
                DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);
            }

            e.Handled = true;
        }
    }
}
