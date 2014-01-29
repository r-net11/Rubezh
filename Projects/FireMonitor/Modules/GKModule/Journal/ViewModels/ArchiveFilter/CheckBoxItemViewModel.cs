using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
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
				if (ItemsList != null)
					ItemsList.OnPropertyChanged(() => ItemsList.HasCheckedItems);
			}
		}
		public CheckBoxItemList ItemsList { get; set; }
	}
}