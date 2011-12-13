using System.Windows.Controls;

namespace DevicesModule.Views
{
    public partial class ZonesSelectionView : UserControl
    {
        public static double CustomWidth { get; set; }
        public static double CustomHeight { get; set; }
        public static double CustomLeft { get; set; }
        public static double CustomTop { get; set; }

        public ZonesSelectionView()
        {
            InitializeComponent();
        }
    }
}