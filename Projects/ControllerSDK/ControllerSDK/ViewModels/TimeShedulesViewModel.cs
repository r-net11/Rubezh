using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using System.Windows;
using ControllerSDK.Views;
using ControllerSDK.SDK;
using System.Diagnostics;

namespace ControllerSDK.ViewModels
{
	public class TimeShedulesViewModel : BaseViewModel
	{
		public TimeShedulesViewModel()
		{
			GetTimeShedulesCommand = new RelayCommand(OnGetTimeShedules);
			SetTimeShedulesCommand = new RelayCommand(OnSetTimeShedules);
		}

		public RelayCommand GetTimeShedulesCommand { get; private set; }
		void OnGetTimeShedules()
		{
			var timeShedules = SDKWrapper.GetTimeShedules(MainWindow.LoginID);

			foreach (var timeShedule in timeShedules)
			{
				Trace.WriteLine(timeShedule.Mask + " " + timeShedule.BeginHours + " " + timeShedule.BeginMinutes + " " + timeShedule.BeginSeconds + " " + timeShedule.EndHours + " " + timeShedule.EndMinutes + " " + timeShedule.EndSeconds);
			}

			return;
			//MessageBox.Show(result.ToString());
		}

		public RelayCommand SetTimeShedulesCommand { get; private set; }
		void OnSetTimeShedules()
		{
			//var result = SDKImport.WRAP_DevCtrl_CloseDoor(MainWindow.LoginID);
			//MessageBox.Show(result.ToString());
		}
	}
}