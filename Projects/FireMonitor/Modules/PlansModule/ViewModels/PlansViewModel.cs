using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Common;
using StrazhAPI.Automation;
using StrazhAPI.AutomationCallback;
using StrazhAPI.GK;
using StrazhAPI.Models;
using StrazhAPI.Models.Layouts;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;

namespace PlansModule.ViewModels
{
	public class PlansViewModel : ViewPartViewModel
	{
		private bool _initialized;
		private LayoutPartPlansProperties _properties;
		public List<IPlanPresenter<Plan, XStateClass>> PlanPresenters { get; private set; }
		public PlanTreeViewModel PlanTreeViewModel { get; private set; }
		public PlanDesignerViewModel PlanDesignerViewModel { get; private set; }

		public PlansViewModel(List<IPlanPresenter<Plan, XStateClass>> planPresenters)
			: this(planPresenters, new LayoutPartPlansProperties() { Type = LayoutPartPlansType.All })
		{
		}
		public PlansViewModel(List<IPlanPresenter<Plan, XStateClass>> planPresenters, LayoutPartPlansProperties properties)
		{
			_properties = properties ?? new LayoutPartPlansProperties() { Type = LayoutPartPlansType.All };
			PlanPresenters = planPresenters;
			ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Subscribe(OnNavigate);
			ServiceFactory.Events.GetEvent<ShowElementEvent>().Subscribe(OnShowElement);
			ServiceFactory.Events.GetEvent<FindElementEvent>().Subscribe(OnFindElementEvent);
			ServiceFactory.Events.GetEvent<SelectPlanEvent>().Subscribe(OnSelectPlan);
			ServiceFactory.Events.GetEvent<ChangePlanPropertiesEvent>().Unsubscribe(OnChangePlanProperties);
			ServiceFactory.Events.GetEvent<ChangePlanPropertiesEvent>().Subscribe(OnChangePlanProperties);
			
			_initialized = false;
			if (_properties.Type != LayoutPartPlansType.Single)
			{
				PlanTreeViewModel = new PlanTreeViewModel(this, _properties.Type == LayoutPartPlansType.Selected ? _properties.Plans : null);
				PlanTreeViewModel.SelectedPlanChanged += SelectedPlanChanged;
				var planNavigationWidth = RegistrySettingsHelper.GetDouble("Monitor.Plans.SplitterDistance");
				if (planNavigationWidth == 0)
					planNavigationWidth = 100;
				PlanNavigationWidth = new GridLength(planNavigationWidth, GridUnitType.Pixel);
				ApplicationService.ShuttingDown += () =>
				{
					RegistrySettingsHelper.SetDouble("Monitor.Plans.SplitterDistance", PlanNavigationWidth.Value);
				};
			}
			else
				PlanNavigationWidth = GridLength.Auto;
			PlanDesignerViewModel = new PlanDesignerViewModel(this);
		}

		public void Initialize()
		{
			_initialized = false;
			if (PlanTreeViewModel != null)
				PlanTreeViewModel.Initialize();
			_initialized = true;
			OnSelectedPlanChanged();
			SafeFiresecService.AutomationEvent -= OnAutomationCallback;
			SafeFiresecService.AutomationEvent += OnAutomationCallback;
		}

		private void OnSelectPlan(Guid planUID)
		{
			if (PlanTreeViewModel != null)
			{
				var newPlan = PlanTreeViewModel.FindPlan(planUID);
				if (PlanTreeViewModel.SelectedPlan == newPlan)
					PlanDesignerViewModel.Update();
				else
					PlanTreeViewModel.SelectedPlan = newPlan;
			}
		}
		private void SelectedPlanChanged(object sender, EventArgs e)
		{
			OnSelectedPlanChanged();
		}
		private void OnSelectedPlanChanged()
		{
			if (_initialized)
			{
				if (PlanTreeViewModel != null)
					PlanDesignerViewModel.SelectPlan(PlanTreeViewModel.SelectedPlan);
				else if (_properties != null && _properties.Plans.Count > 0)
				{
					var plan = FiresecManager.PlansConfiguration.AllPlans.FirstOrDefault(item => item.UID == _properties.Plans[0]);
					if (plan != null)
					{
						var planViewModel = new PlanViewModel(this, plan);
						PlanPresenters.ForEach(planPresenter => planViewModel.RegisterPresenter(planPresenter));
						PlanDesignerViewModel.SelectPlan(planViewModel);
					}
				}
			}
		}

		private void OnShowElement(Guid elementUID)
		{
			foreach (var presenterItem in PlanDesignerViewModel.PresenterItems)
				if (presenterItem.Element.UID == elementUID)
				{
					presenterItem.Navigate();
					PlanDesignerViewModel.Navigate(presenterItem);
				}
		}
		private void OnFindElementEvent(List<Guid> deviceUIDs)
		{
			if (PlanTreeViewModel != null)
			{
				foreach (var plan in PlanTreeViewModel.AllPlans)
					if (plan.PlanFolder == null && FindElementOnPlan(plan, deviceUIDs))
						return;
			}
			else
				FindElementOnPlan(PlanDesignerViewModel.PlanViewModel, deviceUIDs);
		}
		private bool FindElementOnPlan(PlanViewModel plan, List<Guid> deviceUIDs)
		{
			foreach (var element in plan.Plan.ElementUnion)
				if (deviceUIDs.Contains(element.UID))
				{
					PlanTreeViewModel.SelectedPlan = plan;
					OnShowElement(element.UID);
					return true;
				}
			return false;
		}
		private void OnNavigate(NavigateToPlanElementEventArgs args)
		{
			//Debug.WriteLine("[{0}]Navigation: PlanUID={1}\t\tElementUID={2}", DateTime.Now, args.PlanUID, args.ElementUID);
			ServiceFactory.Events.GetEvent<ShowPlansEvent>().Publish(null);
			OnSelectPlan(args.PlanUID);
			OnShowElement(args.ElementUID);
		}

		public bool IsPlanTreeVisible
		{
			get { return !GlobalSettingsHelper.GlobalSettings.Monitor_HidePlansTree && PlanTreeViewModel != null; }
		}

		public override void OnShow()
		{
			base.OnShow();
			foreach (var planPresenter in PlanPresenters)
				planPresenter.ExtensionAttached();
			if (PlanTreeViewModel != null)
				PlanTreeViewModel.Select();
		}

		GridLength _planNavigationWidth;
		public GridLength PlanNavigationWidth
		{
			get { return _planNavigationWidth; }
			set
			{
				_planNavigationWidth = value;
				OnPropertyChanged(() => PlanNavigationWidth);
			}
		}

		private void OnChangePlanProperties(List<PlanCallbackData> properties)
		{
			foreach (var property in properties)
				SetPlanProperty(property);
		}
		private void OnAutomationCallback(AutomationCallbackResult automationCallbackResult)
		{
			switch (automationCallbackResult.AutomationCallbackType)
			{
				case AutomationCallbackType.SetPlanProperty:
					SetPlanProperty((PlanCallbackData)automationCallbackResult.Data);
					break;
				case AutomationCallbackType.GetPlanProperty:
					GetPlanProperty((PlanCallbackData)automationCallbackResult.Data, automationCallbackResult.CallbackUID);
					break;
			}
		}
		private void SetPlanProperty(PlanCallbackData data)
		{
			var element = GetElement(data);
			if (element == null)
				return;
			switch (data.ElementPropertyType)
			{
				case ElementPropertyType.Color:
					element.BorderColor = Utils.Cast<Color>(data.Value);
					break;
				case ElementPropertyType.BackColor:
					element.BackgroundColor = Utils.Cast<Color>(data.Value);
					break;
				case ElementPropertyType.BorderThickness:
					element.BorderThickness = Convert.ToDouble(data.Value);
					break;
				case ElementPropertyType.Left:
					element.Position = new System.Windows.Point(Convert.ToDouble(data.Value), element.Position.Y);
					break;
				case ElementPropertyType.Top:
					element.Position = new System.Windows.Point(element.Position.X, Convert.ToDouble(data.Value));
					break;
			}
			var elementRectangle = element as ElementBaseRectangle;
			if (elementRectangle != null)
				switch (data.ElementPropertyType)
				{
					case ElementPropertyType.Height:
						elementRectangle.Height = Convert.ToDouble(data.Value);
						break;
					case ElementPropertyType.Width:
						elementRectangle.Width = Convert.ToDouble(data.Value);
						break;
				}
			var elementText = element as IElementTextBlock;
			if (elementText != null)
				switch (data.ElementPropertyType)
				{
					case ElementPropertyType.FontBold:
						elementText.FontBold = Convert.ToBoolean(data.Value);
						break;
					case ElementPropertyType.FontItalic:
						elementText.FontItalic = Convert.ToBoolean(data.Value);
						break;
					case ElementPropertyType.FontSize:
						elementText.FontSize = Convert.ToDouble(data.Value);
						break;
					case ElementPropertyType.ForegroundColor:
						elementText.ForegroundColor = Utils.Cast<Color>(data.Value);
						break;
					case ElementPropertyType.Stretch:
						elementText.Stretch = Convert.ToBoolean(data.Value);
						break;
					case ElementPropertyType.Text:
						elementText.Text = Convert.ToString(data.Value);
						break;
					case ElementPropertyType.WordWrap:
						elementText.WordWrap = Convert.ToBoolean(data.Value);
						break;
				}
			ApplicationService.Invoke(() =>
			{
				var presenterItem = PlanDesignerViewModel.PresenterItems.FirstOrDefault(item => item.Element == element);
				if (presenterItem != null)
				{
					presenterItem.InvalidatePainter();
					presenterItem.DesignerCanvas.Refresh();
				}
			});
		}
		private void GetPlanProperty(PlanCallbackData data, Guid callbackUID)
		{
			var value = new object();
			var element = GetElement(data);
			if (element != null)
			{
				var elementRectangle = element as ElementBaseRectangle;
				if (elementRectangle != null)
					switch (data.ElementPropertyType)
					{
						case ElementPropertyType.Height:
							value = Convert.ToInt32(elementRectangle.Height);
							break;
						case ElementPropertyType.Width:
							value = Convert.ToInt32(elementRectangle.Width);
							break;
					}
				var elementText = element as IElementTextBlock;
				if (elementText != null)
					switch (data.ElementPropertyType)
					{
						case ElementPropertyType.FontBold:
							value = elementText.FontBold;
							break;
						case ElementPropertyType.FontItalic:
							value = elementText.FontItalic;
							break;
						case ElementPropertyType.FontSize:
							value = Convert.ToInt32(elementText.FontSize);
							break;
						case ElementPropertyType.ForegroundColor:
							value = elementText.ForegroundColor;
							break;
						case ElementPropertyType.Stretch:
							value = elementText.Stretch;
							break;
						case ElementPropertyType.Text:
							value = elementText.Text;
							break;
						case ElementPropertyType.WordWrap:
							value = elementText.WordWrap;
							break;
					}
				switch (data.ElementPropertyType)
				{
					case ElementPropertyType.Color:
						value = element.BorderColor;
						break;
					case ElementPropertyType.BackColor:
						value = element.BackgroundColor;
						break;
					case ElementPropertyType.BorderThickness:
						value = Convert.ToInt32(element.BorderThickness);
						break;
					case ElementPropertyType.Left:
						value = Convert.ToInt32(element.Position.X);
						break;
					case ElementPropertyType.Top:
						value = Convert.ToInt32(element.Position.Y);
						break;
				}
			}
			FiresecManager.FiresecService.ProcedureCallbackResponse(callbackUID, value);
		}
		private ElementBase GetElement(PlanCallbackData data)
		{
			var plan = PlanTreeViewModel == null ? (PlanDesignerViewModel.Plan.UID == data.PlanUid ? PlanDesignerViewModel.PlanViewModel : null) : PlanTreeViewModel.Plans.FirstOrDefault(x => x.Plan.UID == data.PlanUid);
			if (plan == null)
				return null;
			return plan.Plan.SimpleElements.FirstOrDefault(x => x.UID == data.ElementUid);
		}
	}
}