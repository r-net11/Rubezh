using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace SecurityModule.ViewModels
{
    public class GroupsViewModel : RegionViewModel
    {
        public GroupsViewModel()
        {
            DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
            EditCommand = new RelayCommand(OnEdit, CanEditDelete);
            AddCommand = new RelayCommand(OnAdd);
        }

        public void Initialize()
        {
            Groups = new ObservableCollection<GroupViewModel>();
            if (FiresecManager.SecurityConfiguration.UserGroups != null)
                foreach (var group in FiresecManager.SecurityConfiguration.UserGroups)
                {
                    Groups.Add(new GroupViewModel(group));
                }
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

        bool CanEditDelete()
        {
            return SelectedGroup != null;
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            FiresecManager.SecurityConfiguration.UserGroups.Remove(SelectedGroup.Group);
            Groups.Remove(SelectedGroup);
            SecurityModule.HasChanges = true;
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            var groupDetailsViewModel = new GroupDetailsViewModel();
            groupDetailsViewModel.Initialize(SelectedGroup.Group);
            var result = ServiceFactory.UserDialogs.ShowModalWindow(groupDetailsViewModel);
            if (result)
            {
                SelectedGroup.Group = groupDetailsViewModel.Group;
                SecurityModule.HasChanges = true;
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var groupDetailsViewModel = new GroupDetailsViewModel();
            groupDetailsViewModel.Initialize();
            var result = ServiceFactory.UserDialogs.ShowModalWindow(groupDetailsViewModel);
            if (result)
            {
                FiresecManager.SecurityConfiguration.UserGroups.Add(groupDetailsViewModel.Group);
                var groupViewModel = new GroupViewModel(groupDetailsViewModel.Group);
                Groups.Add(groupViewModel);
                SecurityModule.HasChanges = true;
            }
        }

        public override void OnShow()
        {
            var usersMenuViewModel = new UsersMenuViewModel(AddCommand, DeleteCommand, EditCommand);
            ServiceFactory.Layout.ShowMenu(usersMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}
