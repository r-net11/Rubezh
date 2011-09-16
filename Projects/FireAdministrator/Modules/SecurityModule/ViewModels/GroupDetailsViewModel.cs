using FiresecAPI.Models;
using Infrastructure.Common;

namespace SecurityModule.ViewModels
{
    public class GroupDetailsViewModel : SaveCancelDialogContent
    {
        public UserGroup Group { get; private set; }
        bool _isNew;

        public GroupDetailsViewModel()
        {
        }

        public void Initialize(UserGroup group = null)
        {
            if (group == null)
            {
                _isNew = true;
                Title = "Новая группа";
                Group = new UserGroup();
            }
            else
            {
                _isNew = false;
                Title = "Редактирование группы";
                Group = group;
            }

            CopyProperties();
        }

        void CopyProperties()
        {
            Name = Group.Name;
        }

        void SaveProperties()
        {
            Group.Name = Name;
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

        protected override void Save(ref bool cancel)
        {
            SaveProperties();
        }
    }
}