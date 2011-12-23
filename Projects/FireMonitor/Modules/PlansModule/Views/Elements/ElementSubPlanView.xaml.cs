using System;
using System.Windows.Controls;
using System.Windows.Input;
using Infrastructure;
using PlansModule.Events;

namespace PlansModule
{
    public partial class ElementSubPlanView : ContentControl
    {
        public Guid PlanUID { get; set; }

        public ElementSubPlanView()
        {
            InitializeComponent();
        }

        void _polygon_MouseEnter(object sender, MouseEventArgs e)
        {
            _polygon.StrokeThickness = 1;
        }

        void _polygon_MouseLeave(object sender, MouseEventArgs e)
        {
            _polygon.StrokeThickness = 0;
        }

        private void _polygon_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ServiceFactory.Events.GetEvent<SelectPlanEvent>().Publish(PlanUID);
            }
        }
    }
}