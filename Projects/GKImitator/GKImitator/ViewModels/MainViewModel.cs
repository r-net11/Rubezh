using System.Collections.Generic;
using System.Linq;
using GKImitator.Processor;
using RubezhAPI.GK;
using GKProcessor;
using Infrastructure.Common.Windows.Windows.ViewModels;
using System.Threading;
using System;
using System.Windows;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;

namespace GKImitator.ViewModels
{
	public class MainViewModel : ApplicationViewModel
	{
		GKImitator.Processor.NetProcessor GKProcessor;
		public static MainViewModel Current { get; private set; }

		public MainViewModel()
		{
			Title = "Имитатор ГК";
			Current = this;
			ShowUsersCommand = new RelayCommand(OnShowUsers);
			ShowSchedulesCommand = new RelayCommand(OnShowSchedules);
			ConfigurationCashHelper.Update();
			InitializeDescriptors();
			GKProcessor = new NetProcessor();
			GKProcessor.Start();
			ImitatorServiceManager.Open();
			DelayThread = new Thread(OnCheckDelays);
			DelayThread.Start();
		}

		bool IsApplicationClosing = false;
		Thread DelayThread;
		void OnCheckDelays()
		{
			while (Application.Current != null)
			{
				Thread.Sleep(TimeSpan.FromSeconds(1));
				if (Application.Current != null)
				{
					Application.Current.Dispatcher.Invoke((Action)(() =>
						{
							foreach (var descriptor in Descriptors)
							{
								if (!(descriptor.GKBase is GKPim))
									descriptor.CheckDelays();
							}
						}
						));
				}
			}
		}

		void InitializeDescriptors()
		{
			DescriptorsManager.Create();
			var gkDatabase = DescriptorsManager.GkDatabases.FirstOrDefault();

			Descriptors = new List<DescriptorViewModel>();
			if (gkDatabase != null)
			{
				foreach (var descriptor in gkDatabase.Descriptors)
				{
					var binObjectViewModel = new DescriptorViewModel(descriptor);
					Descriptors.Add(binObjectViewModel);
				}

				foreach (var kauDatabase in gkDatabase.KauDatabases)
				{
					foreach (var descriptor in kauDatabase.Descriptors)
					{
						var descriptorViewModel = Descriptors.FirstOrDefault(x => x.GKBase.GKDescriptorNo == descriptor.GKBase.GKDescriptorNo);
						if (descriptorViewModel != null)
						{
							descriptorViewModel.SetKauDescriptor(descriptor);
						}
					}
				}
			}
			SelectedDescriptor = Descriptors.FirstOrDefault();
		}

		List<DescriptorViewModel> _descriptors;
		public List<DescriptorViewModel> Descriptors
		{
			get { return _descriptors; }
			set
			{
				_descriptors = value;
				OnPropertyChanged(() => Descriptors);
			}
		}

		DescriptorViewModel _selectedDescriptor;
		public DescriptorViewModel SelectedDescriptor
		{
			get { return _selectedDescriptor; }
			set
			{
				_selectedDescriptor = value;
				OnPropertyChanged(() => SelectedDescriptor);
			}
		}

		bool HasAttention = false;
		bool HasFire1 = false;
		bool HasFire2 = false;
		bool HasAutomaticOff = false;

		public void RebuildIndicators()
		{
			var hasAttention = false;
			var hasFire1 = false;
			var hasFire2 = false;
			var hasAutomaticOff = false;

			foreach (var descriptorViewModel in Descriptors)
			{
				hasAttention = hasAttention || descriptorViewModel.GetStateBit(GKStateBit.Attention);
				hasFire1 = hasFire1 || descriptorViewModel.GetStateBit(GKStateBit.Fire1);
				hasFire2 = hasFire2 || descriptorViewModel.GetStateBit(GKStateBit.Fire2);
				hasAutomaticOff = hasAutomaticOff || !descriptorViewModel.GetStateBit(GKStateBit.Norm);
			}

			if (HasFire2 != hasFire2)
			{
				var descriptorViewModel = Descriptors.FirstOrDefault(x => x.GKBaseDescriptor.GKBase is GKDevice && (x.GKBaseDescriptor.GKBase as GKDevice).ShortName == "Индикатор Пожар 2");
				if (descriptorViewModel != null)
				{
					 descriptorViewModel.SetStateBit(GKStateBit.On, hasFire2);
				}
			}

			HasAttention = hasAttention;
			HasFire1 = hasFire1;
			HasFire2 = hasFire2;
			HasAutomaticOff = hasAutomaticOff;
		}

		public RelayCommand ShowUsersCommand { get; private set; }
		void OnShowUsers()
		{
			var usersViewModel = new UsersViewModel();
			DialogService.ShowModalWindow(usersViewModel);
		}

		public RelayCommand ShowSchedulesCommand { get; private set; }
		void OnShowSchedules()
		{
			var schedulesViewModel = new SchedulesViewModel();
			DialogService.ShowModalWindow(schedulesViewModel);
		}
	}
}