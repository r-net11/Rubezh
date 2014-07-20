using System.Collections.ObjectModel;
using System.Linq;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DescriptorViewModel : BaseViewModel
	{
		DescriptorsViewModel DescriptorsViewModel;

		public DescriptorViewModel(BaseDescriptor descriptor, DescriptorsViewModel descriptorsViewModel)
		{
			NavigateCommand = new RelayCommand(OnNavigate);
			DescriptorsViewModel = descriptorsViewModel;
			Descriptor = descriptor;
			PresentationName = descriptor.XBase.PresentationName;
			switch (descriptor.DescriptorType)
			{
				case DescriptorType.Device:
					ImageSource = descriptor.Device.Driver.ImageSource;
					Description = descriptor.Device.Description;
					break;

				case DescriptorType.Zone:
					ImageSource = "/Controls;component/Images/Zone.png";
					Description = descriptor.Zone.Description;
					break;

				case DescriptorType.Direction:
					ImageSource = "/Controls;component/Images/Blue_Direction.png";
					Description = descriptor.Direction.Description;
					break;

				case DescriptorType.PumpStation:
					ImageSource = "/Controls;component/Images/BPumpStation.png";
					Description = descriptor.PumpStation.Description;
					break;

				case DescriptorType.MPT:
					ImageSource = "/Controls;component/Images/BMPT.png";
					Description = descriptor.MPT.Description;
					break;

				case DescriptorType.Delay:
					ImageSource = "/Controls;component/Images/Delay.png";
					Description = descriptor.Delay.Description;
					break;

				case DescriptorType.Pim:
					ImageSource = "/Controls;component/Images/Pim.png";
					break;

				case DescriptorType.GuardZone:
					ImageSource = "/Controls;component/Images/GuardZone.png";
					Description = descriptor.GuardZone.Description;
					break;

				case DescriptorType.Code:
					ImageSource = "/Controls;component/Images/Code.png";
					Description = descriptor.Code.Description;
					break;
			}

			IsFormulaInvalid = Descriptor.Formula.CalculateStackLevels();
		}

		public void InitializeLogic()
		{
			DescriptorLogicItems = new ObservableCollection<DescriptorLogicItem>();
			foreach (var formulaOperation in Descriptor.Formula.FormulaOperations)
			{
				var descriptorLogicItem = new DescriptorLogicItem(formulaOperation, DescriptorsViewModel);
				DescriptorLogicItems.Add(descriptorLogicItem);
			}
		}

		public BaseDescriptor Descriptor { get; set; }
		public string ImageSource { get; set; }
		public string PresentationName { get; set; }
		public string Description { get; set; }
		public bool IsFormulaInvalid { get; set; }


		public ObservableCollection<DescriptorLogicItem> DescriptorLogicItems { get; private set; }

		ObservableCollection<DescriptorViewModel> _inputDescriptors;
		public ObservableCollection<DescriptorViewModel> InputDescriptors
		{
			get { return _inputDescriptors; }
			set
			{
				_inputDescriptors = value;
				OnPropertyChanged("InputDescriptors");
			}
		}

		ObservableCollection<DescriptorViewModel> _outputDescriptors;
		public ObservableCollection<DescriptorViewModel> OutputDescriptors
		{
			get { return _outputDescriptors; }
			set
			{
				_outputDescriptors = value;
				OnPropertyChanged("OutputDescriptors");
			}
		}

		public RelayCommand NavigateCommand { get; private set; }
		void OnNavigate()
		{
			var descriptorViewModel = DescriptorsViewModel.Descriptors.FirstOrDefault(x => x.Descriptor.XBase.BaseUID == Descriptor.XBase.BaseUID);
			if (descriptorViewModel != null)
			{
				DescriptorsViewModel.SelectedDescriptor = descriptorViewModel;
			}
		}
	}
}