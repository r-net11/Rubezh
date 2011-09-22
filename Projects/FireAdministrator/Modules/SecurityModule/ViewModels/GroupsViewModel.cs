using System.Collections.ObjectModel;
using System.Windows;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace SecurityModule.ViewModels
{
    public class GroupsViewModel : RegionViewModel
    {
        public GroupsViewModel()
        {
            Groups = new ObservableCollection<GroupViewModel>();
            if (FiresecManager.SecurityConfiguration.UserGroups != null)
            {
                foreach (var group in FiresecManager.SecurityConfiguration.UserGroups)
                    Groups.Add(new GroupViewModel(group));
            }

            DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
            EditCommand = new RelayCommand(OnEdit, CanEditDelete);
            AddCommand = new RelayCommand(OnAdd);
        }

        public ObservableCollection<GroupViewModel> Groups { get; private set; }

        GroupViewModel _selectedGroup;
        public GroupViewModel SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {
                _selectedGroup = value;
                OnPropertyChanged("SelectedGroup");
            }
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            var result = MessageBox.Show(string.Format("Вы уверенны, что хотите удалить группу \"{0}\" из списка", SelectedGroup.Group.Name),
                "Firesec", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                FiresecManager.SecurityConfiguration.UserGroups.Remove(SelectedGroup.Group);
                Groups.Remove(SelectedGroup);
                SecurityModule.HasChanges = true;
            }
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var groupDetailsViewModel = new GroupDetailsViewModel(SelectedGroup.Group);
            if (ServiceFactory.UserDialogs.ShowModalWindow(groupDetailsViewModel))
            {
                SelectedGroup.Group = groupDetailsViewModel.Group;
                SecurityModule.HasChanges = true;
            }
        }

        bool CanEditDelete()
        {
            return SelectedGroup != null;
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var groupDetailsViewModel = new GroupDetailsViewModel();
            if (ServiceFactory.UserDialogs.ShowModalWindow(groupDetailsViewModel))
            {
                FiresecManager.SecurityConfiguration.UserGroups.Add(groupDetailsViewModel.Group);
                Groups.Add(new GroupViewModel(groupDetailsViewModel.Group));
                SecurityModule.HasChanges = true;
            }
        }

        public override void OnShow()
        {
            ServiceFactory.Layout.ShowMenu(new UsersMenuViewModel(AddCommand, DeleteCommand, EditCommand));
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}