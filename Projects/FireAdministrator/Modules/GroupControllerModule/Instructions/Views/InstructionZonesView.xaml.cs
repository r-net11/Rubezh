using System.Windows.Controls;
using GKModule.ViewModels;

namespace GKModule.Views
{
    public partial class InstructionZonesView : UserControl
    {
        public InstructionZonesView()
        {
            InitializeComponent();
        }

        private void SelectedAvailableZoneDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var viewModel = DataContext as InstructionZonesViewModel;
            if (viewModel.AddOneCommand.CanExecute(null))
                viewModel.AddOneCommand.Execute();
        }

        private void SelectedInstructionZoneDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var viewModel = DataContext as InstructionZonesViewModel;
            if (viewModel.RemoveOneCommand.CanExecute(null))
                viewModel.RemoveOneCommand.Execute();
        }
    }
}