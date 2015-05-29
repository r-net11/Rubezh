using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using GKProcessor;
using Infrastructure.Common.Windows.ViewModels;

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
			InitializeBinaryObjects();

			GKProcessor = new GKImitator.Processor.GKProcessor();
			GKProcessor.Start();
		}

		void InitializeBinaryObjects()
		{
			DescriptorsManager.Create();
			var gkDatabase = DescriptorsManager.GkDatabases.FirstOrDefault();

			BinaryObjects = new List<BinaryObjectViewModel>();
			if (gkDatabase != null)
			{
				foreach (var descriptors in gkDatabase.Descriptors)
				{
					var binObjectViewModel = new BinaryObjectViewModel(descriptors);
					BinaryObjects.Add(binObjectViewModel);
				}
			}
			SelectedBinaryObject = BinaryObjects.FirstOrDefault();
		}

		List<BinaryObjectViewModel> _binaryObjects;
		public List<BinaryObjectViewModel> BinaryObjects
		{
			get { return _binaryObjects; }
			set
			{
				_binaryObjects = value;
				OnPropertyChanged(() => BinaryObjects);
			}
		}

		BinaryObjectViewModel _selectedBinaryObject;
		public BinaryObjectViewModel SelectedBinaryObject
		{
			get { return _selectedBinaryObject; }
			set
			{
				_selectedBinaryObject = value;
				OnPropertyChanged(() => SelectedBinaryObject);
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

			foreach (var binaryObjectViewModel in BinaryObjects)
			{
				hasAttention = hasAttention || binaryObjectViewModel.StateBits.Any(x => x.StateBit == GKStateBit.Attention && x.IsActive);
				hasFire1 = hasFire1 || binaryObjectViewModel.StateBits.Any(x => x.StateBit == GKStateBit.Fire1 && x.IsActive);
				hasFire2 = hasFire2 || binaryObjectViewModel.StateBits.Any(x => x.StateBit == GKStateBit.Fire2 && x.IsActive);
				hasAutomaticOff = hasAutomaticOff || binaryObjectViewModel.StateBits.Any(x => x.StateBit == GKStateBit.Norm && !x.IsActive);
			}

			if (HasFire2 != hasFire2)
			{
				var binaryObjectViewModel = BinaryObjects.FirstOrDefault(x => x.BinaryObject.GKBase is GKDevice && (x.BinaryObject.GKBase as GKDevice).ShortName == "Индикатор Пожар 2");
				if (binaryObjectViewModel != null)
				{
					var staeBitViewModel = binaryObjectViewModel.StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.On);
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