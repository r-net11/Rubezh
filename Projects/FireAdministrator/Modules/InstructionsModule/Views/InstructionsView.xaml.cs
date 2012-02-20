using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using InstructionsModule.ViewModels;

namespace InstructionsModule.Views
{
    public partial class InstructionsView : UserControl
    {
        public InstructionsView()
        {
            InitializeComponent();
            EventManager.RegisterClassHandler(typeof(UserControl), Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D1 && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if ((DataContext as InstructionsViewModel).AddCommand.CanExecute(null))
                {
                    (DataContext as InstructionsViewModel).AddCommand.Execute();
                }
            }
        }
    }
}
