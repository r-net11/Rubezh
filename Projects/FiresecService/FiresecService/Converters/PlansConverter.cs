using System;
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
                foreach (var _plan in innerPlans.surface)
                {
                    Plan plan = new Plan();
                    plan.Caption = _plan.caption;
                    plan.Height = Double.Parse(_plan.height);
                    plan.Width = Double.Parse(_plan.width);
                    int index = 0;
                    foreach (var _element in _plan.layer)
                    {
                        if (index == 2)// Пожарные зоны (добавить ID зоны!!!)
                        {
                            plan.ElementZones = new List<ElementZone>();
                            ElementZone zone = null;
                            foreach (var _zone in _element.elements)
                            {
                                zone = new ElementZone();
                                PolygonPoint polygonPoints = null;
                                zone.PolygonPoints = new List<PolygonPoint>();
                                if (_zone.@class.Equals("TFS_PolyZoneShape"))
                                {
                                    if (_zone.points != null)
                                    {
                                        foreach (var _point in _zone.points)
                                        {
                                            polygonPoints = new PolygonPoint();
                                            polygonPoints.X=Double.Parse(_point.x);
                                            polygonPoints.Y = Double.Parse(_point.y);
                                            zone.PolygonPoints.Add(polygonPoints);
                                        }
                                        
                                    }
                                };
                                if (_zone.@class.Equals("TFS_ZoneShape"))
                                {
                                    if (_zone.rect != null)
                                    {
                                        foreach (var _rect in _zone.rect)
                                        {
                                            polygonPoints = new PolygonPoint();
                                            polygonPoints.X = Double.Parse(_rect.left);
                                            polygonPoints.Y = Double.Parse(_rect.top);
                                            zone.PolygonPoints.Add(polygonPoints);
                                            polygonPoints = new PolygonPoint();
                                            polygonPoints.X = Double.Parse(_rect.right);
                                            polygonPoints.Y = Double.Parse(_rect.top);
                                            zone.PolygonPoints.Add(polygonPoints);
                                            polygonPoints = new PolygonPoint();
                                            polygonPoints.X = Double.Parse(_rect.right);
                                            polygonPoints.Y = Double.Parse(_rect.bottom);
                                            zone.PolygonPoints.Add(polygonPoints);
                                            polygonPoints = new PolygonPoint();
                                            polygonPoints.X = Double.Parse(_rect.left);
                                            polygonPoints.Y = Double.Parse(_rect.bottom);
                                            zone.PolygonPoints.Add(polygonPoints);
                                        }
                                    }
                                };
                                plan.ElementZones.Add(zone);
                            }
                        }
                        index++;
                    }
                    
                    plansConfiguration.Plans.Add(plan);
                }
            }
            List<Driver> drivers = FiresecManager.Drivers;
            return plansConfiguration;
        }
    }
}
