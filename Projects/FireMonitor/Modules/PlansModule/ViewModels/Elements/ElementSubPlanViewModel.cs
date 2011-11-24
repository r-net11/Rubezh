using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Events;

namespace PlansModule.ViewModels
{
    public class ElementSubPlanViewModel : BaseViewModel
    {
        public ElementSubPlanView ElementSubPlanView { get; private set; }
        public Guid PlanUID { get; private set; }
        public string PresentationName { get; private set; }

        public ElementSubPlanViewModel(ElementSubPlan elementSubPlan)
        {
            PlanUID = elementSubPlan.UID;
            if (elementSubPlan.Plan != null)
                PresentationName = elementSubPlan.Plan.Caption;

            ElementSubPlanView = new ElementSubPlanView();
            foreach (var polygonPoint in elementSubPlan.PolygonPoints)
            {
                ElementSubPlanView._polygon.Points.Add(new System.Windows.Point() { X = polygonPoint.X, Y = polygonPoint.Y });
            }
            ElementSubPlanView.PlanUID = elementSubPlan.UID;
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