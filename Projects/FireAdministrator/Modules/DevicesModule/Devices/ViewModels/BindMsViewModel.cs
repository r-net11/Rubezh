using Infrastructure.Common;
using System;

namespace DevicesModule.ViewModels
{
    public class BindMsViewModel : SaveCancelDialogContent
    {
        Guid _deviceUID;

        public BindMsViewModel(Guid deviceUID)
        {
            _deviceUID = deviceUID;
            Title = "Привязка оборудования";
        }

        protected override void Save(ref bool cancel)
        {
            ;
        }
    }
}
