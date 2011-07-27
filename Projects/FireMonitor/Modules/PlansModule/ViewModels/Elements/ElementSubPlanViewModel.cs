using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using PlansModule.Events;
using PlansModule.Models;
using Infrastructure.Common;
using FiresecClient.Models;

namespace PlansModule.ViewModels
{
    public class ElementSubPlanViewModel : BaseViewModel
    {
        public ElementSubPlanViewModel()
        {
            ShowPropertiesCommand = new RelayCommand(OnShowProperties);
        }

        public string Name { get; private set; }
        public string PresentationName { get; private set; }
        ElementSubPlanView _elementSubPlanView;

        public void Initialize(ElementSubPlan elementSubPlan, Canvas canvas)
        {
            Name = elementSubPlan.Name;
            PresentationName = elementSubPlan.Caption;

            _elementSubPlanView = new ElementSubPlanView();
            _elementSubPlanView.DataContext = this;
            _elementSubPlanView._polygon.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(subPlanPolygon_PreviewMouseLeftButtonDown);

            foreach (var polygonPoint in elementSubPlan.PolygonPoints)
            {
                _elementSubPlanView._polygon.Points.Add(new System.Windows.Point() { X = polygonPoint.X, Y = polygonPoint.Y });
            }
            if (elementSubPlan.ShowBackgroundImage)
            {
                ImageBrush polygonImageBrush = new ImageBrush();
                polygonImageBrush.ImageSource = new BitmapImage(new Uri(elementSubPlan.BackgroundSource, UriKind.Absolute));
                _elementSubPlanView._polygon.Fill = polygonImageBrush;
            }

            canvas.Children.Add(_elementSubPlanView);
        }

        State _state;
        public State State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged("State");
            }
        }

        void subPlanPolygon_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ServiceFactory.Events.GetEvent<SelectPlanEvent>().Publish(Name);
            }
        }

        public RelayCommand ShowPropertiesCommand { get; private set; }
        void OnShowProperties()
        {
        }

        public void Update(State state)
        {
            State = state;
        }
    }
}
