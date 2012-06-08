using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using Common;
using Firesec.Plans;
using FiresecAPI.Models;

namespace FiresecService.Configuration
{
	public partial class ConfigurationConverter
	{
		PlansConfiguration ConvertPlans(surfaces innerPlans)
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
									ulong? zoneNo = null;

									long longId = long.Parse(innerElement.id);
									int intId = (int)longId;
									foreach (var zone in DeviceConfiguration.Zones)
									{
										foreach (var zoneShapeId in zone.ShapeIds)
										{
											if ((zoneShapeId == longId.ToString()) || (zoneShapeId == intId.ToString()))
											{
												zoneNo = zone.No;
											}
										}
									}

									switch (innerElement.@class)
									{
										case "TFS_PolyZoneShape":
											AddPolygonZone(plan, innerElement, zoneNo);
											break;

										case "TFS_ZoneShape":
											AddRectangleZone(plan, innerElement, zoneNo);
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
									AddDevice(plan, innerElement);
								}
								break;
						}
					}
					plansConfiguration.Plans.Add(plan);
				}
			}

			DeleteDirectory(Environment.CurrentDirectory + "\\Pictures");
			return plansConfiguration;
		}

		void AddRectangle(Plan plan, surfacesSurfaceLayerElementsElement innerElement)
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

		void AddEllipse(Plan plan, surfacesSurfaceLayerElementsElement innerElement)
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

		void AddLabel(Plan plan, surfacesSurfaceLayerElementsElement innerElement, bool stretch = false)
		{
			var elementTextBlock = new ElementTextBlock()
			{
				Text = innerElement.caption,
				Left = Parse(innerElement.rect[0].left),
				Top = Parse(innerElement.rect[0].top),
				Height = Parse(innerElement.rect[0].bottom) - Parse(innerElement.rect[0].top),
				Width = Parse(innerElement.rect[0].right) - Parse(innerElement.rect[0].left),
				Stretch = true
			};

			FontFamily fontFamily = new FontFamily("Arial");
			double fontDpiSize = elementTextBlock.Height / 2;
			double fontHeight = Math.Ceiling(fontDpiSize * fontFamily.LineSpacing);
			elementTextBlock.FontSize = fontHeight;


			if (innerElement.brush != null)
				try
				{
					elementTextBlock.BorderColor = (Color)ColorConverter.ConvertFromString(innerElement.brush[0].color);
				}
				catch (Exception e)
				{
					Logger.Error(e, "Исключение при вызове ConfigurationConverter.ConvertPlans elementTextBlock.BorderColor. Color = " + innerElement.brush[0].color);
				}

			if (innerElement.pen != null)
				try
				{
					elementTextBlock.ForegroundColor = (Color)ColorConverter.ConvertFromString(innerElement.pen[0].color);
				}
				catch (Exception e)
				{
					Logger.Error(e, "Исключение при вызове ConfigurationConverter.ConvertPlans innerElementLayer.pen. Color = " + innerElement.pen[0].color);
				}

			plan.ElementTextBlocks.Add(elementTextBlock);
		}

		void AddPolyLine(Plan plan, surfacesSurfaceLayerElementsElement innerElement)
		{
			var elementPolyline = new ElementPolyline();
			elementPolyline.PolygonPoints = GetPointCollection(innerElement);
			elementPolyline.Normalize();
			plan.ElementPolylines.Add(elementPolyline);
		}

		void AddPolygon(Plan plan, surfacesSurfaceLayerElementsElement innerElement)
		{
			var elementPolygon = new ElementPolygon();
			elementPolygon.PolygonPoints = GetPointCollection(innerElement);
			elementPolygon.Normalize();
			plan.ElementPolygons.Add(elementPolygon);
		}

		void AddDevice(Plan plan, surfacesSurfaceLayerElementsElement innerElement)
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

				foreach (var device in DeviceConfiguration.Devices)
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

		void AddPolygonZone(Plan plan, surfacesSurfaceLayerElementsElement innerElement, ulong? zoneNo)
		{
			if (innerElement.points != null)
			{
				var elementPolygonZone = new ElementPolygonZone()
				{
					ZoneNo = zoneNo
				};
				elementPolygonZone.PolygonPoints = GetPointCollection(innerElement);
				elementPolygonZone.Normalize();
				plan.ElementPolygonZones.Add(elementPolygonZone);
			};
		}

		void AddRectangleZone(Plan plan, surfacesSurfaceLayerElementsElement innerElement, ulong? zoneNo)
		{
			var elementRectangleZone = new ElementRectangleZone()
			{
				ZoneNo = zoneNo,
				Left = Math.Min(Parse(innerElement.rect[0].left), Parse(innerElement.rect[0].right)),
				Top = Math.Min(Parse(innerElement.rect[0].top), Parse(innerElement.rect[0].bottom)),
				Width = Math.Abs(Parse(innerElement.rect[0].right) - Parse(innerElement.rect[0].left)),
				Height = Math.Abs(Parse(innerElement.rect[0].bottom) - Parse(innerElement.rect[0].top))
			};

			plan.ElementRectangleZones.Add(elementRectangleZone);
		}

		void AddPictire(Plan plan, surfacesSurfaceLayerElementsElement innerElement, ref int pictureIndex)
		{
			foreach (var innerPicture in innerElement.picture)
			{
				if (string.IsNullOrEmpty(innerPicture.idx))
					innerPicture.idx = pictureIndex++.ToString();

				var directoryInfo = new DirectoryInfo(Environment.CurrentDirectory + "\\Pictures\\Sample" + innerPicture.idx + "." + innerPicture.ext);
				if (File.Exists(directoryInfo.FullName) == false)
					continue;

				if (innerPicture.ext == "emf")
				{
					var metafile = new Metafile(directoryInfo.FullName);
					innerPicture.ext = "bmp";
					directoryInfo = new DirectoryInfo(Environment.CurrentDirectory + "\\Pictures\\Sample" + innerPicture.idx + "." + innerPicture.ext);
					metafile.Save(directoryInfo.FullName, ImageFormat.Bmp);
					metafile.Dispose();
				}

				byte[] backgroundPixels = File.ReadAllBytes(directoryInfo.FullName);

				var elementRectanglePicture = new ElementRectangle()
				{
					Left = Parse(innerElement.rect[0].left),
					Top = Parse(innerElement.rect[0].top),
					Height = Parse(innerElement.rect[0].bottom) - Parse(innerElement.rect[0].top),
					Width = Parse(innerElement.rect[0].right) - Parse(innerElement.rect[0].left),
					BackgroundPixels = backgroundPixels
				};

				if ((elementRectanglePicture.Left == 0) && (elementRectanglePicture.Top == 0) && (elementRectanglePicture.Width == plan.Width) && (elementRectanglePicture.Height == plan.Height))
				{
					plan.BackgroundPixels = elementRectanglePicture.BackgroundPixels;
				}
				else
				{
					plan.ElementRectangles.Add(elementRectanglePicture);
				}
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
			foreach (var innerPoint in innerElementLayer.points)
			{
				var point = new System.Windows.Point()
				{
					X = Parse(innerPoint.x),
					Y = Parse(innerPoint.y)
				};

				pointCollection.Add(point);
			}
			return pointCollection;
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