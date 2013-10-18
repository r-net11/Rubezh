using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;
using System.Diagnostics;

namespace DiagnosticsModule.ViewModels
{
	public class PerformanceViewModel : DialogViewModel
	{
		public PerformanceViewModel()
		{
			Title = "Performance";
			Items = new ObservableCollection<PerformanceItemViewModel>();
			ListItems = new ObservableCollection<PerformanceItemViewModel>();
			PublishCommand = new RelayCommand(OnPublish);
			PublishListCommand = new RelayCommand(OnPublishList);
			CleanCommand = new RelayCommand(OnClean);
			TestCommand = new RelayCommand(OnTest);
		}

		public RelayCommand PublishCommand { get; private set; }
		private void OnPublish()
		{
			var list = new List<PerformanceItemViewModel>();
			var multiple = 100;
			for (int i = 0; i < multiple; i++)
				list.AddRange(XManager.Drivers.Select(item => new PerformanceItemViewModel(item)));
			foreach (var item in Items)
				item.Dispose();
			Items = new ObservableCollection<PerformanceItemViewModel>(list);
			Update();
		}
		public RelayCommand CleanCommand { get; private set; }
		private void OnClean()
		{
			foreach (var item in Items)
				item.Dispose();
			for (int i = 0; i < 100; i++)
				Items = new ObservableCollection<PerformanceItemViewModel>();
			Update();
		}
		public RelayCommand CollectCommand { get; private set; }
		private void OnCollect()
		{
			GC.Collect();
			GC.Collect();
			GC.Collect();
			Update();
		}
		public RelayCommand TestCommand { get; private set; }
		private void OnTest()
		{
			var proc = Process.GetCurrentProcess();
		}
		public RelayCommand PublishListCommand { get; private set; }
		private void OnPublishList()
		{
			ListItems.Clear();
			var multiple = 100;
			for (int i = 0; i < multiple; i++)
				foreach (var driver in XManager.Drivers)
					ListItems.Add(new PerformanceItemViewModel(driver));
			Update();
			OnPropertyChanged(() => ListItems);
		}

		public ObservableCollection<PerformanceItemViewModel> Items { get; private set; }
		public ObservableCollection<PerformanceItemViewModel> ListItems { get; private set; }
		public int Count
		{
			get { return Items.Count; }
		}
		public string Memory
		{
			get
			{
				var proc = Process.GetCurrentProcess();
				return (proc.WorkingSet64 / 1024 / 1024).ToString() + " / " + (proc.VirtualMemorySize64 / 1024 / 1024).ToString();
			}
		}
		private void Update()
		{
			OnPropertyChanged(() => Items);
			OnPropertyChanged(() => Count);
			OnPropertyChanged(() => Memory);
		}

		private PerformanceItemViewModel _selectedItem;
		public PerformanceItemViewModel SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				OnPropertyChanged(() => SelectedItem);
			}
		}
	}
}
