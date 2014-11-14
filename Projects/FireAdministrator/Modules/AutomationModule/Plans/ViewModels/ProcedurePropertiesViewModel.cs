using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;
using AutomationModule.ViewModels;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using FiresecAPI.Automation;
using Infrastructure.Designer.ElementProperties.ViewModels;
using Infrustructure.Plans.Elements;

namespace AutomationModule.Plans.ViewModels
{
	public class ProcedurePropertiesViewModel : TextBlockPropertiesViewModel
	{
		private ElementProcedure _element;

		public ProcedurePropertiesViewModel(ElementProcedure element, ProceduresViewModel proceduresViewModel)
			: base(element, (ElementBase) element)
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
			Guid procedureUID = _element.ProcedureUID;
			AutomationPlanExtension.Instance.SetItem<Procedure>(_element, SelectedProcedure == null ? null : SelectedProcedure.Procedure);
			UpdateProcedures(procedureUID);
			return base.Save();
		}
		private void UpdateProcedures(Guid procedureUID)
		{
			if (Procedures != null)
			{
				if (procedureUID != _element.ProcedureUID)
					Update(procedureUID);
				Update(_element.ProcedureUID);
			}
		}
		private void Update(Guid procedureUID)
		{
			var procedure = Procedures.FirstOrDefault(x => x.Procedure.Uid == procedureUID);
			if (procedure != null)
				procedure.Update();
		}
	}
}