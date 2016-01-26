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
				
				ItemsList.BeforeChecked();
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

		public void SetFromParent(bool value)
		{
			_isChecked = value;
			OnPropertyChanged(() => IsChecked);
		}

		public ICheckBoxItemList ItemsList { get; set; }
	}
}