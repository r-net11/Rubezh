using FiresecAPI.Models;
using Infrastructure.Common;

namespace SecurityModule.ViewModels
{
    public class UserDetailsViewModel : SaveCancelDialogContent
    {
        public User User { get; private set; }
        bool _isNew;

        public UserDetailsViewModel()
        {
        }

        public void Initialize(User user = null)
        {
            if (user == null)
            {
                _isNew = true;
                Title = "Новый пользователь";
                User = new User();
            }
            else
            {
                _isNew = false;
                Title = "Редактирование пользователя";
                User = user;
            }

            CopyProperties();
        }

        void CopyProperties()
        {
            Name = User.Name;
        }

        void SaveProperties()
        {
            User.Name = Name;
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