using System;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace FiresecService.Converters
{
    public static class PlansConverter
    {
        public static PlansConfiguration Convert(Firesec.Plans.surfaces innerPlans)
        {
            var plansConfiguration = new PlansConfiguration();
            if (innerPlans.surface != null)
            {
                plansConfiguration.Plans = new List<Plan>();
                foreach (var _planInner in innerPlans.surface)
                {
                    Plan planInner = new Plan();
                    planInner.Caption = _planInner.caption;
                    planInner.Height = Double.Parse(_planInner.height);
                    planInner.Width = Double.Parse(_planInner.width);
                    int index = 0;
                    foreach (var _elementInner in _planInner.layer)
                    {
                        if (_elementInner.name == "План") // Графические примитивы
                        {
                            ;
                        }
                        if ((_elementInner.name =="Несвязанные зоны") || (_elementInner.name == "Пожарные зоны") || (_elementInner.name == "Охранные зоны"))
                        {
                            
                            ElementZone zoneInner = null;
                            if (_elementInner.elements != null)
                            {
                                if (planInner.ElementZones == null) planInner.ElementZones = new List<ElementZone>();
                                foreach (var _zoneInner in _elementInner.elements)
                                {
                                    zoneInner = new ElementZone();
                                    string _idTempS = _zoneInner.id;
                                    long _idTempL = long.Parse(_idTempS);
                                    int _idTempI = (int)_idTempL;
                                    //List<Zone> temp=FiresecManager.DeviceConfiguration.Zones;
                                    foreach (var _index in FiresecManager.DeviceConfiguration.Zones)
                                    {
                                        if (_index.ShapeId == _idTempL.ToString())
                                        {
                                            zoneInner.ZoneNo = _index.No;
                                        }
                                        else
                                            if (_index.ShapeId == _idTempI.ToString())
                                            {
                                                zoneInner.ZoneNo = _index.No;
                                            }
                                    }
                                    PolygonPoint polygonPointsInner = null;
                                    zoneInner.PolygonPoints = new List<PolygonPoint>();
                                    if (_zoneInner.@class.Equals("TFS_PolyZoneShape"))
                                    {
                                        if (_zoneInner.points != null)
                                        {
                                            foreach (var _pointInner in _zoneInner.points)
                                            {
                                                polygonPointsInner = new PolygonPoint();
                                                polygonPointsInner.X = Double.Parse(_pointInner.x);
                                                polygonPointsInner.Y = Double.Parse(_pointInner.y);
                                                zoneInner.PolygonPoints.Add(polygonPointsInner);
                                            }

                                        }
                                    };
                                    if (_zoneInner.@class.Equals("TFS_ZoneShape"))
                                    {
                                       /* 
                                        if (_zoneInner.rect != null)
                                    if (xZone != null)
                                    {
                                        zone.ZoneNo = xZone.No;
                                    }
                                    else
                                    {
                                        ;
                                    }
                                        */
                                        {
                                            foreach (var _rectInner in _zoneInner.rect)
                                            {
                                                polygonPointsInner = new PolygonPoint();
                                                polygonPointsInner.X = Double.Parse(_rectInner.left);
                                                polygonPointsInner.Y = Double.Parse(_rectInner.top);
                                                zoneInner.PolygonPoints.Add(polygonPointsInner);
                                                polygonPointsInner = new PolygonPoint();
                                                polygonPointsInner.X = Double.Parse(_rectInner.right);
                                                polygonPointsInner.Y = Double.Parse(_rectInner.top);
                                                zoneInner.PolygonPoints.Add(polygonPointsInner);
                                                polygonPointsInner = new PolygonPoint();
                                                polygonPointsInner.X = Double.Parse(_rectInner.right);
                                                polygonPointsInner.Y = Double.Parse(_rectInner.bottom);
                                                zoneInner.PolygonPoints.Add(polygonPointsInner);
                                                polygonPointsInner = new PolygonPoint();
                                                polygonPointsInner.X = Double.Parse(_rectInner.left);
                                                polygonPointsInner.Y = Double.Parse(_rectInner.bottom);
                                                zoneInner.PolygonPoints.Add(polygonPointsInner);
                                            }
                                        }
                                    };
                                    planInner.ElementZones.Add(zoneInner);
                                }
                            }
                        };
                        if (_elementInner.name == "Устройства")
                        {
                            ElementDevice deviceInner = null;
                            if (_elementInner.elements != null)
                            {
                                if (planInner.ElementDevices == null) planInner.ElementDevices = new List<ElementDevice>();
                                foreach (var _deviceInner in _elementInner.elements)
                                {
                                    deviceInner = new ElementDevice();
                                    
/* Нету ShapeId в девайсах
                                    string _idTempS = _deviceInner.id;
                                    long _idTempL = long.Parse(_idTempS);
                                    int _idTempI = (int)_idTempL;
                                    //List<Zone> temp=FiresecManager.DeviceConfiguration.Zones;
                                    foreach (var _index in FiresecManager.DeviceConfiguration.Devices)
                                    {
                                        if (_index.ShapeId == _idTempL.ToString())
                                        {
                                            deviceInner.ZoneNo = _index.No;
                                        }
                                        else
                                            if (_index.ShapeId == _idTempI.ToString())
                                            {
                                                deviceInner.ZoneNo = _index.No;
                                            }
                                    }
                                    */
                                    deviceInner.Id = "NULL";
                                    if (_deviceInner.rect != null)
                                    {
                                        foreach (var _rectInner in _deviceInner.rect)
                                        {
                                            string _left = _rectInner.left;
                                            _left = _left.Replace(".", ",");
                                            deviceInner.Left = double.Parse(_left);
                                            string _top= _rectInner.top;
                                            _top = _top.Replace(".", ",");
                                            deviceInner.Top = double.Parse(_top);
                                            string _right = _rectInner.right;
                                            _right = _right.Replace(".", ",");
                                            deviceInner.Width = Double.Parse(_right) - Double.Parse(_left);
                                            string _bottom = _rectInner.bottom;
                                            _bottom = _bottom.Replace(".", ",");
                                            deviceInner.Height = Double.Parse(_bottom) - Double.Parse(_top);
                                        }
                                        planInner.ElementDevices.Add(deviceInner);
                                    }
                                }
                            }
                        }
                        index++;
                    }
                    plansConfiguration.Plans.Add(planInner);
                }
            }
            return plansConfiguration;
        }
    }
}
