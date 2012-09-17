using Firesec.Imitator;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace DiagnosticsModule.ViewModels
{
    public class DiagnosticsViewModel : ViewPartViewModel
    {
        public DiagnosticsViewModel()
        {
            ShowImitatorCommand = new RelayCommand(OnShowImitator);
        }

        public RelayCommand ShowImitatorCommand { get; private set; }
        void OnShowImitator()
        {
			ImitatorService.Show();
        }
    }
}