using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecAPI.Models;
using FiresecClient;
using System.Collections.Generic;
using Infrustructure.Plans.Elements;

namespace AutomationModule.ViewModels
{
	public class ControlPlanStepViewModel : BaseStepViewModel
	{
		public ArgumentViewModel ValueArgument { get; private set; }
		ControlPlanArguments ControlPlanArguments { get; set; }
		public ProcedureLayoutCollectionViewModel ProcedureLayoutCollectionViewModel { get; private set; }

		public ControlPlanStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlPlanArguments = stepViewModel.Step.ControlPlanArguments;
			ValueArgument = new ArgumentViewModel(ControlPlanArguments.ValueArgument, stepViewModel.Update, UpdateContent);
			ElementPropertyTypes = new ObservableCollection<ElementPropertyType>();
			ControlVisualTypes = ProcedureHelper.GetEnumObs<ControlVisualType>();
		}

		public ObservableCollection<PlanViewModel> Plans { get; private set; }
		PlanViewModel _selectedPlan;
		public PlanViewModel SelectedPlan
		{
			get { return _selectedPlan; }
			set
			{
				_selectedPlan = value;
				if (_selectedPlan != null)
				{
					ControlPlanArguments.PlanUid = _selectedPlan.Plan.UID;
					Elements = GetAllElements(_selectedPlan.Plan);
					SelectedElement = Elements.FirstOrDefault(x => x.Uid == ControlPlanArguments.ElementUid);
					OnPropertyChanged(() => Elements);
				}
				OnPropertyChanged(() => SelectedPlan);
			}
		}

		public ObservableCollection<ElementViewModel> GetAllElements(Plan plan)
		{
			var elements = new ObservableCollection<ElementViewModel>();
			var allElements = new List<ElementBase>(plan.ElementUnion);
			allElements.AddRange(plan.ElementEllipses);
			allElements.AddRange(plan.ElementPolylines);
			allElements.AddRange(plan.ElementRectangles);
			allElements.AddRange(plan.ElementTextBlocks);
			allElements.AddRange(plan.ElementPolygons);
			foreach (var elementRectangle in allElements)
			{
				elements.Add(new ElementViewModel(elementRectangle));
			}
			return elements;
		}

		public ObservableCollection<ElementViewModel> Elements { get; private set; }
		ElementViewModel _selectedElement;
		public ElementViewModel SelectedElement
		{
			get { return _selectedElement; }
			set
			{
				_selectedElement = value;
				if (_selectedElement != null)
				{
					ControlPlanArguments.ElementUid = _selectedElement.Uid;
					ElementPropertyTypes = GetElemetProperties(_selectedElement);
					SelectedElementPropertyType = ElementPropertyTypes.FirstOrDefault(x => x == ControlPlanArguments.ElementPropertyType);
					OnPropertyChanged(()=>ElementPropertyTypes);
				}
				OnPropertyChanged(() => SelectedElement);
			}
		}

		public ObservableCollection<ElementPropertyType> ElementPropertyTypes { get; private set; }
		ElementPropertyType _selectedElementPropertyType;
		public ElementPropertyType SelectedElementPropertyType
		{
			get { return _selectedElementPropertyType; }
			set
			{
				_selectedElementPropertyType = value;
				ControlPlanArguments.ElementPropertyType = _selectedElementPropertyType;
				OnPropertyChanged(() => SelectedElementPropertyType);
			}
		}

		public ObservableCollection<ControlVisualType> ControlVisualTypes { get; private set; }
		public ControlVisualType SelectedControlVisualType
		{
			get { return ControlPlanArguments.ControlVisualType; }
			set
			{
				ControlPlanArguments.ControlVisualType = value;
				OnPropertyChanged(() => SelectedControlVisualType);
			}
		}

		ObservableCollection<ElementPropertyType> GetElemetProperties(ElementViewModel element)
		{
			var elementPropertyTypes = new ObservableCollection<ElementPropertyType>();
			if (element.ElementType == typeof(ElementRectangle))
				elementPropertyTypes =  new ObservableCollection<ElementPropertyType>{ElementPropertyType.Height, ElementPropertyType.Width};
			return elementPropertyTypes;
		}

		ExplicitType PropertyTypeToExplicitType(ElementPropertyType elementPropertyType)
		{
			if (elementPropertyType == ElementPropertyType.Height || elementPropertyType == ElementPropertyType.Width)
				return ExplicitType.Integer;
			return ExplicitType.Integer;
		}

		public override void UpdateContent()
		{
			Plans = new ObservableCollection<PlanViewModel>();
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				Plans.Add(new PlanViewModel(plan));
			}
			SelectedPlan = Plans.FirstOrDefault(x => x.Plan.UID == ControlPlanArguments.PlanUid);
			OnPropertyChanged(() => Plans);
			ValueArgument.Update(Procedure, PropertyTypeToExplicitType(SelectedElementPropertyType), isList: false);
			ProcedureLayoutCollectionViewModel = new ProcedureLayoutCollectionViewModel(ControlPlanArguments.LayoutFilter);
			OnPropertyChanged(() => ProcedureLayoutCollectionViewModel);
		}

		public override string Description
		{
			get { return ""; }
		}
	}
}