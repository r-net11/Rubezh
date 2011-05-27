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
            ServiceFactory.Events.GetEvent<ZoneStateChangedEvent>().Subscribe(OnZoneStateChanged);
        }

        Polygon _zonePolygon;
        ElementZone _elementZone;
        Zone _zone;

        public void Initialize(ElementZone elementZone, Canvas canvas)
        {
            _elementZone = elementZone;
            _zone = FiresecManager.CurrentConfiguration.Zones.FirstOrDefault(x => x.No == elementZone.ZoneNo);

            _zonePolygon = new Polygon();
            foreach (PolygonPoint polygonPoint in elementZone.PolygonPoints)
            {
                _zonePolygon.Points.Add(new System.Windows.Point() { X = polygonPoint.X, Y = polygonPoint.Y });
            }
            _zonePolygon.Fill = Brushes.Transparent;
            _zonePolygon.Opacity = 0.6;
            _zonePolygon.Stroke = Brushes.Orange;
            _zonePolygon.StrokeThickness = 0;

            _zonePolygon.ToolTip = "Зона " + _zone.No + "." + _zone.Name;

            _zonePolygon.MouseEnter += new System.Windows.Input.MouseEventHandler(zonePolygon_MouseEnter);
            _zonePolygon.MouseLeave += new System.Windows.Input.MouseEventHandler(zonePolygon_MouseLeave);
            _zonePolygon.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(zonePolygon_PreviewMouseLeftButtonDown);

            AddContextMenu();

            canvas.Children.Add(_zonePolygon);

            OnZoneStateChanged(elementZone.ZoneNo);
        }

        void AddContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Показать в дереве";
            menuItem.Click += new System.Windows.RoutedEventHandler(menuItem_Click);
            contextMenu.Items.Add(menuItem);
            _zonePolygon.ContextMenu = contextMenu;
        }

        void menuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(_elementZone.ZoneNo);
        }

        void zonePolygon_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _zonePolygon.StrokeThickness = 1;
        }

        void zonePolygon_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _zonePolygon.StrokeThickness = 0;
        }

        public string ZoneNo
        {
            get
            {
                return _elementZone.ZoneNo;
            }
        }

        public event Action Selected;

        void zonePolygon_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Selected != null)
                Selected();
        }

        bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                //_zonePolygon.StrokeThickness = value ? 1 : 0;
                OnPropertyChanged("IsSelected");
            }
        }

        void OnZoneStateChanged(string zoneNo)
        {
            if (_elementZone.ZoneNo == zoneNo)
            {
                ZoneState zoneState = FiresecManager.CurrentStates.ZoneStates.FirstOrDefault(x => x.No == zoneNo);

                switch (zoneState.State.StateType)
                {
                    case StateType.Alarm:
                        _zonePolygon.Fill = Brushes.Red;
                        break;

                    case StateType.Failure:
                        _zonePolygon.Fill = Brushes.Red;
                        break;

                    case StateType.Info:
                        _zonePolygon.Fill = Brushes.YellowGreen;
                        break;

                    case StateType.No:
                        _zonePolygon.Fill = Brushes.Transparent;
                        break;

                    case StateType.Norm:
                        _zonePolygon.Fill = Brushes.Green;
                        break;

                    case StateType.Off:
                        _zonePolygon.Fill = Brushes.Red;
                        break;

                    case StateType.Service:
                        _zonePolygon.Fill = Brushes.Yellow;
                        break;

                    case StateType.Unknown:
                        _zonePolygon.Fill = Brushes.Gray;
                        break;

                    case StateType.Warning:
                        _zonePolygon.Fill = Brushes.Yellow;
                        break;
                }
            }
        }
    }
}
