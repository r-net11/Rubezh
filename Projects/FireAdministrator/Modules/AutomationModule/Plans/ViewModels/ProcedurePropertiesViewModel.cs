using AutomationModule.ViewModels;
using Infrastructure.Designer.ElementProperties.ViewModels;
using StrazhAPI.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutomationModule.Plans.ViewModels
{
	public class ProcedurePropertiesViewModel : TextBlockPropertiesViewModel
	{
		private readonly ElementProcedure _element;

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
			_element.ProcedureUID = SelectedProcedure == null ? Guid.Empty : SelectedProcedure.Procedure.Uid;
			AutomationPlanExtension.Instance.SetItem(_element, SelectedProcedure == null ? null : SelectedProcedure.Procedure);
			UpdateProcedures(_element.ProcedureUID);
			return base.Save();
		}
		private void UpdateProcedures(Guid procedureUID)
		{
			if (Procedures == null) return;

			if (procedureUID != _element.ProcedureUID)
				Update(procedureUID);
			Update(_element.ProcedureUID);
		}
		private void Update(Guid procedureUID)
		{
			var procedure = Procedures.FirstOrDefault(x => x.Procedure.Uid == procedureUID);
			if (procedure != null)
				procedure.Update();
		}
	}
}