using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Infrastructure.Common.Windows.ViewModels
{
	public abstract class BaseObjectViewModel<TObject> : BaseViewModel
	{
		public BaseObjectViewModel(TObject model)
		{
			Model = model;
		}

		public TObject Model { get; private set; }

		public virtual void Update()
		{
			OnPropertyChanged(() => Model);
		}
	}
}