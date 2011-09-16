﻿using System;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class SetPasswordViewModel : SaveCancelDialogContent
    {
        Guid _deviceUID;

        public SetPasswordViewModel(Guid deviceUID)
        {
            _deviceUID = deviceUID;
            Title = "Смена пароля";
            DevicePasswordType = DevicePasswordType.Administrator;
        }

        DevicePasswordType _devicePasswordType;
        public DevicePasswordType DevicePasswordType
        {
            get { return _devicePasswordType; }
            set
            {
                _devicePasswordType = value;
                OnPropertyChanged("DevicePasswordType");
            }
        }

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

            FiresecManager.SetPassword(_deviceUID, DevicePasswordType, Password);
        }
    }
}
