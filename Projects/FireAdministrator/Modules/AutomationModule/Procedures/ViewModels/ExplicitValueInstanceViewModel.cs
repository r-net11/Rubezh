using Infrastructure.Common.Windows.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ExplicitValueInstanceViewModel<T> : BaseViewModel
	{
		T _value;
		public T Value
		{
			get { return _value; }
			set
			{
				_value = value;
				OnPropertyChanged(() => Value);
			}
		}

		public ExplicitValueInstanceViewModel(T value)
		{
			Value = value;
		}
	}
}
