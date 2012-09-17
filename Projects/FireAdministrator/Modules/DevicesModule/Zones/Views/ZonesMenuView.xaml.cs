using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevicesModule.ViewModels;

namespace DevicesModule.Views
{
    public partial class ZonesMenuView : UserControl
    {
        public ZonesMenuView()
        {
            InitializeComponent();
            Current = this;
            AcceptKeyboard = true;
        }

        static ZonesMenuView()
        {
            EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDownStatic), true);
        }

        public static ZonesMenuView Current { get; private set; }
        public bool AcceptKeyboard { get; set; }

        static void OnKeyDownStatic(object sender, KeyEventArgs e)
        {
            Current.OnKeyDown(sender, e);
        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (AcceptKeyboard != true)
                return;

            if (e.Key == Key.R && Keyboard.Modifiers == ModifierKeys.Control)
                PressButton(editButton);

            if (e.Key == Key.Delete && Keyboard.Modifiers == ModifierKeys.Control)
                PressButton(deleteButton);

            if (e.Key == Key.Delete && ((int)Keyboard.Modifiers == ((int)ModifierKeys.Control + (int)ModifierKeys.Shift)))
                PressButton(deleteAllEmptyButton);

            if (e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control)
                PressButton(addButton);

            if (e.Key == Key.L && Keyboard.Modifiers == ModifierKeys.Control)
            {
                var zonesViewModel = DataContext as ZonesViewModel;
                if (zonesViewModel != null)
                {
                    if (zonesViewModel.ZoneDevices.ShowZoneLogicCommand.CanExecute(null))
                        zonesViewModel.ZoneDevices.ShowZoneLogicCommand.Execute();
                }
            }

        }

        void PressButton(Button button)
        {
            if (button.Command.CanExecute(null))
                button.Command.Execute(null);
        }
    }
}