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
			Description = descriptor.XBase.PresentationName;
			switch (descriptor.DescriptorType)
			{
				case DescriptorType.Device:
					ImageSource = descriptor.Device.Driver.ImageSource;
					break;

				case DescriptorType.Zone:
					ImageSource = "/Controls;component/Images/Zone.png";
					break;

				case DescriptorType.Direction:
					ImageSource = "/Controls;component/Images/Blue_Direction.png";
					break;

				case DescriptorType.PumpStation:
					ImageSource = "/Controls;component/Images/BPumpStation.png";
					break;

				case DescriptorType.MPT:
					ImageSource = "/Controls;component/Images/BAlarm_shield.png";
					break;

				case DescriptorType.Delay:
					ImageSource = "/Controls;component/Images/Delay.png";
					break;

				case DescriptorType.Pim:
					ImageSource = "/Controls;component/Images/Pim.png";
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