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

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox.IsChecked != null)
            {
                bool isChecked = (bool) checkBox.IsChecked;
                var archiveFilter = DataContext as ArchiveFilterViewModel;
                var classId = (checkBox.DataContext as ClassViewModel).Id;
                archiveFilter.JournalEvents.ForEach(
                    x => { if (x.ClassId == classId) x.IsEnable = isChecked; });
            }
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var archiveFilter = DataContext as ArchiveFilterViewModel;
            var classId = (checkBox.DataContext as EventViewModel).ClassId;
            if (archiveFilter.JournalEvents.TrueForAll(
                x =>
                {
                    if (x.ClassId == classId) return x.IsEnable == checkBox.IsChecked;
                    return true;
                }))
            {
                archiveFilter.JournalTypes.Find(x => x.Id == classId).IsEnable = checkBox.IsChecked;
            }
            else
            {
                archiveFilter.JournalTypes.Find(x => x.Id == classId).IsEnable = null;
            }
        }
    }
}