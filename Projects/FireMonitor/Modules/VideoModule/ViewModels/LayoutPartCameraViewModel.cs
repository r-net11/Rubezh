﻿using System.Linq;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using RubezhClient;
using Infrastructure.Common.Windows.ViewModels;
using RviClient;
using System.Net;

namespace VideoModule.ViewModels
{
	public class LayoutPartCameraViewModel : BaseViewModel
	{
		public Camera Camera { get; set; }
		public LayoutPartCameraViewModel()
		{
		}
		public LayoutPartCameraViewModel(LayoutPartReferenceProperties properties)
		{
			if (properties != null)
			{
				Camera = ClientManager.SystemConfiguration.Cameras.FirstOrDefault(item => item.UID == properties.ReferenceUID);
			}
		}
		public bool PrepareToTranslation(out IPEndPoint ipEndPoint, out int vendorId)
		{
			if (Camera != null)
			{
				return RviClientHelper.PrepareToTranslation(ClientManager.SystemConfiguration, Camera, out ipEndPoint, out vendorId);
			}
			ipEndPoint = null;
			vendorId = int.MinValue;
			return false;
		}
	}
}