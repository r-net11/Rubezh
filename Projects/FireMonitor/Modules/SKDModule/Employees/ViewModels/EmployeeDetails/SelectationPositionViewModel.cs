using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SelectationPositionViewModel : BaseViewModel
	{
		public ShortPosition Position { get; private set; }
		SelectPositionViewModel SelectPositionViewModel;

		public SelectationPositionViewModel(ShortPosition position, SelectPositionViewModel selectPositionViewModel)
		{
			Position = position;
			SelectPositionViewModel = selectPositionViewModel;
			SelectCommand = new RelayCommand(OnAdd);
		}

		public string Name { get { return Position.Name; } }
		public string Description { get { return Position.Description; } }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}

		public RelayCommand SelectCommand { get; private set; }
		void OnAdd()
		{
			var position = SelectPositionViewModel.Positions.FirstOrDefault(x => x.IsChecked);
			if (position != null)
				position.IsChecked = false;
			IsChecked = true;
		}
	}
}