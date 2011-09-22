using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace SecurityModule.ViewModels
{
    public class GroupDetailsViewModel : SaveCancelDialogContent
    {
        public UserGroup Group { get; private set; }

        public GroupDetailsViewModel()
        {
            Title = "Новая группа";
            Group = new UserGroup()
            {
                Id = (int.Parse(FiresecManager.SecurityConfiguration.UserGroups.Last().Id) + 1).ToString()
            };

            Initialize();
        }

        public GroupDetailsViewModel(UserGroup group)
        {
            Title = "Редактирование группы";
            Group = group;

            Initialize();
        }

        void Initialize()
        {
            CopyProperties();
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public ObservableCollection<PermissionViewModel> PermissionViewModels { get; set; }

        void CopyProperties()
        {
            Name = Group.Name;
        }

        void SaveProperties()
        {
            Group.Name = Name;
        }

        protected override void Save(ref bool cancel)
        {
            SaveProperties();
        }

        protected override bool CanSave()
        {
            return Group.Name != Name;
        }
    }
}