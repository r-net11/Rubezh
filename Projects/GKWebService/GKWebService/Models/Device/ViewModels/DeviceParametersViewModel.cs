using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI.GK;

namespace GKWebService.Models.ViewModels
{
	public class DeviceParametersViewModel
	{
		private GKDevice _device;

		public Guid UID
		{
			get { return _device.UID; }
		}

		public string Name
		{
			get { return _device.PresentationName; }
		}

		public string Address
		{
			get { return _device.DottedPresentationAddress; }
		}

		public string ImageSource
		{
			get { return _device.ImageSource.Replace("/Controls;component/", ""); }
		}

		public string StateIcon
		{
			get { return _device.State.StateClass.ToString(); }
		}
			
		public IEnumerable<GKMeasureParameterValue> MeasureParameterValues
		{
			get { return _device.State.XMeasureParameterValues; }
		}

		public DeviceParametersViewModel(GKDevice device)
		{
			_device = device;
		}
	}
}