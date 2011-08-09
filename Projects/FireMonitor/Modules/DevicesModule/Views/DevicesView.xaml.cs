using System.Windows;
using System.Windows.Controls;

namespace DevicesModule.Views
{
    public partial class DevicesView : UserControl
    {
        public DevicesView()
        {
            InitializeComponent();
            Current = this;
            this.Loaded += new RoutedEventHandler(DevicesView_Loaded);
        }

        void DevicesView_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollToSelected();
        }

        void ScrollToSelected()
        {
            if (Current.dataGrid.SelectedItem != null)
            {
                dataGrid.ScrollIntoView(dataGrid.SelectedItem);
            }
        }

        public static DevicesView Current;
    }
}