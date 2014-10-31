using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecAPI.Models;
using FiresecClient;

namespace AutomationModule.ViewModels
{
	public class ControlPlanStepViewModel : BaseStepViewModel
	{
		public ArgumentViewModel ValueArgument { get; private set; }
		ControlPlanArguments ControlPlanArguments { get; set; }

		public ControlPlanStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlPlanArguments = stepViewModel.Step.ControlPlanArguments;
			ValueArgument = new ArgumentViewModel(ControlPlanArguments.ValueArgument, stepViewModel.Update, UpdateContent);
			ElementPropertyTypes = new ObservableCollection<ElementPropertyType>();
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
					foreach (var elementRectangle in _selectedPlan.Plan.ElementRectangles)
					{
						Elements.Add(new ElementViewModel(elementRectangle));
					}
					SelectedElement = Elements.FirstOrDefault(x => x.Uid == ControlPlanArguments.ElementUid);
					OnPropertyChanged(() => Elements);
				}
				OnPropertyChanged(() => SelectedPlan);
			}
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
			Elements = new ObservableCollection<ElementViewModel>();
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				Plans.Add(new PlanViewModel(plan));
			}
			SelectedPlan = Plans.FirstOrDefault(x => x.Plan.UID == ControlPlanArguments.PlanUid);
			OnPropertyChanged(() => Plans);
			ValueArgument.Update(Procedure, PropertyTypeToExplicitType(SelectedElementPropertyType), isList: false);
		}

		public override string Description
		{
			get { return ""; }
		}
	}
}