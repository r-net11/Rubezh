using System;
using System.Collections.Generic;
using System.IO;
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
                    planInner.Name = _planInner.caption;
                    planInner.Height = Double.Parse(_planInner.height);
                    planInner.Width = Double.Parse(_planInner.width);
                    int index = 0;
                    foreach (var _elementInner in _planInner.layer)
                    {
                        if (_elementInner.name == "План") // Графические примитивы
                        {
                            if (_elementInner.elements != null)
                            {
                                foreach (var elementLayer in _elementInner.elements)
                                {
                                    switch (elementLayer.@class)
                                    {
                                        case "TSCDePicture":
                                            int iterator = 0;
                                            foreach (var elementImage in elementLayer.picture)
                                            {
                                                if (string.IsNullOrEmpty(elementImage.idx))
                                                    elementImage.idx = iterator++.ToString();
                                                Uri uri = new Uri(Environment.CurrentDirectory + "\\Pictures\\Sample" + elementImage.idx + "." + elementImage.ext);
                                                if (!File.Exists(uri.AbsolutePath))
                                                    continue;
                                                byte[] image = File.ReadAllBytes(uri.AbsolutePath);
                                                if (planInner.BackgroundPixels == null) planInner.BackgroundPixels = image;
                                                else
                                                {
                                                    RectangleBox rect = new RectangleBox();
                                                    uri = new Uri(Environment.CurrentDirectory + "\\Pictures\\Sample" + elementImage.idx + "." + elementImage.ext);
                                                    image = File.ReadAllBytes(uri.AbsolutePath);
                                                    rect.BackgroundPixels = image;
                                                    rect.Height = ValidationDouble(elementLayer.rect[0].bottom) - ValidationDouble(elementLayer.rect[0].top);
                                                    rect.Width = ValidationDouble(elementLayer.rect[0].right) - ValidationDouble(elementLayer.rect[0].left);
                                                    rect.Left = ValidationDouble(elementLayer.rect[0].left);
                                                    rect.Top = ValidationDouble(elementLayer.rect[0].top);
                                                    if (planInner.Rectangls == null) planInner.Rectangls = new List<RectangleBox>();
                                                    planInner.Rectangls.Add(rect);
                                                }
                                            }
                                            break;
                                        case "TSCDeLabel":
                                            CaptionBox captionBox = new CaptionBox();
                                            if (elementLayer.brush != null) captionBox.BorderColor = elementLayer.brush[0].color;
                                            if (elementLayer.pen != null) captionBox.Color = elementLayer.pen[0].color;
                                            captionBox.Text = elementLayer.caption;
                                            captionBox.Left = ValidationDouble(elementLayer.rect[0].left);
                                            captionBox.Top = ValidationDouble(elementLayer.rect[0].top);
                                            if (planInner.TextBoxes == null) planInner.TextBoxes = new List<CaptionBox>();
                                            planInner.TextBoxes.Add(captionBox);
                                            break;
                                    }
                                }
                            }
                        }
                        if ((_elementInner.name == "Зоны") || (_elementInner.name == "Несвязанные зоны") || (_elementInner.name == "Пожарные зоны") || (_elementInner.name == "Охранные зоны"))
                        {
                            if (_elementInner.elements != null)
                            {
                                if (planInner.ElementZones == null) planInner.ElementZones = new List<ElementZone>();
                                foreach (var _zoneInner in _elementInner.elements)
                                {
                                    ElementZone zoneInner = null;

                                    zoneInner = new ElementZone();
                                    string _idTempS = _zoneInner.id;
                                    long _idTempL = long.Parse(_idTempS);
                                    int _idTempI = (int) _idTempL;
                                    foreach (var _index in FiresecManager.DeviceConfiguration.Zones)
                                    {
                                        foreach (var zoneShapeId in _index.ShapeIds)
                                        {
                                            if (zoneShapeId == _idTempL.ToString())
                                            {
                                                zoneInner.ZoneNo = _index.No;
                                            }
                                            else
                                                if (zoneShapeId == _idTempI.ToString())
                                                {
                                                    zoneInner.ZoneNo = _index.No;
                                                }
                                        }
                                    }
                                    PolygonPoint polygonPointsInner = null;
                                    zoneInner.PolygonPoints = new List<PolygonPoint>();
                                    switch (_zoneInner.@class)
                                    {
                                        case "TFS_PolyZoneShape":
                                            if (_zoneInner.points != null)
                                            {
                                                foreach (var _pointInner in _zoneInner.points)
                                                {
                                                    polygonPointsInner = new PolygonPoint();
                                                    polygonPointsInner.X = ValidationDouble(_pointInner.x);
                                                    polygonPointsInner.Y = ValidationDouble(_pointInner.y);
                                                    _pointInner.y = _pointInner.y.Replace(".", ",");

                                                    zoneInner.PolygonPoints.Add(polygonPointsInner);
                                                }
                                            }; break;
                                        case "TFS_ZoneShape":
                                            foreach (var _rectInner in _zoneInner.rect)
                                            {
                                                polygonPointsInner = new PolygonPoint();
                                                polygonPointsInner.X = ValidationDouble(_rectInner.left);
                                                polygonPointsInner.Y = ValidationDouble(_rectInner.top);
                                                zoneInner.PolygonPoints.Add(polygonPointsInner);
                                                polygonPointsInner = new PolygonPoint();
                                                polygonPointsInner.X = ValidationDouble(_rectInner.right);
                                                polygonPointsInner.Y = ValidationDouble(_rectInner.top);
                                                zoneInner.PolygonPoints.Add(polygonPointsInner);
                                                polygonPointsInner = new PolygonPoint();
                                                polygonPointsInner.X = ValidationDouble(_rectInner.right);
                                                polygonPointsInner.Y = ValidationDouble(_rectInner.bottom);
                                                zoneInner.PolygonPoints.Add(polygonPointsInner);
                                                polygonPointsInner = new PolygonPoint();
                                                polygonPointsInner.X = ValidationDouble(_rectInner.left);
                                                polygonPointsInner.Y = ValidationDouble(_rectInner.bottom);
                                                zoneInner.PolygonPoints.Add(polygonPointsInner);
                                            };
                                            break;
                                    }

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
                                            deviceInner.Left = ValidationDouble(_rectInner.left);
                                            deviceInner.Top = ValidationDouble(_rectInner.top);
                                            deviceInner.Width = ValidationDouble(_rectInner.right) - deviceInner.Left;
                                            deviceInner.Height = ValidationDouble(_rectInner.bottom) - deviceInner.Top;
                                        }
                                        planInner.ElementDevices.Add(deviceInner);
                                    }
                                }
                            }
                        }
                        index++;
                    }
                    plansConfiguration.Plans.Add(planInner);
                    /*
                    List<Plan> plans = plansConfiguration.Plans;
                    System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(typeof(List<Plan>));
                    FileStream fs_out = new FileStream(@"D:/del/Plans_new310811.xml", FileMode.Create);
                    System.Xml.XmlDictionaryWriter xdw = System.Xml.XmlDictionaryWriter.CreateTextWriter(fs_out);
                    dcs.WriteObject(xdw, plans);
                    xdw.Close();
                    */
                }
            }
            
            return plansConfiguration;
        }

        static Double ValidationDouble(string str)
        {
            Double result;
            try
            {
                str = str.Replace(".", ",");
                result = Double.Parse(str);
                return result;
            }
            catch
            {
                str = str.Replace(",", ".");
                result = Double.Parse(str);
                return result;
            }
        }
    }
}
