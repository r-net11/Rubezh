using System;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq.Expressions;
using Infrastructure;
using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public abstract class BaseStepViewModel : BaseViewModel
	{
		public virtual void UpdateContent() { }
		public abstract string Description { get; }
		public Action UpdateDescriptionHandler;
		protected Procedure Procedure { get; private set; }

		public BaseStepViewModel(StepViewModel stepViewModel)
		{
			UpdateDescriptionHandler = stepViewModel.Update;
			Procedure = stepViewModel.Procedure;
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