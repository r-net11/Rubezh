using System.Windows.Controls;
using System.Windows.Input;
using FiltersModule.ViewModels;

namespace FiltersModule.Views
{
    public partial class FiltersView : UserControl
    {
        public FiltersView()
        {
            InitializeComponent();
            CommandBinding DeleteCmdBinding = new CommandBinding(
                ApplicationCommands.Delete,
                DeleteCmdExecuted,
                DeleteCmdCanExecute);

            CommandBindings.Add(DeleteCmdBinding);
        }

        void DeleteCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            (DataContext as FiltersViewModel).RemoveCommand.Execute();
        }

        void DeleteCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}