using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LayoutModel = RubezhAPI.Models.Layouts.Layout;

namespace AutomationModule.ViewModels
{
	public class ProcedureLayoutCollectionViewModel : BaseViewModel
	{
		static LayoutModel _noLayout = new LayoutModel { Caption = "<Без макетов>", UID = Guid.Empty };
		public static LayoutModel NoLayout { get { return _noLayout; } }
		public List<Guid> ProcedureLayoutCollection { get; private set; }

		public ProcedureLayoutCollectionViewModel(List<Guid> procedureLayoutCollection)
		{
			ProcedureLayoutCollection = procedureLayoutCollection;
			Layouts = new ObservableCollection<LayoutViewModel>();
			if (procedureLayoutCollection.Contains(Guid.Empty))
				Layouts.Add(new LayoutViewModel(NoLayout));
			foreach (var layoutUID in procedureLayoutCollection)
			{
				var layout = ClientManager.LayoutsConfiguration.Layouts.FirstOrDefault(x => x.UID == layoutUID);
				if (layout != null)
					Layouts.Add(new LayoutViewModel(layout));
			}

			EditCommand = new RelayCommand(OnEdit);
		}

		ObservableCollection<LayoutViewModel> _layouts;
		public ObservableCollection<LayoutViewModel> Layouts
		{
			get { return _layouts; }
			set
			{
				_layouts = value;
				OnPropertyChanged(() => Layouts);
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var procedureLayoutsSelectionViewModel = new ProcedureLayoutsSelectionViewModel(ProcedureLayoutCollection);
			if (DialogService.ShowModalWindow(procedureLayoutsSelectionViewModel))
			{
				Layouts = new ObservableCollection<LayoutViewModel>(procedureLayoutsSelectionViewModel.LayoutItems.Where(x => x.IsChecked).Select(x => new LayoutViewModel(x.Layout)));
				ProcedureLayoutCollection.Clear();
				ProcedureLayoutCollection.AddRange(Layouts.Select(x => x.Layout.UID));
			}
		}
	}
}