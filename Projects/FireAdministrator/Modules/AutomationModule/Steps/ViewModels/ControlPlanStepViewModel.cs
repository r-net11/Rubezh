using Infrastructure.Automation;
using Infrastructure.Common.Services;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.Models;
using RubezhClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class ControlPlanStepViewModel : BaseStepViewModel
	{
		public ArgumentViewModel ValueArgument { get; private set; }
		ControlPlanArguments ControlPlanArguments { get; set; }
		public ProcedureLayoutCollectionViewModel ProcedureLayoutCollectionViewModel { get; private set; }
		public ControlElementType ControlElementType { get; private set; }

		public ControlPlanStepViewModel(StepViewModel stepViewModel, ControlElementType controlElementType)
			: base(stepViewModel)
		{
			ControlPlanArguments = stepViewModel.Step.ControlPlanArguments;
			ControlElementType = controlElementType;
			ValueArgument = new ArgumentViewModel(ControlPlanArguments.ValueArgument, stepViewModel.Update, UpdateContent, controlElementType == ControlElementType.Set);
			IsServerContext = Procedure.ContextType == ContextType.Server;
			ElementPropertyTypes = new ObservableCollection<ElementPropertyType>();
			ServiceFactoryBase.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementsChanged);
			ServiceFactoryBase.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementsChanged);
			ServiceFactoryBase.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementsChanged);
			ServiceFactoryBase.Events.GetEvent<PlansConfigurationChangedEvent>().Subscribe((o) => UpdateContent());
		}

		private void OnElementsChanged(List<ElementBase> elements)
		{
			UpdateContent();
		}

		bool _isServerContext;
		public bool IsServerContext
		{
			get { return _isServerContext; }
			set
			{
				_isServerContext = value;
				OnPropertyChanged(() => IsServerContext);
			}
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
					Elements = ProcedureHelper.GetAllElements(_selectedPlan.Plan);
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
					OnPropertyChanged(() => ElementPropertyTypes);
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
				var explicitTypeViewModel = PropertyTypeToExplicitType(SelectedElementPropertyType);
				ValueArgument.Update(Procedure, explicitTypeViewModel.ExplicitType, explicitTypeViewModel.EnumType, isList: false);
				OnPropertyChanged(() => SelectedElementPropertyType);
			}
		}

		public bool ForAllClients
		{
			get { return ControlPlanArguments.ForAllClients; }
			set
			{
				ControlPlanArguments.ForAllClients = value;
				OnPropertyChanged(() => ForAllClients);
			}
		}

		ObservableCollection<ElementPropertyType> GetElemetProperties(ElementViewModel element)
		{
			if (element.ElementType == typeof(ElementRectangle) || element.ElementType == typeof(ElementEllipse) || element.ElementType == typeof(ElementProcedure))
				return new ObservableCollection<ElementPropertyType> 
				{ 
					ElementPropertyType.IsVisible, 
					ElementPropertyType.IsEnabled,
					ElementPropertyType.Height, 
					ElementPropertyType.Width,
					ElementPropertyType.Color, 
					ElementPropertyType.BackColor, 
					ElementPropertyType.BorderThickness, 
					ElementPropertyType.Left, 
					ElementPropertyType.Top 
				};
			if (element.ElementType == typeof(ElementPolygon))
				return new ObservableCollection<ElementPropertyType> 
				{ 
					ElementPropertyType.IsVisible, 
					ElementPropertyType.IsEnabled, 
					ElementPropertyType.Color, 
					ElementPropertyType.BackColor, 
					ElementPropertyType.BorderThickness, 
					ElementPropertyType.Left, 
					ElementPropertyType.Top 
				};
			if (element.ElementType == typeof(ElementPolyline))
				return new ObservableCollection<ElementPropertyType> 
				{ 
					ElementPropertyType.IsVisible, 
					ElementPropertyType.IsEnabled, 
					ElementPropertyType.Color, 
					ElementPropertyType.BorderThickness, 
					ElementPropertyType.Left, 
					ElementPropertyType.Top 
				};
			if (element.ElementType == typeof(ElementGKDevice))
				return new ObservableCollection<ElementPropertyType> 
				{ 
					ElementPropertyType.IsVisible, 
					ElementPropertyType.IsEnabled,
					ElementPropertyType.Left, 
					ElementPropertyType.Top 
				};
			if (element.ElementType == typeof(ElementRectangleGKZone)
				|| element.ElementType == typeof(ElementRectangleGKGuardZone)
				|| element.ElementType == typeof(ElementRectangleGKSKDZone)
				|| element.ElementType == typeof(ElementRectangleGKDirection)
				|| element.ElementType == typeof(ElementRectangleGKMPT)
				|| element.ElementType == typeof(ElementRectangleGKDelay)
				|| element.ElementType == typeof(ElementRectangleGKPumpStation)
				|| element.ElementType == typeof(ElementPolygonGKZone)
				|| element.ElementType == typeof(ElementPolygonGKGuardZone)
				|| element.ElementType == typeof(ElementPolygonGKSKDZone)
				|| element.ElementType == typeof(ElementPolygonGKDirection)
				|| element.ElementType == typeof(ElementPolygonGKMPT)
				|| element.ElementType == typeof(ElementPolygonGKDelay)
				|| element.ElementType == typeof(ElementPolygonGKPumpStation)
				|| element.ElementType == typeof(ElementRectangleSubPlan)
				|| element.ElementType == typeof(ElementPolygonSubPlan))
				return new ObservableCollection<ElementPropertyType> 
				{ 
					ElementPropertyType.IsVisible, 
					ElementPropertyType.IsEnabled,
					ElementPropertyType.Height, 
					ElementPropertyType.Width,
					ElementPropertyType.Left, 
					ElementPropertyType.Top 
				};
			if (element.ElementType == typeof(ElementTextBlock))
				return AutomationHelper.GetEnumObs<ElementPropertyType>();
			return new ObservableCollection<ElementPropertyType>();
		}

		ExplicitTypeViewModel PropertyTypeToExplicitType(ElementPropertyType elementPropertyType)
		{
			if (elementPropertyType == ElementPropertyType.Height || elementPropertyType == ElementPropertyType.Width || elementPropertyType == ElementPropertyType.BorderThickness ||
				elementPropertyType == ElementPropertyType.FontSize || elementPropertyType == ElementPropertyType.Left || elementPropertyType == ElementPropertyType.Top)
				return new ExplicitTypeViewModel(ExplicitType.Integer);
			if (elementPropertyType == ElementPropertyType.FontBold || elementPropertyType == ElementPropertyType.FontItalic || elementPropertyType == ElementPropertyType.Stretch ||
				elementPropertyType == ElementPropertyType.WordWrap || elementPropertyType == ElementPropertyType.IsVisible || elementPropertyType == ElementPropertyType.IsEnabled)
				return new ExplicitTypeViewModel(ExplicitType.Boolean);
			if (elementPropertyType == ElementPropertyType.Color || elementPropertyType == ElementPropertyType.BackColor || elementPropertyType == ElementPropertyType.ForegroundColor)
				return new ExplicitTypeViewModel(EnumType.ColorType);
			if (elementPropertyType == ElementPropertyType.Text)
				return new ExplicitTypeViewModel(ExplicitType.String);
			return new ExplicitTypeViewModel(ExplicitType.Integer);
		}

		public override void UpdateContent()
		{
			Plans = new ObservableCollection<PlanViewModel>(ClientManager.PlansConfiguration.AllPlans.Select(x => new PlanViewModel(x)));
			SelectedPlan = Plans.FirstOrDefault(x => x.Plan.UID == ControlPlanArguments.PlanUid);
			IsServerContext = Procedure.ContextType == ContextType.Server;
			OnPropertyChanged(() => Plans);
			ProcedureLayoutCollectionViewModel = new ProcedureLayoutCollectionViewModel(ControlPlanArguments.LayoutFilter);
			OnPropertyChanged(() => ProcedureLayoutCollectionViewModel);
		}

		public override string Description
		{
			get
			{
				return "План: " + (SelectedPlan != null ? SelectedPlan.Caption : "<пусто>") + "; Элемент: " + (SelectedElement != null ? SelectedElement.PresentationName : "<пусто>") +
					"; Свойство: " + SelectedElementPropertyType.ToDescription() + "; Операция: " + ControlElementType.ToDescription() + "; Значение: " + ValueArgument.Description;
			}
		}
	}
}