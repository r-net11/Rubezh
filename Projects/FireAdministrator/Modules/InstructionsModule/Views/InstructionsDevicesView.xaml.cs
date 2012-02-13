using System.Windows.Controls;
using InstructionsModule.ViewModels;

namespace InstructionsModule.Views
{
    public partial class InstructionsDevicesView : UserControl
    {
        public InstructionsDevicesView()
        {
            InitializeComponent();
        }

        private void SelectedAvailableDeviceDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var viewModel = DataContext as InstructionDevicesViewModel;
            if (viewModel.AddOneCommand.CanExecute(null))
                viewModel.AddOneCommand.Execute();
        }

        private void SelectedInstructionDeviceDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var viewModel = DataContext as InstructionDevicesViewModel;
            if (viewModel.RemoveOneCommand.CanExecute(null))
                viewModel.RemoveOneCommand.Execute();
        }
    }
}
