using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
    public class ConvertationViewModel : SaveCancelDialogViewModel
    {
        public ConvertationViewModel()
        {
            Title = "Сопоставление шлейфов при конвертировании";

			int Index = 0;
            FSShleifs = new ObservableCollection<FSShleifViewModel>();
            foreach (var device in FiresecManager.Devices)
            {
                if (device.Driver.IsPanel)
                {
                    for (int i = 0; i < device.Driver.ShleifCount; i++)
                    {
                        var fsShleifViewModel = new FSShleifViewModel()
                        {
							FSDevice = device,
                            FSShleifNo = i + 1,
                        };
						var kauDevice = fsShleifViewModel.KAUDevices.FirstOrDefault(x=>x.IntAddress == (Index/8 + 1));
						if(kauDevice != null)
						{
							fsShleifViewModel.KAUDevice = kauDevice;
							fsShleifViewModel.KAUShleifNo = Index % 8 + 1;
						}
                        FSShleifs.Add(fsShleifViewModel);
						Index++;
                    }
                }
            }
			SelectedFSShleif = FSShleifs.FirstOrDefault();
        }

        public ObservableCollection<FSShleifViewModel> FSShleifs { get; private set; }

		FSShleifViewModel _selectedFSShleif;
		public FSShleifViewModel SelectedFSShleif
		{
			get { return _selectedFSShleif; }
			set
			{
				_selectedFSShleif = value;
				OnPropertyChanged("SelectedFSShleif");
			}
		}
    }
}