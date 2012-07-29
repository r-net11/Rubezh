using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using XFiresecAPI;

namespace PlansModule.ViewModels
{
	public class ElementXDeviceViewModel : BaseViewModel
	{
		private ElementXDevice ElementDevice { get; set; }
		private XDevice XDevice { get; set; }
		public ElementXDeviceView ElementXDeviceView { get; private set; }
		public Guid XDeviceUID { get; private set; }
		public XDeviceState XDeviceState { get; private set; }

		public ElementXDeviceViewModel(ElementXDevice elementXDevice)
		{
			ShowInTreeCommand = new RelayCommand(OnShowInTree);
			DisableCommand = new RelayCommand(OnDisable, CanDisable);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);

			ElementDevice = elementXDevice;
			XDeviceUID = elementXDevice.XDeviceUID;
			XDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementXDevice.XDeviceUID);
			XDeviceState = XManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == elementXDevice.XDeviceUID);
			if (XDeviceState != null)
				XDeviceState.StateChanged += new Action(OnDeviceStateChanged);
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
			ElementXDeviceView._deviceControl.IsManualUpdate = true;

			ElementXDeviceView.Width = 10;
			ElementXDeviceView.Height = 10;
			Canvas.SetLeft(ElementXDeviceView, ElementDevice.Left - 5);
			Canvas.SetTop(ElementXDeviceView, ElementDevice.Top - 5);

			if (XDevice != null)
			{
				ElementXDeviceView._deviceControl.DriverId = XDevice.DriverUID;
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
			ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(XDevice.UID);
		}

		public bool CanDisable()
		{
			return XManager.CanDisable(XDeviceState);
		}

		public RelayCommand DisableCommand { get; private set; }
		void OnDisable()
		{
			if (ServiceFactory.SecurityService.Validate())
				XManager.ChangeDisabled(XDeviceState);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceDetailsEvent>().Publish(XDevice.UID);
		}

		void OnDeviceStateChanged()
		{
			OnPropertyChanged("DeviceState");
			ElementXDeviceView._deviceControl.StateType = XDeviceState.StateType;
			ElementXDeviceView._deviceControl.AdditionalStateCodes = new List<string>(
				from state in XDeviceState.States
				select state.ToString());
			ElementXDeviceView._deviceControl.Update();

			UpdateTooltip();
		}

		string _toolTip;
		public string ToolTip
		{
			get { return _toolTip; }
			set
			{
				_toolTip = value;
				OnPropertyChanged("ToolTip");
			}
		}

		void UpdateTooltip()
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.Append(XDevice.PresentationAddressDriver);
			stringBuilder.Append(" - ");
			stringBuilder.Append(XDevice.Driver.ShortName);
			stringBuilder.Append("\n");

			//if (XDeviceState.ParentStringStates != null)
			//    foreach (var parentState in XDeviceState.ParentStringStates)
			//        stringBuilder.AppendLine(parentState);

			foreach (var state in XDeviceState.States)
				stringBuilder.AppendLine(state.ToString());

			//if (XDeviceState.Parameters != null)
			//{
			//    var nullString = "<NULL>";
			//    foreach (var parameter in XDeviceState.Parameters.Where(x => x.Visible && string.IsNullOrEmpty(x.Value) == false && x.Value != nullString))
			//    {
			//        stringBuilder.Append(parameter.Caption);
			//        stringBuilder.Append(" - ");
			//        stringBuilder.AppendLine(parameter.Value);
			//    }
			//}

			ToolTip = stringBuilder.ToString();
		}
	}
}