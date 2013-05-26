using System.Windows.Controls;

namespace ClientFS2.Views
{
    /// <summary>
    /// Логика взаимодействия для ZoneSelectationView.xaml
    /// </summary>
    public partial class ZoneSelectationView
    {
        public ZoneSelectationView()
        {
            InitializeComponent();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            if (listView != null && listView.SelectedItem != null)
            {
                listView.ScrollIntoView(listView.SelectedItem);
            }
        }
    }
}
