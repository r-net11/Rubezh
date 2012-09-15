using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace PlansModule.ViewModels
{
	public class ElementSubPlanViewModel : BaseViewModel
	{
		public ElementSubPlanView ElementSubPlanView { get; private set; }
		public ElementSubPlan ElementSubPlan { get; private set; }

		public ElementSubPlanViewModel(ElementSubPlan elementSubPlan)
		{
			ElementSubPlan = elementSubPlan;

			ElementSubPlanView = new ElementSubPlanView();
			//Rect rectangle = elementSubPlan.GetRectangle();
			//ElementSubPlanView._polygon.Points.Add(rectangle.TopLeft);
			//ElementSubPlanView._polygon.Points.Add(rectangle.TopRight);
			//ElementSubPlanView._polygon.Points.Add(rectangle.BottomRight);
			//ElementSubPlanView._polygon.Points.Add(rectangle.BottomLeft);
			ElementSubPlanView.PlanUID = elementSubPlan.PlanUID;
		}

		StateType _stateType;
		public StateType StateType
		{
			get { return _stateType; }
			set
			{
				_stateType = value;
				OnPropertyChanged("StateType");
			}
		}
	}
}