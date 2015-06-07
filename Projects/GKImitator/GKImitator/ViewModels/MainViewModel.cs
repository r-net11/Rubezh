using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using GKProcessor;
using Infrastructure.Common.Windows.ViewModels;
using System.Threading;
using System;
using System.Windows;

namespace GKImitator.ViewModels
{
	public class MainViewModel : ApplicationViewModel
	{
		GKImitator.Processor.GKProcessor GKProcessor;
		public static MainViewModel Current { get; private set; }

		public MainViewModel()
		{
			Title = "Имитатор ГК";
			Current = this;

			ConfigurationCashHelper.Update();
			InitializeDescriptors();

			GKProcessor = new GKImitator.Processor.GKProcessor();
			GKProcessor.Start();

			DelayThread = new Thread(OnCheckDelays);
			DelayThread.Start();
		}

		Thread DelayThread;
		void OnCheckDelays()
		{
			while (true)
			{
				Thread.Sleep(TimeSpan.FromSeconds(1));
				Application.Current.Dispatcher.Invoke((Action)(() =>
					{
						foreach (var descriptor in Descriptors)
						{
							descriptor.CheckDelays();
						}
					}
					));
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
			}
			SelectedDescriptor = Descriptors.FirstOrDefault();

			foreach (var kauDatabase in DescriptorsManager.KauDatabases)
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
				hasAttention = hasAttention || descriptorViewModel.StateBits.Any(x => x.StateBit == GKStateBit.Attention && x.IsActive);
				hasFire1 = hasFire1 || descriptorViewModel.StateBits.Any(x => x.StateBit == GKStateBit.Fire1 && x.IsActive);
				hasFire2 = hasFire2 || descriptorViewModel.StateBits.Any(x => x.StateBit == GKStateBit.Fire2 && x.IsActive);
				hasAutomaticOff = hasAutomaticOff || descriptorViewModel.StateBits.Any(x => x.StateBit == GKStateBit.Norm && !x.IsActive);
			}

			if (HasFire2 != hasFire2)
			{
				var descriptorViewModel = Descriptors.FirstOrDefault(x => x.BaseDescriptor.GKBase is GKDevice && (x.BaseDescriptor.GKBase as GKDevice).ShortName == "Индикатор Пожар 2");
				if (descriptorViewModel != null)
				{
					var staeBitViewModel = descriptorViewModel.StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.On);
					if (staeBitViewModel != null)
						staeBitViewModel.IsActive = hasFire2;
				}
			}

			HasAttention = hasAttention;
			HasFire1 = hasFire1;
			HasFire2 = hasFire2;
			HasAutomaticOff = hasAutomaticOff;
		}
	}
}