using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecClient.SKDHelpers;
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
			Positions = new List<SelectationPositionViewModel>();
			var positions = PositionHelper.GetByOrganisation(Employee.OrganisationUID);
			if (positions == null)
				return;
			foreach (var position in positions)
				Positions.Add(new SelectationPositionViewModel(position));
			if (Employee.Position != null)
			{
				SelectedPosition = Positions.FirstOrDefault(x => x.Position.UID == Employee.Position.UID);
				if (SelectedPosition == null)
					SelectedPosition = Positions.FirstOrDefault();
			}
			else
				SelectedPosition = Positions.FirstOrDefault();
			SelectedPosition.IsChecked = true;
		}

		public List<SelectationPositionViewModel> Positions { get; private set; }

		SelectationPositionViewModel _selectedPosition;
		public SelectationPositionViewModel SelectedPosition
		{
			get { return _selectedPosition; }
			set
			{
				_selectedPosition = value;
				OnPropertyChanged(() => SelectedPosition);
			}
		}

		protected override bool Save()
		{
			SelectedPosition = Positions.FirstOrDefault(x => x.IsChecked);
			return base.Save();
		}
	}
}
