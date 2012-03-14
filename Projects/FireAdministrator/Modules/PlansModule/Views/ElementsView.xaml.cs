using System.Windows;
using System.Windows.Controls;

namespace PlansModule.Views
{
    public partial class ElementsView : UserControl
    {
        public ElementsView()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(ElementsView_Loaded);
            _elementsDataGrid.SelectionChanged += new SelectionChangedEventHandler(ElementsView_Loaded);
        }

        void ElementsView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_elementsDataGrid.SelectedItem != null)
                _elementsDataGrid.ScrollIntoView(_elementsDataGrid.SelectedItem);
        }
    }
}
