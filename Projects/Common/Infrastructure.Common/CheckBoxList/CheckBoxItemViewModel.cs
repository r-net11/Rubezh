using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.CheckBoxList
{
	public abstract class CheckBoxItemViewModel : BaseViewModel, ICheckBoxItem
	{
		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
				Update(value);
			}
		}

		protected virtual void Update(bool value)
		{
			if (ItemsList != null)
				ItemsList.Update();
		}

		public ICheckBoxItemList ItemsList { get; set; }
	}
}