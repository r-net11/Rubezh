using System;
using System.Collections.Generic;
using System.Windows;
using Entities.DeviceOriented;
using FiresecAPI.Models;

namespace Infrastructure.Common.Video.RVI_VSS
{
	public partial class CellPlayerWrap
	{
		public CellPlayerWrap()
		{
			InitializeComponent();
		}

		public List<IChannel> Connect(string ipAddress, int port, string login = "admin", string password = "admin")
		{
			return FormsPlayer.Connect(ipAddress, port, login, password);
		}

		public void Start(int channelNumber)
		{
			FormsPlayer.Start(channelNumber);
		}

		public void Start(PlayBackDeviceRecord record)
		{
			FormsPlayer.Start(record);
		}

		public bool Pause(PlayBackDeviceRecord record, bool pausePlayBack)
		{
			return FormsPlayer.Pause(record, pausePlayBack);
		}

		public void Stop()
		{
			FormsPlayer.Stop();
		}

		public void Stop(PlayBackDeviceRecord record)
		{
			FormsPlayer.Stop(record);
		}

		public void Fast(PlayBackDeviceRecord record)
		{
			FormsPlayer.Fast(record);
		}

		public void Slow(PlayBackDeviceRecord record)
		{
			FormsPlayer.Slow(record);
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			base.OnDragOver(e);
			e.Effects = e.Data.GetDataPresent("DESIGNER_ITEM") ? DragDropEffects.Move : DragDropEffects.None;
			e.Handled = true;
		}
		
		public event Action<Camera> DropHandler;
		protected override void OnDrop(DragEventArgs e)
		{
			base.OnDrop(e);
			var camera = e.Data.GetData("DESIGNER_ITEM") as Camera;
			if (camera != null)
			{
				DropHandler(camera);
				e.Handled = true;
			}
		}

		public DeviceStatuses Status
		{
			get { return FormsPlayer.Status; }
		}

	}
}