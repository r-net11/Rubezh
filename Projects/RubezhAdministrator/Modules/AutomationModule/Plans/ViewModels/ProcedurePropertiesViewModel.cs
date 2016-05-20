using AutomationModule.ViewModels;
using Infrastructure.Plans.ElementProperties.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutomationModule.Plans.ViewModels
{
	public class ProcedurePropertiesViewModel : TextBlockPropertiesViewModel
	{
		const int _sensivityFactor = 100;
		private ElementProcedure _element;
		ElementBaseRectangle ElementBaseRectangle { get; set; }

		public ProcedurePropertiesViewModel(ElementProcedure element, ProceduresViewModel proceduresViewModel)
			: base(element)
		{
			Procedures = proceduresViewModel.Procedures;
			_element = element;
			ElementBaseRectangle = element as ElementBaseRectangle;
			Left = (int)(ElementBaseRectangle.Left * _sensivityFactor);
			Top = (int)(ElementBaseRectangle.Top * _sensivityFactor);
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
			ElementBaseRectangle.Left = (double)Left / _sensivityFactor;
			ElementBaseRectangle.Top = (double)Top / _sensivityFactor;
			AutomationPlanExtension.Instance.RewriteItem(_element, SelectedProcedure.Procedure);
			return base.Save();
		}
		protected override bool CanSave()
		{
			return SelectedProcedure != null;
		}
	}
}