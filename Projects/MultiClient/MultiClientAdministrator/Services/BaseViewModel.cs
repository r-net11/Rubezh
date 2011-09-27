using System.ComponentModel;
using System.Windows;
using Infrastructure.Common;

namespace MultiClient.Services
{
    public abstract class BaseViewModel :
            DependencyObject,
            INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members and helper

        readonly NotifyPropertyChangedHelper _propertyChangeHelper = new NotifyPropertyChangedHelper();

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChangeHelper.Add(value); }
            remove { _propertyChangeHelper.Remove(value); }
        }

        protected void SetValue<T>(ref T field, T value, params string[] propertyNames)
        {
            _propertyChangeHelper.SetValue(this, ref field, value, propertyNames);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            _propertyChangeHelper.NotifyPropertyChanged(this, propertyName);
        }

        #endregion
    }
}
