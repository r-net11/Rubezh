using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using FiresecClient;

namespace GKModule.ViewModels
{
    public class ConvertationViewModel : SaveCancelDialogViewModel
    {
        public ConvertationViewModel()
        {
            Title = "Сопоставление шлейфов при конвертировании";

            FSShleifs = new ObservableCollection<FSShleifViewModel>();
            foreach (var device in FiresecManager.Devices)
            {
                if (device.Driver.IsPanel)
                {
                    for (int i = 0; i < device.Driver.ShleifCount; i++)
                    {
                        var fsShleifViewModel = new FSShleifViewModel()
                        {
                            Name = device.PresentationAddressAndDriver,
                            FSShleifNo = i + 1,
                            KAUName = "КАУ",
                            KAUShleifNo = 1
                        };
                        FSShleifs.Add(fsShleifViewModel);
                    }
                }
            }
        }

        public ObservableCollection<FSShleifViewModel> FSShleifs { get; private set; }
    }
}