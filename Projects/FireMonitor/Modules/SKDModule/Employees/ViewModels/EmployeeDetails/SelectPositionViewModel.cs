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

		public SelectPositionViewModel(Employee employee)
		{
			Title = "Должность";
			Employee = employee;
			Positions = new ObservableCollection<SelectationPositionViewModel>();
			var positions = PositionHelper.GetByOrganisation(Employee.OrganisationUID);
			if (positions == null)
				return;
			foreach (var position in positions)
				Positions.Add(new SelectationPositionViewModel(position, this));
			SelectationPositionViewModel selectedPosition;
			if (Employee.Position != null)
			{
				selectedPosition = Positions.FirstOrDefault(x => x.Position.UID == Employee.Position.UID);
				if (selectedPosition == null)
					selectedPosition = Positions.FirstOrDefault();
			}
			else
				selectedPosition = Positions.FirstOrDefault();
			selectedPosition.IsChecked = true;
			AddCommand = new RelayCommand(OnAdd);
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
			}
		}
	}
}
