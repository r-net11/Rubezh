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
            foreach (var polygonPoint in elementSubPlan.PolygonPoints)
            {
                ElementSubPlanView._polygon.Points.Add(new System.Windows.Point() { X = polygonPoint.X, Y = polygonPoint.Y });
            }
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