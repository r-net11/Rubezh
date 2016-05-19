using System.Collections.ObjectModel;
using System.Linq;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.GK;

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
			PresentationName = descriptor.GKBase.PresentationName;
			Description = descriptor.GKBase.Description;
			switch (descriptor.DescriptorType)
			{
				case DescriptorType.Device:
					var device = descriptor.GKBase as GKDevice;
					if (device != null)
					{
						ImageSource = device.Driver.ImageSource;
					}
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
					ImageSource = "/Controls;component/Images/BMPT.png";
					break;

				case DescriptorType.Delay:
					ImageSource = "/Controls;component/Images/Delay.png";
					break;

				case DescriptorType.Pim:
					ImageSource = "/Controls;component/Images/Pim.png";
					break;

				case DescriptorType.GuardZone:
					ImageSource = "/Controls;component/Images/GuardZone.png";
					break;

				case DescriptorType.Code:
					ImageSource = "/Controls;component/Images/Code.png";
					break;

				case DescriptorType.Door:
					ImageSource = "/Controls;component/Images/Door.png";
					break;
			}

			IsFormulaInvalid = !Descriptor.Formula.CheckStackOverflow();
		}

		public void InitializeLogic()
		{
			DescriptorLogicItems = new ObservableCollection<DescriptorLogicItem>();
			var branches = Descriptor.Formula.GetBranches();
			//foreach (var formulaOperation in Descriptor.Formula.FormulaOperations)
			for(int i = 0; i < Descriptor.Formula.FormulaOperations.Count; i++)
			{
				var formulaOperation = Descriptor.Formula.FormulaOperations[i];

				var descriptorLogicItem = new DescriptorLogicItem(formulaOperation, DescriptorsViewModel, Descriptor);
				DescriptorLogicItems.Add(descriptorLogicItem);
				if(!string.IsNullOrEmpty(descriptorLogicItem.Error))
				{
					IsFormulaInvalid = true;
				}

				var stackString = "";
				foreach(var branch in branches)
				{
					var stackDepth = branch.StackDepthHistory.FirstOrDefault(x=>x.Item1 == i);
					if(stackDepth != null)
					{
						stackString += stackDepth.Item2.ToString("d00") + " ";
					}
					else
					{
						stackString += "   ";
					}
				}
				descriptorLogicItem.StackDepth = stackString;
			}
		}

		public BaseDescriptor Descriptor { get; set; }
		public string ImageSource { get; set; }
		public string PresentationName { get; set; }
		public string Description { get; set; }

		bool _isFormulaInvalid;
		public bool IsFormulaInvalid
		{
			get { return _isFormulaInvalid; }
			set
			{
				_isFormulaInvalid = value;
				OnPropertyChanged(() => IsFormulaInvalid);
			}
		}

		public ObservableCollection<DescriptorLogicItem> DescriptorLogicItems { get; private set; }

		ObservableCollection<DescriptorViewModel> _inputDescriptors;
		public ObservableCollection<DescriptorViewModel> InputDescriptors
		{
			get { return _inputDescriptors; }
			set
			{
				_inputDescriptors = value;
				OnPropertyChanged(() => InputDescriptors);
			}
		}

		ObservableCollection<DescriptorViewModel> _outputDescriptors;
		public ObservableCollection<DescriptorViewModel> OutputDescriptors
		{
			get { return _outputDescriptors; }
			set
			{
				_outputDescriptors = value;
				OnPropertyChanged(() => OutputDescriptors);
			}
		}

		public RelayCommand NavigateCommand { get; private set; }
		void OnNavigate()
		{
			var descriptorViewModel = DescriptorsViewModel.Descriptors.FirstOrDefault(x => x.Descriptor.GKBase.UID == Descriptor.GKBase.UID);
			if (descriptorViewModel != null)
			{
				DescriptorsViewModel.SelectedDescriptor = descriptorViewModel;
			}
		}
	}
}