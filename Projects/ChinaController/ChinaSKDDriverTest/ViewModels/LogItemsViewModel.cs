using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using System.Windows;
using ChinaSKDDriverNativeApi;
using ChinaSKDDriverAPI;

namespace ControllerSDK.ViewModels
{
	public class LogItemsViewModel : BaseViewModel
	{
		public LogItemsViewModel()
		{
			GetLogsCountCommand = new RelayCommand(OnGetLogsCount);
			QueryLogListCommand = new RelayCommand(OnQueryLogList);
			GetAllLogsCommand = new RelayCommand(OnGetAllLogs);
		}

		public RelayCommand GetLogsCountCommand { get; private set; }
		void OnGetLogsCount()
		{
			var result = MainViewModel.Wrapper.GetLogsCount();
			if (result > 0)
			{
				MessageBox.Show("LogsCount = " + result.ToString());
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		public RelayCommand QueryLogListCommand { get; private set; }
		void OnQueryLogList()
		{
			NativeWrapper.WRAP_Dev_QueryLogList_Result outResult;
			var result = NativeWrapper.WRAP_QueryLogList(MainViewModel.Wrapper.LoginID, out outResult);
			List<DeviceJournalItem> deviceJournalItems = new List<DeviceJournalItem>();
			if (result)
			{
				foreach (var log in outResult.Logs)
				{
					var deviceJournalItem = new DeviceJournalItem();
					//deviceJournalItem.DateTime = new DateTime(log.stuOperateTime.year, log.stuOperateTime.month, log.stuOperateTime.day, log.stuOperateTime.hour, log.stuOperateTime.minute, log.stuOperateTime.second);
					deviceJournalItem.OperatorName = log.szOperator;
					deviceJournalItem.Name = log.szOperation;
					deviceJournalItem.Description = log.szDetailContext;
					deviceJournalItems.Add(deviceJournalItem);
				}
			}
			if (result)
			{
				var text = "";
				foreach (var deviceJournalItem in deviceJournalItems)
				{
					text += "\n";
					text += "DateTime = " + deviceJournalItem.DateTime.ToString() + "\n";
					text += "OperatorName = " + deviceJournalItem.OperatorName.ToString() + "\n";
					text += "Name = " + deviceJournalItem.Name.ToString() + "\n";
					text += "Description = " + deviceJournalItem.Description.ToString() + "\n";
				}
				MessageBox.Show(text);
			}
			else
			{
				MessageBox.Show("Error");
			}
		}

		public RelayCommand GetAllLogsCommand { get; private set; }
		void OnGetAllLogs()
		{
			var result = MainViewModel.Wrapper.GetAllLogs();
		}
	}
}