using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using Firesec.Plans;
using FiresecAPI.Models;

namespace FiresecService.Configuration
{
	public partial class ConfigurationManager
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
													Left = Parse(innerElementLayer.rect[0].left),
													Top = Parse(innerElementLayer.rect[0].top),
													Height = Parse(innerElementLayer.rect[0].bottom) - Parse(innerElementLayer.rect[0].top),
													Width = Parse(innerElementLayer.rect[0].right) - Parse(innerElementLayer.rect[0].left),
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
											break;
									}
								}
								break;
							case "Несвязанные зоны":
							case "Пожарные зоны":
							case "Охранные зоны":
							case "Зоны":

								foreach (var innerElementLayer in innerLayer.elements)
								{
									ulong? zoneNo = null;

									long longId = long.Parse(innerElementLayer.id);
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

									switch (innerElementLayer.@class)
									{
										case "TFS_PolyZoneShape":
											if (innerElementLayer.points != null)
											{
												var elementPolygonZone = new ElementPolygonZone()
												{
													ZoneNo = zoneNo,
												};
												elementPolygonZone.PolygonPoints = GetPointCollection(innerElementLayer);
												elementPolygonZone.Normalize();
												plan.ElementPolygonZones.Add(elementPolygonZone);
											};
											break;

										case "TFS_ZoneShape":
											var elementRectangleZone = new ElementRectangleZone()
											{
												ZoneNo = zoneNo,
												Left = Math.Min(Parse(innerElementLayer.rect[0].left), Parse(innerElementLayer.rect[0].right)),
												Top = Math.Min(Parse(innerElementLayer.rect[0].top), Parse(innerElementLayer.rect[0].bottom)),
												Width = Math.Abs(Parse(innerElementLayer.rect[0].right) - Parse(innerElementLayer.rect[0].left)),
												Height = Math.Abs(Parse(innerElementLayer.rect[0].bottom) - Parse(innerElementLayer.rect[0].top))
											};

											plan.ElementRectangleZones.Add(elementRectangleZone);
											break;

										case "TSCDeRectangle":
											var elementRectangle = new ElementRectangle()
											{
												Left = Parse(innerElementLayer.rect[0].left),
												Top = Parse(innerElementLayer.rect[0].top),
												Height = Parse(innerElementLayer.rect[0].bottom) - Parse(innerElementLayer.rect[0].top),
												Width = Parse(innerElementLayer.rect[0].right) - Parse(innerElementLayer.rect[0].left),
											};
											plan.ElementRectangles.Add(elementRectangle);
											break;

										case "TSCDeEllipse":
											var elementEllipse = new ElementEllipse()
											{
												Left = Parse(innerElementLayer.rect[0].left),
												Top = Parse(innerElementLayer.rect[0].top),
												Height = Parse(innerElementLayer.rect[0].bottom) - Parse(innerElementLayer.rect[0].top),
												Width = Parse(innerElementLayer.rect[0].right) - Parse(innerElementLayer.rect[0].left),
											};
											plan.ElementEllipses.Add(elementEllipse);
											break;

										case "TSCDeLabel":
											var elementTextBlock = new ElementTextBlock()
											{
												Text = innerElementLayer.caption,
												Left = Parse(innerElementLayer.rect[0].left),
												Top = Parse(innerElementLayer.rect[0].top),
											};

											if (innerElementLayer.brush != null)
												try
												{
													elementTextBlock.BorderColor = (Color)ColorConverter.ConvertFromString(innerElementLayer.brush[0].color);
												}
												catch { ;}

											if (innerElementLayer.pen != null)
												try
												{
													elementTextBlock.ForegroundColor = (Color)ColorConverter.ConvertFromString(innerElementLayer.pen[0].color);
												}
												catch { ;}

											plan.ElementTextBlocks.Add(elementTextBlock);
											break;

										case "TSCDePolyLine":
											var elementPolyline = new ElementPolyline();
											elementPolyline.PolygonPoints = GetPointCollection(innerElementLayer);
											elementPolyline.Normalize();
											plan.ElementPolylines.Add(elementPolyline);
											break;

										case "TSCDePolygon":
											var elementPolygon = new ElementPolygon();
											elementPolygon.PolygonPoints = GetPointCollection(innerElementLayer);
											elementPolygon.Normalize();
											plan.ElementPolygons.Add(elementPolygon);
											break;
									}
								}
								break;

							case "Устройства":
								foreach (var innerDevice in innerLayer.elements)
								{
									if (innerDevice.rect != null)
									{
										var innerRect = innerDevice.rect[0];

										long longId = long.Parse(innerDevice.id);
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
								break;
						}
					}
					plansConfiguration.Plans.Add(plan);
				}
			}

			DeleteDirectory(Environment.CurrentDirectory + "\\Pictures");
			return plansConfiguration;
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
			catch { return; }
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