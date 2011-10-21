using System.Windows.Controls;
using Infrastructure;

namespace DevicesModule.Views
{
    public partial class ValidationErrorsView : UserControl
    {
        public ValidationErrorsView()
        {
            InitializeComponent();
        }

        void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ServiceFactory.Layout.ShowValidationArea(null);
        }
    }
}