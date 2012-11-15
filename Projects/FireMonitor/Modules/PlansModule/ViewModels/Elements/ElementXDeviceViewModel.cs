using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using XFiresecAPI;
using PlansModule.ViewModels.Elements;

namespace PlansModule.ViewModels
{
	public class ElementXDeviceViewModel : BaseViewModel, IElementDevice
	{
		public ElementXDevice ElementDevice { get; private set; }
		private XDevice XDevice { get; set; }
		public ElementXDeviceView ElementXDeviceView { get; private set; }
		public Guid XDeviceUID { get; private set; }
		public XDeviceState DeviceState { get; private set; }

		public ElementXDeviceViewModel(ElementXDevice elementXDevice)
		{
			ShowInTreeCommand = new RelayCommand(OnShowInTree);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);

			ElementDevice = elementXDevice;
			XDeviceUID = elementXDevice.XDeviceUID;
			XDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementXDevice.XDeviceUID);
			if (XDevice != null)
			{
				DeviceState = XDevice.DeviceState;
				DeviceState.StateChanged += new Action(OnDeviceStateChanged);
			}
		}

		public Point Location
		{
			get { return new Point(ElementDevice.Left, ElementDevice.Top); }
		}

		public void DrawElementXDevice()
		{
			if (ElementXDeviceView != null)
				return;

			ElementXDeviceView = new ElementXDeviceView()
			{
				DataContext = this,
			};

			ElementXDeviceView.Width = 10;
			ElementXDeviceView.Height = 10;
			Canvas.SetLeft(ElementXDeviceView, ElementDevice.Left - 5);
			Canvas.SetTop(ElementXDeviceView, ElementDevice.Top - 5);

			if (XDevice != null)
			{
				ElementXDeviceView._xDeviceControl.XDriverId = XDevice.DriverUID;
				OnDeviceStateChanged();
			}
		}

		bool _isSelected;
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				DrawElementXDevice();
				ElementXDeviceView._selectationRectangle.StrokeThickness = value ? 1 : 0;
				OnPropertyChanged("IsSelected");

				if (value)
				{
					ElementXDeviceView.Flush();
				}
			}
		}

		public RelayCommand ShowInTreeCommand { get; private set; }
		void OnShowInTree()
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(XDevice.UID);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Publish(XDevice.UID);
		}

		void OnDeviceStateChanged()
		{
			OnPropertyChanged("DeviceState");
			OnPropertyChanged("ToolTip");
			ElementXDeviceView._xDeviceControl.XStateClass = DeviceState.StateClass;
			ElementXDeviceView._xDeviceControl.Update();
		}

		public string ToolTip
		{
			get
			{
				var stringBuilder = new StringBuilder();
				stringBuilder.Append(XDevice.PresentationAddressAndDriver);
				stringBuilder.Append(" - ");
				stringBuilder.Append(XDevice.Driver.ShortName);
				stringBuilder.Append("\n");

				foreach (var state in DeviceState.States)
					stringBuilder.AppendLine(state.ToDescription());

				return stringBuilder.ToString();
			}
		}
	}
}