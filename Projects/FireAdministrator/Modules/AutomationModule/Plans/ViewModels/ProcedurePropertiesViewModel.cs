using AutomationModule.ViewModels;
using Infrastructure.Designer.ElementProperties.ViewModels;
using RubezhAPI.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutomationModule.Plans.ViewModels
{
	public class ProcedurePropertiesViewModel : TextBlockPropertiesViewModel
	{
		private ElementProcedure _element;

		public ProcedurePropertiesViewModel(ElementProcedure element, ProceduresViewModel proceduresViewModel)
			: base(element)
		{
			Procedures = proceduresViewModel.Procedures;
			_element = element;
			Title = "Свойства фигуры: Процедура";
			if (element.ProcedureUID != Guid.Empty)
				SelectedProcedure = Procedures.FirstOrDefault(x => x.Procedure.Uid == element.ProcedureUID);
		}

		public ObservableCollection<ProcedureViewModel> Procedures { get; private set; }

		private ProcedureViewModel _selectedProcedure;
		public ProcedureViewModel SelectedProcedure
		{
			get { return _selectedProcedure; }
			set
			{
				_selectedProcedure = value;
				OnPropertyChanged(() => SelectedProcedure);
			}
		}

		protected override bool Save()
		{
			AutomationPlanExtension.Instance.RewriteItem(_element, SelectedProcedure.Procedure);
			return base.Save();
		}
		protected override bool CanSave()
		{
			return SelectedProcedure != null;
		}
	}
}