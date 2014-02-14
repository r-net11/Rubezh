using System;
using System.Collections.Generic;
using System.Windows.Media;
using FiresecAPI.Models;
using FiresecClient;
using Infrustructure.Plans.Elements;

namespace VideoModule.Plans.Designer
{
	public static class Helper
	{
		private static Dictionary<Guid, Camera> _cameraMap;
		public static void BuildMap()
		{
			BuildCameraMap();
		}
		public static void BuildCameraMap()
		{
			_cameraMap = new Dictionary<Guid, Camera>();
			FiresecManager.SystemConfiguration.Cameras.ForEach(item => _cameraMap.Add(item.UID, item));
		}

		public static Camera GetCamera(ElementCamera element)
		{
			return element.CameraUID != Guid.Empty && _cameraMap.ContainsKey(element.CameraUID) ? _cameraMap[element.CameraUID] : null;
		}
		public static void SetCamera(ElementCamera element, Camera camera)
		{
			ResetCamera(element);
			element.CameraUID = camera == null ? Guid.Empty : camera.UID;
			if (camera != null)
				camera.PlanElementUIDs.Add(element.UID);
		}
		public static Camera SetCamera(ElementCamera element)
		{
			var camera = GetCamera(element);
			SetCamera(element, camera);
			return camera;
		}
		public static void ResetCamera(ElementCamera element)
		{
			var camera = GetCamera(element);
			if (camera != null)
				camera.PlanElementUIDs.Remove(element.UID);
		}

		public static string GetCameraTitle(ElementCamera element)
		{
			var camera = GetCamera(element);
			return camera == null ? "Неизвестная камера" : camera.Name;
		}
	}
}