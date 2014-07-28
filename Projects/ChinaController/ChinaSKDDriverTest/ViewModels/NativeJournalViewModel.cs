using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using ChinaSKDDriver;
using ChinaSKDDriverAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ChinaSKDDriverNativeApi;

namespace ControllerSDK.ViewModels
{
	public class NativeJournalViewModel : BaseViewModel
	{
		public NativeJournalViewModel()
		{
			JournalItems = new ObservableCollection<JournalItemViewModel>();

			return;

			fDisConnectDelegate = new NativeWrapper.fDisConnectDelegate(OnDisConnectDelegate);
			fHaveReConnectDelegate = new NativeWrapper.fHaveReConnectDelegate(OnfHaveReConnectDelegate);
			fMessCallBackDelegate = new NativeWrapper.fMessCallBackDelegate(OnfMessCallBackDelegate);

			NativeWrapper.CLIENT_Init(fDisConnectDelegate, 0);
			NativeWrapper.CLIENT_SetAutoReconnect(fHaveReConnectDelegate, 0);
			NativeWrapper.CLIENT_SetDVRMessCallBack(fMessCallBackDelegate, 0);

			//NativeWrapper.CLIENT_StartListenEx(MainViewModel.Wrapper.LoginID);
			//NativeWrapper.CLIENT_StopListen(MainViewModel.Wrapper.LoginID);
		}

		NativeWrapper.fDisConnectDelegate fDisConnectDelegate;
		NativeWrapper.fHaveReConnectDelegate fHaveReConnectDelegate;
		NativeWrapper.fMessCallBackDelegate fMessCallBackDelegate;

		void OnDisConnectDelegate(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			var journalItem = new SKDJournalItem();
			journalItem.SystemDateTime = DateTime.Now;
			journalItem.DeviceDateTime = DateTime.Now;
			journalItem.JournalEventNameType = FiresecAPI.Journal.JournalEventNameType.Потеря_связи;
			var journalItemViewModel = new JournalItemViewModel(journalItem);
			Dispatcher.BeginInvoke(new Action(() =>
			{
				JournalItems.Add(journalItemViewModel);
			}));
		}

		void OnfHaveReConnectDelegate(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			var journalItem = new SKDJournalItem();
			journalItem.SystemDateTime = DateTime.Now;
			journalItem.DeviceDateTime = DateTime.Now;
			journalItem.JournalEventNameType = FiresecAPI.Journal.JournalEventNameType.Восстановление_связи;
			var journalItemViewModel = new JournalItemViewModel(journalItem);
			Dispatcher.BeginInvoke(new Action(() =>
			{
				JournalItems.Add(journalItemViewModel);
			}));
		}

		bool OnfMessCallBackDelegate(Int32 lCommand, Int32 lLoginID, IntPtr pBuf, UInt32 dwBufLen, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			var journalItem = new SKDJournalItem();
			journalItem.SystemDateTime = DateTime.Now;
			journalItem.DeviceDateTime = DateTime.Now;
			journalItem.JournalEventNameType = FiresecAPI.Journal.JournalEventNameType.Проход_разрешен;
			var journalItemViewModel = new JournalItemViewModel(journalItem);
			Dispatcher.BeginInvoke(new Action(() =>
			{
				JournalItems.Add(journalItemViewModel);
			}));
			return true;
		}

		public ObservableCollection<JournalItemViewModel> JournalItems { get; private set; }

		JournalItemViewModel _selectedJournalItem;
		public JournalItemViewModel SelectedJournalItem
		{
			get { return _selectedJournalItem; }
			set
			{
				_selectedJournalItem = value;
				OnPropertyChanged(() => SelectedJournalItem);
			}
		}
	}
}