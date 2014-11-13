using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans;
using Infrastructure.Common.Windows;
using FiresecAPI.AutomationCallback;
using FiresecAPI.Automation;
using Infrustructure.Plans.Elements;
using System.Windows.Media;
namespace PlansModule.ViewModels
{
	public class PlanTreeViewModel : BaseViewModel
	{
		public static PlanTreeViewModel Current { get; private set; }
		public event EventHandler SelectedPlanChanged;
		public List<PlanViewModel> AllPlans { get; private set; }
		private PlansViewModel _plansViewModel;
		private List<Guid> _filter;

		public PlanTreeViewModel(PlansViewModel plansViewModel, List<Guid> filter)
		{
			Current = this;
			_filter = filter;
			_plansViewModel = plansViewModel;
		}

		public void Initialize()
		{
			SelectedPlan = null;
			AllPlans = new List<PlanViewModel>();
			Plans = new ObservableCollection<PlanViewModel>();
			foreach (var plan in FiresecManager.PlansConfiguration.Plans)
				AddPlan(plan, null);
			if (SelectedPlan != null)
				SelectedPlan.ExpandToThis();
			_plansViewModel.PlanPresenters.ForEach(planPresenter => AddPlanPresenter(planPresenter));
			SafeFiresecService.AutomationEvent -= OnAutomationCallback;
			SafeFiresecService.AutomationEvent += OnAutomationCallback;
		}
		private void AddPlan(Plan plan, PlanViewModel parentPlanViewModel)
		{
			if (_filter == null || _filter.Contains(plan.UID))
			{
				var planViewModel = new PlanViewModel(_plansViewModel, plan);
				planViewModel.IsExpanded = true;
				AllPlans.Add(planViewModel);
				if (parentPlanViewModel == null)
					Plans.Add(planViewModel);
				else
					parentPlanViewModel.AddChild(planViewModel);
				if (SelectedPlan == null && !planViewModel.IsFolder)
					SelectedPlan = planViewModel;

				foreach (var childPlan in plan.Children)
					AddPlan(childPlan, planViewModel);
			}
		}

		PlanViewModel _selectedPlan;
		public PlanViewModel SelectedPlan
		{
			get { return _selectedPlan; }
			set
			{
				if (SelectedPlan != value)
				{
					_selectedPlan = value;
					OnPropertyChanged(() => SelectedPlan);
					if (SelectedPlanChanged != null)
						SelectedPlanChanged(this, EventArgs.Empty);
				}
			}
		}

		ObservableCollection<PlanViewModel> _plans;
		public ObservableCollection<PlanViewModel> Plans
		{
			get { return _plans; }
			private set
			{
				_plans = value;
				OnPropertyChanged(() => Plans);
			}
		}

		public PlanViewModel FindPlan(Guid planUID)
		{
			return AllPlans.FirstOrDefault(item => item.Plan != null && item.Plan.UID == planUID);
		}
		public void AddPlanPresenter(IPlanPresenter<Plan, XStateClass> planPresenter)
		{
			AllPlans.ForEach(planViewModel => planViewModel.RegisterPresenter(planPresenter));
		}
		public void Select()
		{
			if (SelectedPlan == null && Plans.IsNotNullOrEmpty())
				SelectedPlan = Plans[0];
		}

		void OnAutomationCallback(AutomationCallbackResult automationCallbackResult)
		{
			ApplicationService.Invoke(() =>
			{
				if (automationCallbackResult.AutomationCallbackType != AutomationCallbackType.GetPlanProperty && automationCallbackResult.AutomationCallbackType != AutomationCallbackType.SetPlanProperty)
					return;
				var planArguments = (PlanCallbackData)automationCallbackResult.Data;
				var plan = Plans.FirstOrDefault(x => x.Plan.UID == planArguments.PlanUid);
				if (plan == null)
					return;
				var elementBase = GetAllElements(plan.Plan).FirstOrDefault(x => x.UID == planArguments.ElementUid);
				if (elementBase == null)
					return;
				switch (automationCallbackResult.AutomationCallbackType)
				{
					case AutomationCallbackType.SetPlanProperty:						
						if (elementBase is ElementBaseRectangle)
						{
							var element = elementBase as ElementBaseRectangle;
							if (element != null)
							{
								if (planArguments.ElementPropertyType == ElementPropertyType.Height)
									element.Height = Convert.ToDouble(planArguments.Value);
								if (planArguments.ElementPropertyType == ElementPropertyType.Width)
									element.Width = Convert.ToDouble(planArguments.Value);
							}
						}
						if (elementBase is ElementTextBlock)
						{
							var element = elementBase as ElementTextBlock;
							if (element != null)
							{
								if (planArguments.ElementPropertyType == ElementPropertyType.FontBold)
									element.FontBold = Convert.ToBoolean(planArguments.Value);
								if (planArguments.ElementPropertyType == ElementPropertyType.FontItalic)
									element.FontItalic = Convert.ToBoolean(planArguments.Value);
								if (planArguments.ElementPropertyType == ElementPropertyType.FontSize)
									element.FontSize = Convert.ToDouble(planArguments.Value);
								if (planArguments.ElementPropertyType == ElementPropertyType.ForegroundColor)
									element.ForegroundColor = (Color)ColorConverter.ConvertFromString(planArguments.Value.ToString());
								if (planArguments.ElementPropertyType == ElementPropertyType.Stretch)
									element.Stretch = Convert.ToBoolean(planArguments.Value);
								if (planArguments.ElementPropertyType == ElementPropertyType.Text)
									element.Text = Convert.ToString(planArguments.Value);
								if (planArguments.ElementPropertyType == ElementPropertyType.WordWrap)
									element.WordWrap = Convert.ToBoolean(planArguments.Value);
							}
						}
						if (planArguments.ElementPropertyType == ElementPropertyType.Color)
							elementBase.BorderColor = (Color)ColorConverter.ConvertFromString(planArguments.Value.ToString());
						if (planArguments.ElementPropertyType == ElementPropertyType.BackColor)
							elementBase.BackgroundColor = (Color)ColorConverter.ConvertFromString(planArguments.Value.ToString());
						if (planArguments.ElementPropertyType == ElementPropertyType.BorderThickness)
							elementBase.BorderThickness = Convert.ToDouble(planArguments.Value);
						if (planArguments.ElementPropertyType == ElementPropertyType.Left)
							elementBase.Position = new System.Windows.Point(Convert.ToDouble(planArguments.Value), elementBase.Position.Y);
						if (planArguments.ElementPropertyType == ElementPropertyType.Top)
							elementBase.Position = new System.Windows.Point(elementBase.Position.X, Convert.ToDouble(planArguments.Value));
						SelectedPlanChanged(this, EventArgs.Empty);
						break;

					case AutomationCallbackType.GetPlanProperty:
						var value = new object();
						if (elementBase is ElementBaseRectangle)
						{
							var element = elementBase as ElementBaseRectangle;
							if (element != null)
							{
								if (planArguments.ElementPropertyType == ElementPropertyType.Height)
									value = Convert.ToInt32(element.Height);
								if (planArguments.ElementPropertyType == ElementPropertyType.Width)
									value = Convert.ToInt32(element.Width);
							}
						}
						if (elementBase is ElementTextBlock)
						{
							var element = elementBase as ElementTextBlock;
							if (element != null)
							{
								if (planArguments.ElementPropertyType == ElementPropertyType.FontBold)
									value = element.FontBold;
								if (planArguments.ElementPropertyType == ElementPropertyType.FontItalic)
									value = element.FontItalic;
								if (planArguments.ElementPropertyType == ElementPropertyType.FontSize)
									value = Convert.ToInt32(element.FontSize);
								if (planArguments.ElementPropertyType == ElementPropertyType.ForegroundColor)
									value = element.ForegroundColor;
								if (planArguments.ElementPropertyType == ElementPropertyType.Stretch)
									value = element.Stretch;
								if (planArguments.ElementPropertyType == ElementPropertyType.Text)
									value = element.Text;
								if (planArguments.ElementPropertyType == ElementPropertyType.WordWrap)
									value = element.WordWrap;
							}
						}
						if (planArguments.ElementPropertyType == ElementPropertyType.Color)
							value = elementBase.BorderColor;
						if (planArguments.ElementPropertyType == ElementPropertyType.BackColor)
							value = elementBase.BackgroundColor;
						if (planArguments.ElementPropertyType == ElementPropertyType.BorderThickness)
							value = Convert.ToInt32(elementBase.BorderThickness);
						if (planArguments.ElementPropertyType == ElementPropertyType.Left)
							value = Convert.ToInt32(elementBase.Position.X);
						if (planArguments.ElementPropertyType == ElementPropertyType.Top)
							value = Convert.ToInt32(elementBase.Position.Y);
						FiresecManager.FiresecService.ProcedureCallbackResponse(automationCallbackResult.ProcedureUID, value);
						break;
				}
			});
		}

		public static ObservableCollection<ElementBase> GetAllElements(Plan plan)
		{
			var elements = new ObservableCollection<ElementBase>();
			var allElements = new List<ElementBase>(plan.ElementRectangles);
			allElements.AddRange(plan.ElementEllipses);
			allElements.AddRange(plan.ElementPolylines);
			allElements.AddRange(plan.ElementTextBlocks);
			allElements.AddRange(plan.ElementPolygons);
			foreach (var elementRectangle in allElements)
			{
				elements.Add(elementRectangle);
			}
			return elements;
		}
	}
}