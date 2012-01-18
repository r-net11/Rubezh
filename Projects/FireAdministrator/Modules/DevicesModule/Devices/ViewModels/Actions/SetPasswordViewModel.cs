using System;
using FiresecAPI.Models;
using Infrastructure.Common;
using Controls;

namespace DevicesModule.ViewModels
{
    public class SetPasswordViewModel : SaveCancelDialogContent
    {
        Guid _deviceUID;
        static bool _isUsb;

        public SetPasswordViewModel(Guid deviceUID, bool isUsb)
        {
            _deviceUID = deviceUID;
            _isUsb = isUsb;
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
            if (!DigitalPasswordHelper.Check(Password) || !DigitalPasswordHelper.Check(PasswordConfirm))
            {
                DialogBox.DialogBox.Show("Пароль может содержать только цифры");
                cancel = true;
                return;
            }

            if (Password != PasswordConfirm)
            {
                DialogBox.DialogBox.Show("Пароль и подтверждение пароля должны совпадать");
                cancel = true;
                return;
            }

            SetPasswordHelper.Run(_deviceUID, _isUsb, _devicePasswordType, _password);
        }
    }
}