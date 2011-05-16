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
using Infrastructure.Common;
using Infrastructure.Events;
using Firesec;
using System.Diagnostics;

namespace PlansModule.ViewModels
{
    public class ElementZoneViewModel : BaseViewModel
    {
        public ElementZoneViewModel()
        {
            ShowCommand = new RelayCommand(OnShow);
            ServiceFactory.Events.GetEvent<ZoneStateChangedEvent>().Subscribe(OnZoneStateChanged);
        }

        public bool IsSelected { get; set; }
        Polygon zonePolygon;
        public ElementZone elementZone;
        Zone zone;

        public void Initialize(ElementZone elementZone, Canvas canvas)
        {
            this.elementZone = elementZone;
            zone = FiresecManager.CurrentConfiguration.Zones.FirstOrDefault(x => x.No == elementZone.ZoneNo);

            zonePolygon = new Polygon();
            foreach (PolygonPoint polygonPoint in elementZone.PolygonPoints)
            {
                zonePolygon.Points.Add(new System.Windows.Point() { X = polygonPoint.X, Y = polygonPoint.Y });
            }
            zonePolygon.Fill = Brushes.Transparent;
            zonePolygon.Opacity = 0.6;
            zonePolygon.Stroke = Brushes.Orange;
            zonePolygon.StrokeThickness = 0;

            zonePolygon.ToolTip = "Зона " + elementZone.ZoneNo;

            zonePolygon.MouseEnter += new System.Windows.Input.MouseEventHandler(zonePolygon_MouseEnter);
            zonePolygon.MouseLeave += new System.Windows.Input.MouseEventHandler(zonePolygon_MouseLeave);
            zonePolygon.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(zonePolygon_PreviewMouseLeftButtonDown);

            canvas.Children.Add(zonePolygon);

            OnZoneStateChanged(elementZone.ZoneNo);
        }

        void zonePolygon_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            zonePolygon.StrokeThickness = 1;
        }

        void zonePolygon_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            zonePolygon.StrokeThickness = 0;
        }

        public event Action Selected;

        void zonePolygon_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Selected != null)
                Selected();
        }

        public string Name
        {
            get { return zone.No + "." + zone.Name; }
        }

        public RelayCommand ShowCommand { get; private set; }
        void OnShow()
        {
            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(elementZone.ZoneNo);
        }

        void OnZoneStateChanged(string zoneNo)
        {
            if (elementZone.ZoneNo == zoneNo)
            {
                ZoneState zoneState = FiresecManager.CurrentStates.ZoneStates.FirstOrDefault(x => x.No == zoneNo);

                switch (zoneState.State.StateType)
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
