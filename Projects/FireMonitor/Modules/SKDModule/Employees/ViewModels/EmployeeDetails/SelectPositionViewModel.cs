using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SelectPositionViewModel : SaveCancelDialogViewModel
	{
		public Employee Employee { get; private set; }
		HRViewModel _hrViewModel;

		public SelectPositionViewModel(Employee employee, ShortPosition position, HRViewModel hrViewModel)
		{
			Title = "Должность";
			Employee = employee;
			_hrViewModel = hrViewModel;
			AddCommand = new RelayCommand(OnAdd);
			Positions = new ObservableCollection<SelectationPositionViewModel>();
			var positions = PositionHelper.GetByOrganisation(Employee.OrganisationUID);
			if (positions == null || positions.Count() == 0)
			{
				MessageBoxService.Show("Для данной организации не указано не одной должности");
				return;
			}
			foreach (var item in positions)
				Positions.Add(new SelectationPositionViewModel(item, this));
			SelectationPositionViewModel selectedPosition;
			if (position != null)
				selectedPosition = Positions.FirstOrDefault(x => x.Position.UID == position.UID);
			else
				selectedPosition = Positions.FirstOrDefault();
			selectedPosition.IsChecked = true;
		}

		public ObservableCollection<SelectationPositionViewModel> Positions { get; set; }

		public SelectationPositionViewModel SelectedPosition
		{
			get { return Positions.FirstOrDefault(x => x.IsChecked); }
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var positionDetailsViewModel = new PositionDetailsViewModel(Employee.OrganisationUID);
			if (DialogService.ShowModalWindow(positionDetailsViewModel))
			{
				var positionViewModel = new SelectationPositionViewModel(positionDetailsViewModel.ShortPosition, this);
				Positions.Add(positionViewModel);
				positionViewModel.SelectCommand.Execute();
				_hrViewModel.UpdatePositions();
			}
		}
	}
}
