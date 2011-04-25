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
using FiresecClient;
using Infrastructure.Events;
using Firesec;

namespace PlansModule.ViewModels
{
    public class ElementZoneViewModel : BaseViewModel
    {
        public ElementZoneViewModel()
        {
            ShowInListCommand = new RelayCommand(OnShowInList);
            ServiceFactory.Events.GetEvent<ZoneStateChangedEvent>().Subscribe(OnZoneStateChanged);
        }

        Polygon zonePolygon;
        public ElementZone elementZone;

        public void Initialize(ElementZone elementZone, Canvas canvas)
        {
            this.elementZone = elementZone;
            Zone zone = FiresecManager.CurrentConfiguration.Zones.FirstOrDefault(x => x.No == elementZone.ZoneNo);
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

            OnZoneStateChanged(elementZone.ZoneNo);
        }

        void zonePolygon_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            zonePolygon.Stroke = Brushes.Orange;
            zonePolygon.StrokeThickness = 1;
            //zonePolygon.Fill = Brushes.Green;
            zonePolygon.Opacity = 0.3;
        }

        void zonePolygon_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            zonePolygon.StrokeThickness = 0;
            zonePolygon.Opacity = 0.6;
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

        void OnZoneStateChanged(string zoneNo)
        {
            if (elementZone.ZoneNo == zoneNo)
            {
                ZoneState zoneState = FiresecManager.CurrentStates.ZoneStates.FirstOrDefault(x => x.No == zoneNo);

                StateType stateType = StateHelper.NameToType(zoneState.State);
                switch (stateType)
                {
                    case StateType.Alarm:
                        zonePolygon.Fill = Brushes.Red;
                        break;

                    case StateType.Failure:
                        zonePolygon.Fill = Brushes.Red;
                        break;

                    case StateType.Info:
                        zonePolygon.Fill = Brushes.YellowGreen;
                        break;

                    case StateType.No:
                        zonePolygon.Fill = Brushes.Transparent;
                        break;

                    case StateType.Norm:
                        zonePolygon.Fill = Brushes.Green;
                        break;

                    case StateType.Off:
                        zonePolygon.Fill = Brushes.Red;
                        break;

                    case StateType.Service:
                        zonePolygon.Fill = Brushes.Yellow;
                        break;

                    case StateType.Unknown:
                        zonePolygon.Fill = Brushes.Gray;
                        break;

                    case StateType.Warning:
                        zonePolygon.Fill = Brushes.Yellow;
                        break;
                }
            }
        }
    }
}
