using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Media;
using Common;
using Firesec.Models.Plans;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Services;

namespace Firesec
{
	public partial class ConfigurationConverter
	{
		PlansConfiguration ConvertPlans(surfaces innerPlans, DeviceConfiguration deviceConfiguration)
		{
			var plansConfiguration = new PlansConfiguration();

			if ((innerPlans != null) && (innerPlans.surface != null))
			{
				foreach (var innerPlan in innerPlans.surface)
				{
					var plan = new Plan()
					{
						Caption = innerPlan.caption,
						Height = Double.Parse(innerPlan.height) * 10,
						Width = Double.Parse(innerPlan.width) * 10
					};

					foreach (var innerLayer in innerPlan.layer)
					{
						if (innerLayer.elements == null)
							continue;

						switch (innerLayer.name)
						{
							case "План":
								foreach (var innerElement in innerLayer.elements)
								{
									switch (innerElement.@class)
									{
										case "TSCDePicture":
											int pictureIndex = 0;
											AddPictire(plan, innerElement, ref pictureIndex);
											break;

										case "TSCDeRectangle":
											AddRectangle(plan, innerElement);
											break;

										case "TSCDeEllipse":
											AddEllipse(plan, innerElement);
											break;

										case "TSCDeLabel":
											AddLabel(plan, innerElement);
											break;

										case "TSCDeText":
											AddLabel(plan, innerElement, true);
											break;

										case "TSCDePolyLine":
											AddPolyLine(plan, innerElement);
											break;

										case "TSCDePolygon":
											AddPolygon(plan, innerElement);
											break;

										default:
											Logger.Error("ConfigurationConverter.ConvertPlans: Неизвестный элемент " + innerElement.@class);
											break;
									}
								}
								break;
							case "Несвязанные зоны":
							case "Пожарные зоны":
							case "Охранные зоны":
							case "Зоны":
								foreach (var innerElement in innerLayer.elements)
								{
									Zone activeZone = null;


									long longId = long.Parse(innerElement.id);
									int intId = (int)longId;
									foreach (var zone in deviceConfiguration.Zones)
										foreach (var zoneShapeId in zone.ShapeIds)
											if ((zoneShapeId == longId.ToString()) || (zoneShapeId == intId.ToString()))
												activeZone = zone;

									switch (innerElement.@class)
									{
										case "TFS_PolyZoneShape":
											AddPolygonZone(plan, innerElement, activeZone);
											break;

										case "TFS_ZoneShape":
											AddRectangleZone(plan, innerElement, activeZone);
											break;

										case "TSCDeRectangle":
											AddRectangle(plan, innerElement);
											break;

										case "TSCDeEllipse":
											AddEllipse(plan, innerElement);
											break;

										case "TSCDeLabel":
											AddLabel(plan, innerElement);
											break;

										case "TSCDeText":
											AddLabel(plan, innerElement, true);
											break;

										case "TSCDePolyLine":
											AddPolyLine(plan, innerElement);
											break;

										case "TSCDePolygon":
											AddPolygon(plan, innerElement);
											break;

										default:
											Logger.Error("ConfigurationConverter.ConvertPlans: Неизвестный элемент " + innerElement.@class);
											break;
									}
								}
								break;

							case "Устройства":
								foreach (var innerElement in innerLayer.elements)
								{
									AddDevice(plan, innerElement, deviceConfiguration);
								}
								break;
						}
					}
					plansConfiguration.Plans.Add(plan);
				}
			}

			var picturesDirectory = GetPicturesDirectory();
			if (picturesDirectory != null)
			{
				DeleteDirectory(Environment.CurrentDirectory + "\\Pictures");
			}
			return plansConfiguration;
		}

		void AddRectangle(Plan plan, surfacesSurfaceLayerElementsElement innerElement)
		{
			try
			{
				var elementRectangle = new ElementRectangle()
				{
					Left = Parse(innerElement.rect[0].left),
					Top = Parse(innerElement.rect[0].top),
					Height = Parse(innerElement.rect[0].bottom) - Parse(innerElement.rect[0].top),
					Width = Parse(innerElement.rect[0].right) - Parse(innerElement.rect[0].left)
				};
				plan.ElementRectangles.Add(elementRectangle);
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigurationConverter.AddRectangle");
			}
		}

		void AddEllipse(Plan plan, surfacesSurfaceLayerElementsElement innerElement)
		{
			try
			{
				var elementEllipse = new ElementEllipse()
				{
					Left = Parse(innerElement.rect[0].left),
					Top = Parse(innerElement.rect[0].top),
					Height = Parse(innerElement.rect[0].bottom) - Parse(innerElement.rect[0].top),
					Width = Parse(innerElement.rect[0].right) - Parse(innerElement.rect[0].left)
				};
				plan.ElementEllipses.Add(elementEllipse);
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigurationConverter.AddEllipse");
			}
		}

		void AddLabel(Plan plan, surfacesSurfaceLayerElementsElement innerElement, bool stretch = false)
		{
			try
			{
				var elementTextBlock = new ElementTextBlock()
				{
					Text = innerElement.caption,
					Left = Parse(innerElement.rect[0].left),
					Top = Parse(innerElement.rect[0].top),
					Height = Parse(innerElement.rect[0].bottom) - Parse(innerElement.rect[0].top),
					Width = Parse(innerElement.rect[0].right) - Parse(innerElement.rect[0].left),
					Stretch = true,
					FontBold = false,
					FontItalic = false,
				};

				FontFamily fontFamily = new FontFamily("Arial");
				double fontDpiSize = elementTextBlock.Height / 2;
				double fontHeight = Math.Ceiling(fontDpiSize * fontFamily.LineSpacing);
				elementTextBlock.FontSize = fontHeight;

				if (innerElement.brush != null)
					try
					{
						elementTextBlock.BackgroundColor = ConvertColor(innerElement.brush[0].color);
					}
					catch (Exception e)
					{
						Logger.Error(e, "Исключение при вызове ConfigurationConverter.ConvertPlans");
					}

				if (innerElement.pen != null)
					try
					{
						elementTextBlock.ForegroundColor = ConvertColor(innerElement.pen[0].color);
					}
					catch (Exception e)
					{
						Logger.Error(e, "Исключение при вызове ConfigurationConverter.ConvertPlans innerElementLayer.pen");
					}

				plan.ElementTextBlocks.Add(elementTextBlock);
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigurationConverter.AddLabel");
			}
		}

		void AddPolyLine(Plan plan, surfacesSurfaceLayerElementsElement innerElement)
		{
			try
			{
				var elementPolyline = new ElementPolyline();
				elementPolyline.Points = GetPointCollection(innerElement);
				//elementPolyline.Normalize();
				plan.ElementPolylines.Add(elementPolyline);
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigurationConverter.AddPolyLine");
			}
		}

		void AddPolygon(Plan plan, surfacesSurfaceLayerElementsElement innerElement)
		{
			try
			{
				var elementPolygon = new ElementPolygon();
				elementPolygon.Points = GetPointCollection(innerElement);
				//elementPolygon.Normalize();
				plan.ElementPolygons.Add(elementPolygon);
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigurationConverter.AddPolygon");
			}
		}

		void AddDevice(Plan plan, surfacesSurfaceLayerElementsElement innerElement, DeviceConfiguration deviceConfiguration)
		{
			try
			{
				if (innerElement.rect != null)
				{
					var innerRect = innerElement.rect[0];

					long longId = long.Parse(innerElement.id);
					int intId = (int)longId;

					var height = Parse(innerRect.bottom) - Parse(innerRect.top);
					var width = Parse(innerRect.right) - Parse(innerRect.left);
					var elementDevice = new ElementDevice()
					{
						Left = Parse(innerRect.left) + height / 2,
						Top = Parse(innerRect.top) + width / 2
					};
					plan.ElementDevices.Add(elementDevice);

					foreach (var device in deviceConfiguration.Devices)
					{
						foreach (var deviceShapeId in device.ShapeIds)
						{
							if ((deviceShapeId == longId.ToString()) || (deviceShapeId == intId.ToString()))
							{
								elementDevice.DeviceUID = device.UID;
								device.PlanElementUIDs.Add(elementDevice.UID);
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigurationConverter.AddPolygonZone");
			}
		}

		void AddPolygonZone(Plan plan, surfacesSurfaceLayerElementsElement innerElement, Zone zone)
		{
			try
			{
				if (innerElement.points != null)
				{
					var elementPolygonZone = new ElementPolygonZone()
					{
						ZoneUID = zone == null ? Guid.Empty : zone.UID,
					};
					elementPolygonZone.Points = GetPointCollection(innerElement);
					//elementPolygonZone.Normalize();
					plan.ElementPolygonZones.Add(elementPolygonZone);
					if (zone != null)
					{
						if (zone.PlanElementUIDs == null)
							zone.PlanElementUIDs = new List<Guid>();
						zone.PlanElementUIDs.Add(elementPolygonZone.UID);
					}
				};
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigurationConverter.AddPolygonZone");
			}
		}

		void AddRectangleZone(Plan plan, surfacesSurfaceLayerElementsElement innerElement, Zone zone)
		{
			try
			{
				var elementRectangleZone = new ElementRectangleZone()
				{
					ZoneUID = zone == null ? Guid.Empty : zone.UID,
					Left = Math.Min(Parse(innerElement.rect[0].left), Parse(innerElement.rect[0].right)),
					Top = Math.Min(Parse(innerElement.rect[0].top), Parse(innerElement.rect[0].bottom)),
					Width = Math.Abs(Parse(innerElement.rect[0].right) - Parse(innerElement.rect[0].left)),
					Height = Math.Abs(Parse(innerElement.rect[0].bottom) - Parse(innerElement.rect[0].top))
				};
				plan.ElementRectangleZones.Add(elementRectangleZone);
				if (zone != null)
				{
					if (zone.PlanElementUIDs == null)
						zone.PlanElementUIDs = new List<Guid>();
					zone.PlanElementUIDs.Add(elementRectangleZone.UID);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigurationConverter.AddRectangleZone");
			}
		}

		void AddPictire(Plan plan, surfacesSurfaceLayerElementsElement innerElement, ref int pictureIndex)
		{
			try
			{
				if (innerElement.picture == null)
					return;

				foreach (var innerPicture in innerElement.picture)
				{
					if (string.IsNullOrEmpty(innerPicture.idx))
						innerPicture.idx = pictureIndex++.ToString();

					var picturesDirectory = GetPicturesDirectory();
					if (picturesDirectory == null)
						continue;
					var directoryInfo = new DirectoryInfo(picturesDirectory + "\\Sample" + innerPicture.idx + "." + innerPicture.ext);
					if (File.Exists(directoryInfo.FullName) == false)
						continue;

					if (innerPicture.ext == "emf")
					{
						var metafile = new Metafile(directoryInfo.FullName);
						innerPicture.ext = "bmp";
						directoryInfo = new DirectoryInfo(picturesDirectory + "\\Sample" + innerPicture.idx + "." + innerPicture.ext);
						metafile.Save(directoryInfo.FullName, ImageFormat.Bmp);
						metafile.Dispose();
					}

					var guid = ServiceFactoryBase.ContentService.AddContent(directoryInfo.FullName);
					var elementRectanglePicture = new ElementRectangle()
					{
						Left = Parse(innerElement.rect[0].left),
						Top = Parse(innerElement.rect[0].top),
						Height = Parse(innerElement.rect[0].bottom) - Parse(innerElement.rect[0].top),
						Width = Parse(innerElement.rect[0].right) - Parse(innerElement.rect[0].left),
					};

					if ((elementRectanglePicture.Left == 0) && (elementRectanglePicture.Top == 0) && (elementRectanglePicture.Width == plan.Width) && (elementRectanglePicture.Height == plan.Height))
					{
						plan.BackgroundImageSource = guid;
						plan.BackgroundSourceName = directoryInfo.FullName;
					}
					else
					{
						elementRectanglePicture.BackgroundImageSource = guid;
						elementRectanglePicture.BackgroundSourceName = directoryInfo.FullName;
						plan.ElementRectangles.Add(elementRectanglePicture);
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigurationConverter.AddPictire");
			}
		}

		static void DeleteDirectory(string directoryName)
		{
			try
			{
				if (Directory.Exists(directoryName))
				{
					foreach (string file in Directory.GetFiles(directoryName))
					{
						File.SetAttributes(file, FileAttributes.Normal);
						File.Delete(file);
					}
					Directory.Delete(directoryName, true);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ConfigurationConverter.DeleteDirectory");
			}
		}

		static PointCollection GetPointCollection(surfacesSurfaceLayerElementsElement innerElementLayer)
		{
			var pointCollection = new PointCollection();
			try
			{
				foreach (var innerPoint in innerElementLayer.points)
				{
					var point = new System.Windows.Point()
					{
						X = Parse(innerPoint.x),
						Y = Parse(innerPoint.y)
					};

					pointCollection.Add(point);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigurationConverter.GetPointCollection");
			}
			return pointCollection;
		}

		static Color ConvertColor(string stringColor)
		{
			try
			{
				if (string.IsNullOrEmpty(stringColor))
					return Colors.Transparent;

				if (stringColor.StartsWith("cl"))
				{
					stringColor = stringColor.Remove(0, 2);
				}
				if (stringColor.StartsWith("$"))
				{
					stringColor = stringColor.Replace("$", "#");
				}
				var color = (Color)ColorConverter.ConvertFromString(stringColor);
				return color;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ConfigurationConverter.ConvertColor stringColor = " + stringColor);
			}
			return Colors.Transparent;
		}

		static Double Parse(string input)
		{
			Double result;
			try
			{
				var index = input.IndexOf(".");
				if (index >= 0)
					input = input.Substring(0, index);

				index = input.IndexOf(",");
				if (index >= 0)
					input = input.Substring(0, index);

				result = Double.Parse(input, NumberStyles.Float, CultureInfo.InvariantCulture);
				return result;
			}
			catch
			{
				input = input.Replace(",", ".");
				result = Double.Parse(input);
				return result;
			}
		}

		string GetPicturesDirectory()
		{
			try
			{
				var agentLocation = FSAgentLoadHelper.GetLocation();
				if (agentLocation != null)
				{
					var fileInfo = new FileInfo(agentLocation);
					return fileInfo.DirectoryName + "\\Pictures";
				}
				return null;
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigurationConverter.GetPicturesDirectory");
				return null;
			}
		}
	}
}