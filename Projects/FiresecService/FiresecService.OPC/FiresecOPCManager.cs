using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graybox.OPC.ServerToolkit.CLRWrapper;
using System.Threading;

namespace FiresecService.OPC
{
	public static class FiresecOPCManager
	{
		static Thread thread;
		static OPCDAServer srv = new OPCDAServer();
		static List<int> tagIds = new List<int>();

		public static void Start()
		{
			thread = new Thread(new ThreadStart(OnRun));
			thread.ApartmentState = ApartmentState.MTA;
			thread.Start();
		}

		public static void Stop()
		{
			thread.Abort();
			if (srv != null)
			{
				srv.RevokeClassObject();
			}
		}

		static void OnRun()
		{
			try
			{
				Run();
			}
			catch
			{
			}
		}

		static void Run()
		{
			Guid srvGuid = new Guid("FBBF742D-3077-40F4-9877-C97F5EE4CE0E");

			OPCDAServer.RegisterServer(
				srvGuid,
				"Rubezh",
				"FiresecOPC",
				"Rubezh.FiresecOPC",
				"1.0");

			//OPCDAServer.UnregisterServer(srvGuid);


			srv.Events.ServerReleased += new ServerReleasedEventHandler(Events_ServerReleased);
			srv.Events.ReadItems += new ReadItemsEventHandler(Events_ReadItems);
			srv.Events.WriteItems += new WriteItemsEventHandler(Events_WriteItems);
			srv.Events.ActivateItems += new ActivateItemsEventHandler(Events_ActivateItems);
			srv.Events.DeactivateItems += new DeactivateItemsEventHandler(Events_DeactivateItems);

			srv.Initialize(srvGuid, 50, 50, ServerOptions.NoAccessPaths, '.', 100);

			for (int i = 0; i < 5; i++)
			{
				int tagId;
				// Create an OPC tag for the holding register.
				tagId = srv.CreateTag(
					tagIds.Count,
					"Device.HoldingRegisters.Reg" + i.ToString(),
					AccessRights.readable,
					(double)0);
			}

			srv.RegisterClassObject();
			while (true)
			{
				Thread.Sleep(500);
				srv.BeginUpdate();
				foreach (var tagId in tagIds)
				{
					srv.SetTag(tagId, 0);
				}
				srv.EndUpdate(false);
			}
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
		}
	}
}