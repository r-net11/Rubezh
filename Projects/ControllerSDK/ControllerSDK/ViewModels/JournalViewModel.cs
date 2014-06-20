using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using ChinaSKDDriverNativeApi;
using System.Runtime.InteropServices;
using ChinaSKDDriver;
using ControllerSDK.Views;
using ChinaSKDDriverAPI;
using System.Threading;
using System.Diagnostics;

namespace ControllerSDK.ViewModels
{
	public class JournalViewModel : BaseViewModel
	{
		public JournalViewModel()
		{
			StartCommand = new RelayCommand(OnStart);
			GetJournalItemIndexCommand = new RelayCommand(OnGetJournalItemIndex);
			JournalItems = new ObservableCollection<JournalItemViewModel>();
			MainViewModel.Wrapper.NewJournalItem += new Action<SKDJournalItem>(Wrapper_NewJournalItem);
		}

		void Wrapper_NewJournalItem(SKDJournalItem journalItem)
		{
			Trace.WriteLine("SKDJournalItem " + journalItem.DeviceDateTime.ToString());
			//return;
			Dispatcher.BeginInvoke(new Action(() =>
			{
				var journalItemViewModel = new JournalItemViewModel(journalItem);
				JournalItems.Add(journalItemViewModel);
			}));
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

		public RelayCommand StartCommand { get; private set; }
		void OnStart()
		{
			//NativeWrapper.WRAP_ProgressCallback callback = new NativeWrapper.WRAP_ProgressCallback(OnProgressCallback);
			//NativeWrapper.WRAP_StartProgress(callback);
			//NativeWrapper.WRAP_StartListen(MainViewModel.Wrapper.LoginID);

			////MainViewModel.Wrapper.StartListen();

			//var thread = new Thread(RunMonitoring);
			//thread.Start();

			MainViewModel.Wrapper.NewJournalItem += new Action<SKDJournalItem>(Wrapper_NewJournalItem);
			MainViewModel.Wrapper.StartWatcher();
		}

		public RelayCommand GetJournalItemIndexCommand { get; private set; }
		void OnGetJournalItemIndex()
		{
			//var index = NativeWrapper.WRAP_GetLastIndex();
			//Trace.WriteLine("Index = " + index);

			//var journalItem = new NativeWrapper.WRAP_JournalItem();
			//NativeWrapper.WRAP_GetJournalItem(0, ref journalItem);
		}

		//void OnProgressCallback(int index)
		//{
		//    Trace.WriteLine("OnProgressCallback " + index);
		//}

		//void RunMonitoring()
		//{
		//    var lastIndex = -1;
		//    while (true)
		//    {
		//        Thread.Sleep(100);
		//        var index = NativeWrapper.WRAP_GetLastIndex();
		//        if (index > lastIndex)
		//        {
		//            for (int i = lastIndex + 1; i <= index; i++ )
		//            {
		//                var wrapJournalItem = new NativeWrapper.WRAP_JournalItem();
		//                NativeWrapper.WRAP_GetJournalItem(i, ref wrapJournalItem);
		//                var deteTime = Wrapper.NET_TIMEToDateTime(wrapJournalItem.DeviceDateTime);
		//                Trace.WriteLine("New journal item " + deteTime.ToString());

		//                Dispatcher.BeginInvoke(new Action(() =>
		//                {
		//                    var journalItem = new SKDJournalItem();
		//                    journalItem.SystemDateTime = DateTime.Now;
		//                    journalItem.DeviceDateTime = DateTime.Now;
		//                    journalItem.Name = "New Journal Event";
		//                    var journalItemViewModel = new JournalItemViewModel(journalItem);
		//                    JournalItems.Add(journalItemViewModel);
		//                }));
		//            }
		//            lastIndex = index;
		//        }
		//    }
		//}
	}
}