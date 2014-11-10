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
using System.Drawing;
using Infrustructure.Plans.Elements;
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
				switch (automationCallbackResult.AutomationCallbackType)
				{
					case AutomationCallbackType.SetPlanProperty:
						var planArguments = (PlanCallbackData)automationCallbackResult.Data;
						var plan = Plans.FirstOrDefault(x => x.Plan.UID == planArguments.PlanUid);
						if (plan != null)
						{
							var elementBase = GetAllElements(plan.Plan).FirstOrDefault(x => x.UID == planArguments.ElementUid);
							if (elementBase is ElementBaseRectangle)
							{
								var element = elementBase as ElementBaseRectangle;
								if (element != null)
								{
									if (planArguments.ElementPropertyType == ElementPropertyType.Height)
										element.Height = Convert.ToDouble(planArguments.Value);
									if (planArguments.ElementPropertyType == ElementPropertyType.Width)
										element.Width = Convert.ToDouble(planArguments.Value);
									if (planArguments.ElementPropertyType == ElementPropertyType.Color)
									{
										var drawingColor = Color.FromName(planArguments.Value.ToString());
										element.BorderColor = System.Windows.Media.Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
									}
									if (planArguments.ElementPropertyType == ElementPropertyType.BackColor)
									{
										var drawingColor = Color.FromName(planArguments.Value.ToString());
										element.BackgroundColor = System.Windows.Media.Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
									}
									if (planArguments.ElementPropertyType == ElementPropertyType.BorderThickness)
										element.BorderThickness = Convert.ToDouble(planArguments.Value);
									if (planArguments.ElementPropertyType == ElementPropertyType.Left)
										element.Left = Convert.ToDouble(planArguments.Value);
									if (planArguments.ElementPropertyType == ElementPropertyType.Top)
										element.Top = Convert.ToDouble(planArguments.Value);
								}
							}
							if (elementBase is ElementBasePolygon)
							{
								var element = elementBase as ElementBasePolygon;
								if (element != null)
								{
									if (planArguments.ElementPropertyType == ElementPropertyType.Color)
									{
										var drawingColor = Color.FromName(planArguments.Value.ToString());
										element.BorderColor = System.Windows.Media.Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
									}
									if (planArguments.ElementPropertyType == ElementPropertyType.BackColor)
									{
										var drawingColor = Color.FromName(planArguments.Value.ToString());
										element.BackgroundColor = System.Windows.Media.Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
									}
									if (planArguments.ElementPropertyType == ElementPropertyType.BorderThickness)
										element.BorderThickness = Convert.ToDouble(planArguments.Value);
									if (planArguments.ElementPropertyType == ElementPropertyType.Left)
										element.Position = new System.Windows.Point(Convert.ToDouble(planArguments.Value), element.Position.Y);
									if (planArguments.ElementPropertyType == ElementPropertyType.Top)
										element.Position = new System.Windows.Point(element.Position.X, Convert.ToDouble(planArguments.Value));
								}
							}

							if (elementBase is ElementBasePolyline)
							{
								var element = elementBase as ElementBasePolyline;
								if (element != null)
								{
									if (planArguments.ElementPropertyType == ElementPropertyType.Color)
									{
										var drawingColor = Color.FromName(planArguments.Value.ToString());
										element.BorderColor = System.Windows.Media.Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
									}
									if (planArguments.ElementPropertyType == ElementPropertyType.BorderThickness)
										element.BorderThickness = Convert.ToDouble(planArguments.Value);
									if (planArguments.ElementPropertyType == ElementPropertyType.Left)
										element.Position = new System.Windows.Point(Convert.ToDouble(planArguments.Value), element.Position.Y);
									if (planArguments.ElementPropertyType == ElementPropertyType.Top)
										element.Position = new System.Windows.Point(element.Position.X, Convert.ToDouble(planArguments.Value));
								}
							}
						}
						SelectedPlanChanged(this, EventArgs.Empty);
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