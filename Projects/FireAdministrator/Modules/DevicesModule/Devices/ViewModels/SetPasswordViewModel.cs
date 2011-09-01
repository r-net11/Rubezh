using Infrastructure.Common;
using System.Linq;
using FiresecClient;
using System.Windows;
using System.Collections.Generic;
using System;

namespace DevicesModule.ViewModels
{
    public class SetPasswordViewModel : SaveCancelDialogContent
    {
        Guid _deviceUID;

        public SetPasswordViewModel(Guid deviceUID)
        {
            _deviceUID = deviceUID;
            Title = "Смена пароля";

            Users = new List<UserPassword>();
            Users.Add(new UserPassword("Инсталлятора") { IsSelected = true });
            Users.Add(new UserPassword("Администратора"));
            Users.Add(new UserPassword("Дежурного"));
        }

        public List<UserPassword> Users { get; private set; }

        string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }

        string _passwordConfirm;
        public string PasswordConfirm
        {
            get { return _passwordConfirm; }
            set
            {
                _passwordConfirm = value;
                OnPropertyChanged("PasswordConfirm");
            }
        }

        protected override void Save(ref bool cancel)
        {
            if (Password != PasswordConfirm)
            {
                MessageBox.Show("Пароль не был корректно подтвержден. Пароль и подтверждение должны совпадать");
                return;
            }

            var userPassword = Users.FirstOrDefault(x => x.IsSelected == true);

            FiresecManager.SetPassword(_deviceUID);
        }
    }

    public class UserPassword
    {
        public UserPassword(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public bool  IsSelected { get; set; }
    }
}
