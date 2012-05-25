using System;
using System.Linq;
using System.Windows.Threading;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using System.Collections.Generic;
using System.Threading;
using Infrastructure;

namespace DevicesModule.ViewModels
{
	public class DeviceControlViewModel : BaseViewModel
	{
		Device Device;
		bool IsBuisy = false;

		public DeviceControlViewModel(Device device)
		{
			Device = device;
			ConfirmCommand = new RelayCommand(OnConfirm, CanConfirm);

			Blocks = new List<BlockViewModel>();

			foreach (var property in device.Driver.Properties)
			{
				if (property.IsControl)
				{
					var blockViewModel = Blocks.FirstOrDefault(x => x.Name == property.BlockName);
					if (blockViewModel == null)
					{
						blockViewModel = new BlockViewModel()
						{
							Name = property.BlockName
						};
						Blocks.Add(blockViewModel);
					}
					blockViewModel.Commands.Add(property);
				}
			}
		}

		public List<BlockViewModel> Blocks { get; private set; }

		BlockViewModel _selectedBlock;
		public BlockViewModel SelectedBlock
		{
			get { return _selectedBlock; }
			set
			{
				_selectedBlock = value;
				OnPropertyChanged("SelectedBlock");
			}
		}

		bool CanConfirm()
		{
			return SelectedBlock != null && SelectedBlock.SelectedCommand != null && IsBuisy == false;
		}

		public RelayCommand ConfirmCommand { get; private set; }
		void OnConfirm()
		{
			IsBuisy = true;
			if (ServiceFactory.SecurityService.Validate())
			{
				var thread = new Thread(DoConfirm);
				thread.Start();
			}
		}

		void DoConfirm()
		{
			var result = FiresecManager.FiresecService.ExecuteCommand(Device.UID, SelectedBlock.SelectedCommand.Name);
			Dispatcher.BeginInvoke(new Action(() => { IsBuisy = false; OnPropertyChanged("ConfirmCommand"); }));
		}
	}

	public class BlockViewModel : BaseViewModel
	{
		public BlockViewModel()
		{
			Commands = new List<DriverProperty>();
		}

		public string Name { get; set; }

		public List<DriverProperty> Commands { get; set; }

		DriverProperty _selectedCommand;
		public DriverProperty SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				OnPropertyChanged("SelectedCommand");
			}
		}
	}
}