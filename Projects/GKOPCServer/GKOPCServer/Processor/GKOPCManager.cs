using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Graybox.OPC.ServerToolkit.CLRWrapper;
using XFiresecAPI;

namespace GKOPCServer
{
	public static class GKOPCManager
	{
		static Guid srvGuid = new Guid("C8F9CDB2-5BD7-4369-8DEC-4514CE236DF5");
		static Thread thread;
		public static OPCDAServer OPCDAServer { get; private set; }
		static List<TagDevice> TagDevices = new List<TagDevice>();
		static List<TagZone> TagZones = new List<TagZone>();
		static List<TagDirection> TagDirections = new List<TagDirection>();
		static int TagsCount = 0;
		static AutoResetEvent StopEvent;

		public static void Start()
		{
			//OPCRegister();
			try
			{
				StopEvent = new AutoResetEvent(false);
				thread = new Thread(new ThreadStart(OnRun));
				thread.Name = "GKOPCManager";
				thread.SetApartmentState(ApartmentState.MTA);
				thread.Start();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове GKOPCManager.Start");
			}
		}

		public static void Stop()
		{
			StopEvent.Set();
		}

		public static void OPCRefresh()
		{
			Stop();
			Start();
		}

		static void OnRun()
		{
			try
			{
				Run();
			}
			catch (Exception e)
			{
#if DEBUG
				MessageBox.Show(e.ToString());
#endif
				Logger.Error(e, "Исключение при вызове GKOPCManager.OnRun");
			}
		}

		static void Run()
		{
			OPCDAServer = new OPCDAServer();
			OPCDAServer.Events.ServerReleased += new ServerReleasedEventHandler(Events_ServerReleased);
			OPCDAServer.Events.ReadItems += new ReadItemsEventHandler(Events_ReadItems);
			OPCDAServer.Events.WriteItems += new WriteItemsEventHandler(Events_WriteItems);
			OPCDAServer.Events.ActivateItems += new ActivateItemsEventHandler(Events_ActivateItems);
			OPCDAServer.Events.DeactivateItems += new DeactivateItemsEventHandler(Events_DeactivateItems);

			OPCDAServer.Initialize(srvGuid, 50, 50, ServerOptions.NoAccessPaths, '/', 100);

			if (FiresecManager.Devices == null)
				return;

			foreach (var device in XManager.Devices)
			{
				if (TagsCount >= 100)
					break;

				if (!device.IsOPCUsed)
					continue;

				var tagName = new StringBuilder();
				foreach (var parentDevice in device.AllParents)
				{
					if (parentDevice.Driver.DriverType != XDriverType.System)
					{
						if (parentDevice.Driver.HasAddress)
							tagName.Append(parentDevice.ShortName + " - " + parentDevice.PresentationAddress + "/");
						else
							tagName.Append(parentDevice.ShortName + "/");
					}
				}
				if (device.Driver.HasAddress)
					tagName.Append(device.ShortName + " - " + device.PresentationAddress + "/");
				else
					tagName.Append(device.ShortName + "/");

				var name = tagName.ToString();
				var tagId = OPCDAServer.CreateTag(
					TagsCount,
					name,
					AccessRights.readable,
					(double)0);

				var tagDevice = new TagDevice(tagId, device.State);
				TagDevices.Add(tagDevice);
				TagsCount++;
			}
			foreach (var zone in (from XZone zone in XManager.Zones orderby zone.No select zone))
			{
				if (TagsCount >= 100)
					break;

				if (!zone.IsOPCUsed)
					continue;

				var tagId = OPCDAServer.CreateTag(
					TagsCount,
					"Zones/" + zone.PresentationName,
					AccessRights.readable,
					(double)0);

				var tagZone = new TagZone(tagId, zone.State);
				TagZones.Add(tagZone);
				TagsCount++;
			}

			foreach (var direction in (from XDirection direction in XManager.Directions orderby direction.No select direction))
			{
				if (TagsCount >= 100)
					break;

				if (!direction.IsOPCUsed)
					continue;

				var tagId = OPCDAServer.CreateTag(
					TagsCount,
					"Directions/" + direction.PresentationName,
					AccessRights.readable,
					(double)0);

				var tagDirection = new TagDirection(tagId, direction.State);
				TagDirections.Add(tagDirection);
				TagsCount++;
			}

			OPCDAServer.RegisterClassObject();

			OPCDAServer.BeginUpdate();
			foreach (var tagDevice in TagDevices)
			{
				OPCDAServer.SetTag(tagDevice.TagId, tagDevice.State.StateClass);
			}
			foreach (var tagZone in TagZones)
			{
				OPCDAServer.SetTag(tagZone.TagId, tagZone.ZoneState.StateClass);
			}
			foreach (var tagDirection in TagDirections)
			{
				OPCDAServer.SetTag(tagDirection.TagId, tagDirection.State.StateClass);
			}
			OPCDAServer.EndUpdate(false);

			while (true)
			{
				if (StopEvent.WaitOne(5000))
					break;
			}
			OPCDAServer.RevokeClassObject();
			OPCDAServer = null;
		}

		static void Events_DeactivateItems(object sender, DeactivateItemsArgs e)
		{
		}

		static void Events_ActivateItems(object sender, ActivateItemsArgs e)
		{
		}

		static void Events_WriteItems(object sender, WriteItemsArgs e)
		{
		}

		static void Events_ReadItems(object sender, ReadItemsArgs e)
		{
		}

		static void Events_ServerReleased(object sender, ServerReleasedArgs e)
		{
			e.Suspend = false;
		}

		public static void OPCRegister()
		{
			OPCDAServer.RegisterServer(
				srvGuid,
				"Rubezh",
				"GKFiresecOPC",
				"Rubezh.GKFiresecOPC",
				"1.0");
		}

		public static void OPCUnRegister()
		{
			OPCDAServer.UnregisterServer(srvGuid);
		}
	}
}