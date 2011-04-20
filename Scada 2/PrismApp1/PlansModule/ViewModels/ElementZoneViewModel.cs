using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using PlansModule.Events;
using PlansModule.Models;
using ClientApi;
using Infrastructure.Events;

namespace PlansModule.ViewModels
{
    public class ElementZoneViewModel : BaseViewModel
    {
        public ElementZoneViewModel()
        {
            ShowInListCommand = new RelayCommand(OnShowInList);
        }

        Polygon zonePolygon;
        public ElementZone elementZone;

        public void Initialize(ElementZone elementZone, Canvas canvas)
        {
            this.elementZone = elementZone;
            Zone zone = ServiceClient.CurrentConfiguration.Zones.FirstOrDefault(x => x.No == elementZone.ZoneNo);
            Name = zone.Name;

            zonePolygon = new Polygon();
            foreach (PolygonPoint polygonPoint in elementZone.PolygonPoints)
            {
                zonePolygon.Points.Add(new System.Windows.Point() { X = polygonPoint.X, Y = polygonPoint.Y });
            }
            zonePolygon.Fill = Brushes.Transparent;

            zonePolygon.ToolTip = "Зона " + elementZone.ZoneNo;

            zonePolygon.MouseEnter += new System.Windows.Input.MouseEventHandler(zonePolygon_MouseEnter);
            zonePolygon.MouseLeave += new System.Windows.Input.MouseEventHandler(zonePolygon_MouseLeave);
            zonePolygon.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(zonePolygon_PreviewMouseLeftButtonDown);

            canvas.Children.Add(zonePolygon);
        }

        void zonePolygon_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            zonePolygon.Stroke = Brushes.Orange;
            zonePolygon.StrokeThickness = 1;
            zonePolygon.Fill = Brushes.Green;
            zonePolygon.Opacity = 0.5;
        }

        void zonePolygon_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            zonePolygon.StrokeThickness = 0;
            zonePolygon.Opacity = 0.0;
        }

        void zonePolygon_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ServiceFactory.Events.GetEvent<PlanZoneSelectedEvent>().Publish(elementZone.ZoneNo);
        }

        string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                OnPropertyChanged("IsActive");
            }
        }

        public RelayCommand ShowInListCommand { get; private set; }
        void OnShowInList()
        {
            ServiceFactory.Events.GetEvent<ShowZonesEvent>().Publish(elementZone.ZoneNo);
        }
    }
}
