using System;
using System.IO;
using System.Windows;
using Firesec.Plans;
using FiresecAPI.Models;

namespace FiresecService.Converters
{
    public static class PlansConverter
    {
        public static PlansConfiguration Convert(surfaces innerPlans)
        {
            var plansConfiguration = new PlansConfiguration();

            if ((innerPlans != null) && (innerPlans.surface != null))
            {
                foreach (var innerPlan in innerPlans.surface)
                {
                    var plan = new Plan()
                    {
                        Caption = innerPlan.caption,
                        Name = innerPlan.caption,
                        Height = Double.Parse(innerPlan.height) * 10,
                        Width = Double.Parse(innerPlan.width) * 10
                    };
                    int index = 0;
                    foreach (var innerLayer in innerPlan.layer)
                    {
                        if (innerLayer.elements == null)
                            continue;

                        if (innerLayer.name == "План")
                        {
                            foreach (var innerElementLayer in innerLayer.elements)
                            {
                                switch (innerElementLayer.@class)
                                {
                                    case "TSCDePicture":
                                        int pictureIndex = 0;
                                        foreach (var innerPicture in innerElementLayer.picture)
                                        {
                                            if (string.IsNullOrEmpty(innerPicture.idx))
                                                innerPicture.idx = pictureIndex++.ToString();

                                            Uri uri = new Uri(Environment.CurrentDirectory + "\\Pictures\\Sample" + innerPicture.idx + "." + innerPicture.ext);
                                            if (File.Exists(uri.AbsolutePath) == false)
                                                continue;
                                            byte[] imageBytes = File.ReadAllBytes(uri.AbsolutePath);

                                            var elementRectangle = new ElementRectangle()
                                            {
                                                Height = Parse(innerElementLayer.rect[0].bottom) - Parse(innerElementLayer.rect[0].top),
                                                Width = Parse(innerElementLayer.rect[0].right) - Parse(innerElementLayer.rect[0].left),
                                                Left = Parse(innerElementLayer.rect[0].left),
                                                Top = Parse(innerElementLayer.rect[0].top),
                                                BackgroundPixels = imageBytes
                                            };

                                            plan.ElementRectangles.Add(elementRectangle);
                                        }
                                        break;

                                    case "TSCDeLabel":
                                        var elementTextBlock = new ElementTextBlock()
                                        {
                                            Text = innerElementLayer.caption,
                                            Left = Parse(innerElementLayer.rect[0].left),
                                            Top = Parse(innerElementLayer.rect[0].top),
                                        };

                                        //if (innerElementLayer.brush != null)
                                        //    elementTextBlock.BorderColor = innerElementLayer.brush[0].color;

                                        if (innerElementLayer.pen != null)
                                            elementTextBlock.Color = innerElementLayer.pen[0].color;

                                        plan.ElementTextBlocks.Add(elementTextBlock);
                                        break;
                                }
                            }
                        }
                        if ((innerLayer.name == "Зоны") || (innerLayer.name == "Несвязанные зоны") || (innerLayer.name == "Пожарные зоны") || (innerLayer.name == "Охранные зоны"))
                        {
                            foreach (var innerZone in innerLayer.elements)
                            {
                                var elementZone = new ElementZone();

                                long idTempL = long.Parse(innerZone.id);
                                int idTempI = (int)idTempL;
                                foreach (var zone in ConfigurationConverter.DeviceConfiguration.Zones)
                                {
                                    foreach (var zoneShapeId in zone.ShapeIds)
                                    {
                                        if ((zoneShapeId == idTempL.ToString()) ||
                                            (zoneShapeId == idTempI.ToString()))
                                        {
                                            elementZone.ZoneNo = zone.No;
                                        }
                                    }
                                }

                                switch (innerZone.@class)
                                {
                                    case "TFS_PolyZoneShape":
                                        if (innerZone.points != null)
                                        {
                                            foreach (var innerPoint in innerZone.points)
                                            {
                                                var point = new Point()
                                                {
                                                    X = Parse(innerPoint.x),
                                                    Y = Parse(innerPoint.y)
                                                };
                                                innerPoint.y = innerPoint.y.Replace(".", ",");

                                                elementZone.PolygonPoints.Add(point);
                                            }
                                            elementZone.Normalize();
                                        };
                                        break;

                                    case "TFS_ZoneShape":
                                        foreach (var innerRect in innerZone.rect)
                                        {
                                            elementZone.PolygonPoints.Add(new Point()
                                            {
                                                X = Parse(innerRect.left),
                                                Y = Parse(innerRect.top)
                                            });

                                            elementZone.PolygonPoints.Add(new Point()
                                            {
                                                X = Parse(innerRect.right),
                                                Y = Parse(innerRect.top)
                                            });

                                            elementZone.PolygonPoints.Add(new Point()
                                            {
                                                X = Parse(innerRect.right),
                                                Y = Parse(innerRect.bottom)
                                            });

                                            elementZone.PolygonPoints.Add(new Point()
                                            {
                                                X = Parse(innerRect.left),
                                                Y = Parse(innerRect.bottom)
                                            });

                                            elementZone.Normalize();
                                        };
                                        break;
                                }
                                plan.ElementZones.Add(elementZone);
                            }
                        };

                        if (innerLayer.name == "Устройства")
                        {
                            foreach (var innerDevice in innerLayer.elements)
                            {
                                var elementDevice = new ElementDevice();
                                long idTempL = long.Parse(innerDevice.id);
                                int idTempI = (int)idTempL;

                                foreach (var device in ConfigurationConverter.DeviceConfiguration.Devices)
                                {
                                    foreach (var deviceShapeId in device.ShapeIds)
                                    {
                                        if ((deviceShapeId == idTempL.ToString()) ||
                                            (deviceShapeId == idTempI.ToString()))
                                        {
                                            elementDevice.Id = device.UID;
                                        }
                                    }
                                }

                                if (innerDevice.rect != null)
                                {
                                    foreach (var innerRect in innerDevice.rect)
                                    {
                                        elementDevice.Left = Parse(innerRect.left);
                                        elementDevice.Top = Parse(innerRect.top);
                                        elementDevice.Width = Parse(innerRect.right) - elementDevice.Left;
                                        elementDevice.Height = Parse(innerRect.bottom) - elementDevice.Top;
                                    }
                                }

                                plan.ElementDevices.Add(elementDevice);
                            }
                        }
                        index++;
                    }
                    plansConfiguration.Plans.Add(plan);
                }
            }

            return plansConfiguration;
        }

        static Double Parse(string input)
        {
            Double result;
            try
            {
                input = input.Replace(".", ",");
                result = Double.Parse(input);
                return result;
            }
            catch
            {
                input = input.Replace(",", ".");
                result = Double.Parse(input);
                return result;
            }
        }
    }
}
