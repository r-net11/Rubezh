using System.Collections.ObjectModel;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class PlanViewModel : BaseViewModel
	{
		public Plan Plan { get; private set; }
		public PlanViewModel(Plan plan)
		{
			Plan = plan;
		}

		public string Caption
		{
			get { return Plan.Caption; }
		}

		public ObservableCollection<ElementViewModel> Elements { get; private set; }
		ElementViewModel _selectedElement;
		public ElementViewModel SelectedElement
		{
			get { return _selectedElement; }
			set
			{
				_selectedElement = value;
				OnPropertyChanged(() => SelectedElement);
			}
		}
	}
}
