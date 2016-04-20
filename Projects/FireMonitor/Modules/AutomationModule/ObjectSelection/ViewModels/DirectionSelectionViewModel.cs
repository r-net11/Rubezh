using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;

namespace AutomationModule.ViewModels
{
	public class DirectionSelectionViewModel : SaveCancelDialogViewModel
	{
		public DirectionSelectionViewModel(GKDirection direction)
		{
			Title = "Выбор направления";
			Directions = new ObservableCollection<DirectionViewModel>();
			GKManager.Directions.ForEach(x => Directions.Add(new DirectionViewModel(x)));
			if (direction != null)
				SelectedDirection = Directions.FirstOrDefault(x => x.Direction.UID == direction.UID);
			if (SelectedDirection == null)
				SelectedDirection = Directions.FirstOrDefault();
		}

		public ObservableCollection<DirectionViewModel> Directions { get; private set; }

		DirectionViewModel _selectedDirection;
		public DirectionViewModel SelectedDirection
		{
			get { return _selectedDirection; }
			set
			{
				_selectedDirection = value;
				OnPropertyChanged(() => SelectedDirection);
			}
		}
	}
}