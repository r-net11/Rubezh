using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Infrastructure.Common.Windows.ViewModels
{
	public abstract class BaseViewModel : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged Members and helper

		private readonly NotifyPropertyChangedHelper _propertyChangeHelper = new NotifyPropertyChangedHelper();

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChangeHelper.Add(value); }
			remove { _propertyChangeHelper.Remove(value); }
		}

		protected void SetValue<T>(ref T field, T value, params string[] propertyNames)
		{
			_propertyChangeHelper.SetValue(this, ref field, value, propertyNames);
		}

		public void OnPropertyChanged(string propertyName)
		{
			_propertyChangeHelper.NotifyPropertyChanged(this, propertyName);
		}

		public void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
		{
			OnPropertyChanged(ExtractPropertyName(propertyExpression));
		}

		private static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
		{
			if (propertyExpression == null)
				throw new ArgumentNullException("propertyExpression");
			var memberExpression = propertyExpression.Body as MemberExpression;
			if (memberExpression == null)
				throw new ArgumentException("The expression is not a member access expression.", "propertyExpression");
			var property = memberExpression.Member as PropertyInfo;
			if (property == null)
				throw new ArgumentException("The member access expression does not access a property.", "propertyExpression");
			return property.Name;
		}

		#endregion INotifyPropertyChanged Members and helper

		public BaseViewModel()
		{

		}
	}
}