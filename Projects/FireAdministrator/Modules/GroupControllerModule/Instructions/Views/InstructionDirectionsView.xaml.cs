using System.Windows.Controls;
using GKModule.ViewModels;

namespace GKModule.Views
{
    public partial class InstructionDirectionsView : UserControl
    {
        public InstructionDirectionsView()
        {
            InitializeComponent();
        }

        private void SelectedAvailableDirectionDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var viewModel = DataContext as InstructionDirectionsViewModel;
            if (viewModel.AddOneCommand.CanExecute(null))
                viewModel.AddOneCommand.Execute();
        }

        private void SelectedInstructionDirectionDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var viewModel = DataContext as InstructionDirectionsViewModel;
            if (viewModel.RemoveOneCommand.CanExecute(null))
                viewModel.RemoveOneCommand.Execute();
        }
    }
}
