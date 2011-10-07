using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JournalModule.ViewModels;

namespace JournalModule.Views
{
    public partial class ArchiveFilterView : UserControl
    {
        public ArchiveFilterView()
        {
            InitializeComponent();
        }

        void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox.IsChecked != null)
            {
                bool isChecked = (bool) checkBox.IsChecked;
                var archiveFilter = DataContext as ArchiveFilterViewModel;
                var classId = (checkBox.DataContext as ClassViewModel).Id;
                archiveFilter.JournalEvents.Where(x => x.ClassId == classId).All(x => (x.IsEnable = isChecked) == isChecked);
            }
        }

        void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var archiveFilter = DataContext as ArchiveFilterViewModel;
            var classId = (checkBox.DataContext as EventViewModel).ClassId;

            if (archiveFilter.JournalEvents.Where(x => x.ClassId == classId).All(x => x.IsEnable == checkBox.IsChecked))
            {
                archiveFilter.JournalTypes.Find(x => x.Id == classId).IsEnable = checkBox.IsChecked;
            }
            else
            {
                archiveFilter.JournalTypes.Find(x => x.Id == classId).IsEnable = null;
            }
        }

        void CheckBox_Indeterminate(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var archiveFilter = DataContext as ArchiveFilterViewModel;
            var classId = (checkBox.DataContext as ClassViewModel).Id;
            if (archiveFilter.JournalEvents.Where(x => x.ClassId == classId).All(x => x.IsEnable))
            {
                checkBox.IsChecked = false;
                e.Handled = true;
            }
            else if (archiveFilter.JournalEvents.Where(x => x.ClassId == classId).All(x => !x.IsEnable))
            {
                checkBox.IsChecked = true;
                e.Handled = true;
            }
        }
    }
}