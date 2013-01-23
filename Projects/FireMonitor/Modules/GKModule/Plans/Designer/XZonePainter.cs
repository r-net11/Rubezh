﻿using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using FiresecAPI;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;
using XFiresecAPI;
using System.Windows.Controls;
using Controls.Converters;

namespace GKModule.Plans.Designer
{
    class XZonePainter : PolygonZonePainter, IPainter
    {
        private PresenterItem _presenterItem;
        private IPainter _painter;
        private XZone _zone;
        private ContextMenu _contextMenu;

        public XZonePainter(PresenterItem presenterItem)
            : base(presenterItem.Element)
        {
            ShowInTreeCommand = new RelayCommand(OnShowInTree, CanShowInTree);
            _presenterItem = presenterItem;
            _painter = presenterItem.Painter;
            Bind();
        }

        private void Bind()
        {
            _presenterItem.SetBorder(new PresenterBorder(_presenterItem));
            _presenterItem.ContextMenuProvider = CreateContextMenu;
            _zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == ((IElementZone)_presenterItem.Element).ZoneUID);
            if (_zone != null)
                _zone.ZoneState.StateChanged += OnPropertyChanged;
            _presenterItem.Title = GetZoneTooltip();
        }

        private void OnPropertyChanged()
        {
            _presenterItem.Title = GetZoneTooltip();
            //_presenterItem.Redraw();
			_presenterItem.DesignerCanvas.Refresh();
        }
        private string GetZoneTooltip()
        {
            if (_zone == null)
                return null;
            var sb = new StringBuilder();
            sb.AppendLine(_zone.PresentationName);
            sb.AppendLine("Состояние: " + _zone.ZoneState.GetStateType().ToDescription());
            return sb.ToString().TrimEnd();
        }

        #region IPainter Members

        protected override Brush GetBrush()
        {
            return PainterCache.GetBrush(GetStateColor());
        }
        //public UIElement Draw(ElementBase element)
        //{
        //    if (_zone == null)
        //        return null;
        //    var shape = (Shape)_painter.Draw(element);
        //    shape.Fill = GetStateBrush();
        //    shape.Opacity = 1;
        //    return shape;
        //}

        #endregion

        public Color GetStateColor()
        {
            var stateType = _zone.ZoneState.GetStateType();
            switch (stateType)
            {
                case StateType.Fire:
                    return Colors.Red;

                case StateType.Attention:
                    return Colors.Yellow;

                case StateType.Failure:
                    return Colors.Pink;

                case StateType.Service:
                    return Colors.Yellow;

                case StateType.Off:
                    return Colors.Yellow;

                case StateType.Unknown:
                    return Colors.Gray;

                case StateType.Info:
                    return Colors.LightBlue;

                case StateType.Norm:
                    return Colors.LightGreen;

                case StateType.No:
                    return Colors.White;

                default:
                    return Colors.Black;
            }
        }

        public RelayCommand ShowInTreeCommand { get; private set; }
        void OnShowInTree()
        {
            ServiceFactory.Events.GetEvent<ShowXZoneEvent>().Publish(_zone.UID);
        }
        bool CanShowInTree()
        {
            return _zone != null;
        }

        private ContextMenu CreateContextMenu()
        {
            if (_contextMenu == null)
            {
                _contextMenu = new ContextMenu();
                _contextMenu.Items.Add(new MenuItem()
                {
                    Header = "Показать в списке",
                    Command = ShowInTreeCommand
                });
            }
            return _contextMenu;
        }
    }
}