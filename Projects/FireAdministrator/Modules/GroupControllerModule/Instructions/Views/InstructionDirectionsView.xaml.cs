using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
