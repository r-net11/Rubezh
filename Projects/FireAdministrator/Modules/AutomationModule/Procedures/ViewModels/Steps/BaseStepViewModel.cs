using System;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq.Expressions;
using Infrastructure;

namespace AutomationModule.ViewModels
{
	public abstract class BaseStepViewModel : BaseViewModel
	{
		public virtual void UpdateContent() { }
		public abstract string Description { get; }
		public Action UpdateDescriptionHandler;

		public BaseStepViewModel(Action updateDescriptionHandler)
		{
			UpdateDescriptionHandler = updateDescriptionHandler;
		}

		public new void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
		{
			ServiceFactory.SaveService.AutomationChanged = true;
			base.OnPropertyChanged(propertyExpression);
			if (UpdateDescriptionHandler != null)
				UpdateDescriptionHandler();
		}
	}
}