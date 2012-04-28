using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SoundsModule.Views
{
    public partial class SoundsMenuView : UserControl
    {
        public SoundsMenuView()
        {
            InitializeComponent();
            Current = this;
            AcceptKeyboard = true;
        }

        static SoundsMenuView()
        {
            EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDownStatic), true);
        }

        public static SoundsMenuView Current { get; private set; }
        public bool AcceptKeyboard { get; set; }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (AcceptKeyboard == false)
                return;
            if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                PressButton(playButton);
            }            
        }

        static void OnKeyDownStatic(object sender, KeyEventArgs e)
        {
            Current.OnKeyDown(sender, e);
        }

        void PressButton(Button button)
        {
            if (button.Command.CanExecute(null))
                button.Command.Execute(null);
        }
    }
}
