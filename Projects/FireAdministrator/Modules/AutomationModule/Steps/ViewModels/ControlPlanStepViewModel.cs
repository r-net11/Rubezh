using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.Automation;
using StrazhAPI.Models;
using FiresecClient;
using System.Collections.Generic;
using Infrustructure.Plans.Elements;
using Infrastructure.Common.Services;
using Infrustructure.Plans.Events;
using StrazhAPI;

namespace AutomationModule.ViewModels
{
	public class ControlPlanStepViewModel : BaseStepViewModel
	{
		public ArgumentViewModel ValueArgument { get; private set; }
		ControlPlanArguments ControlPlanArguments { get; set; }
		public ProcedureLayoutCollectionViewModel ProcedureLayoutCollectionViewModel { get; private set; }
		public ControlElementType ControlElementType { get; private set; }

		public ControlPlanStepViewModel(StepViewModel stepViewModel, ControlElementType controlElementType) : base(stepViewModel)
		{
			ControlPlanArguments = stepViewModel.Step.ControlPlanArguments;
			ControlElementType = controlElementType;
			ValueArgument = new ArgumentViewModel(ControlPlanArguments.ValueArgument, stepViewModel.Update, UpdateContent, controlElementType == ControlElementType.Set);
			ElementPropertyTypes = new ObservableCollection<ElementPropertyType>();
			ServiceFactoryBase.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementsChanged);
			ServiceFactoryBase.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementsChanged);
			ServiceFactoryBase.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementsChanged);
			ServiceFactoryBase.Events.GetEvent<PlansConfigurationChangedEvent>().Subscribe(o => UpdateContent());
		}

		private void OnElementsChanged(List<ElementBase> elements)
		{
			UpdateContent();
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
				ValueArgument.Update(Procedure, explicitTypeViewModel.ExplicitType, explicitTypeViewModel.EnumType);
				OnPropertyChanged(() => SelectedElementPropertyType);
			}
		}

		public bool ForAllClients
		{
			get { return ControlPlanArguments.ForAllClients; }
			set
			{
				ControlPlanArguments.ForAllClients = value;
				if (value == false)
					StoreOnServer = false;
				OnPropertyChanged(() => ForAllClients);
				OnPropertyChanged(() => CanStoreOnServer);
			}
		}

		public bool StoreOnServer
		{
			get { return ControlPlanArguments.StoreOnServer; }
			set
			{
				ControlPlanArguments.StoreOnServer = value;
				OnPropertyChanged(() => StoreOnServer);
			}
		}

		public bool CanStoreOnServer
		{
			get { return ControlElementType == ControlElementType.Set && ForAllClients; }
		}

		ObservableCollection<ElementPropertyType> GetElemetProperties(ElementViewModel element)
		{
			var elementPropertyTypes = new ObservableCollection<ElementPropertyType>();
			if (element.ElementType == typeof(ElementRectangle) || element.ElementType == typeof(ElementEllipse))
				elementPropertyTypes = new ObservableCollection<ElementPropertyType> { ElementPropertyType.Height, ElementPropertyType.Width,
					ElementPropertyType.Color, ElementPropertyType.BackColor, ElementPropertyType.BorderThickness, ElementPropertyType.Left, ElementPropertyType.Top };
			if (element.ElementType == typeof(ElementPolygon))
				elementPropertyTypes = new ObservableCollection<ElementPropertyType> { ElementPropertyType.Color, ElementPropertyType.BackColor, ElementPropertyType.BorderThickness, ElementPropertyType.Left, ElementPropertyType.Top };
			if (element.ElementType == typeof(ElementPolyline))
				elementPropertyTypes = new ObservableCollection<ElementPropertyType> { ElementPropertyType.Color, ElementPropertyType.BorderThickness, ElementPropertyType.Left, ElementPropertyType.Top };
			if (element.ElementType == typeof(ElementTextBlock))
				elementPropertyTypes = ProcedureHelper.GetEnumObs<ElementPropertyType>();
			return elementPropertyTypes;
		}

		ExplicitTypeViewModel PropertyTypeToExplicitType(ElementPropertyType elementPropertyType)
		{
			if (elementPropertyType == ElementPropertyType.Height || elementPropertyType == ElementPropertyType.Width || elementPropertyType == ElementPropertyType.BorderThickness ||
				elementPropertyType == ElementPropertyType.FontSize || elementPropertyType == ElementPropertyType.Left || elementPropertyType == ElementPropertyType.Top)
				return new ExplicitTypeViewModel(ExplicitType.Integer);
			if (elementPropertyType == ElementPropertyType.FontBold || elementPropertyType == ElementPropertyType.FontItalic || elementPropertyType == ElementPropertyType.Stretch ||
				elementPropertyType == ElementPropertyType.WordWrap)
				return new ExplicitTypeViewModel(ExplicitType.Boolean);
			if (elementPropertyType == ElementPropertyType.Color || elementPropertyType == ElementPropertyType.BackColor || elementPropertyType == ElementPropertyType.ForegroundColor)
				return new ExplicitTypeViewModel(EnumType.ColorType);
			if (elementPropertyType == ElementPropertyType.Text)
				return new ExplicitTypeViewModel(ExplicitType.String);
			return new ExplicitTypeViewModel(ExplicitType.Integer);
		}

		public override void UpdateContent()
		{
			Plans = new ObservableCollection<PlanViewModel>(FiresecManager.PlansConfiguration.AllPlans.Select(x => new PlanViewModel(x)));
			SelectedPlan = Plans.FirstOrDefault(x => x.Plan.UID == ControlPlanArguments.PlanUid);
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