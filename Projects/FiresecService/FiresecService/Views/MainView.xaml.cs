using System.Windows.Controls;

namespace FiresecService.Views
{
    public partial class MainView : UserControl
    {
        public static MainView Current { get; private set; }

        public MainView()
        {
            InitializeComponent();
            Current = this;
        }
    }
}