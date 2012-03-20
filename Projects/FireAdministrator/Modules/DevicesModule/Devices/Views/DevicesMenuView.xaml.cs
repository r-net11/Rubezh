using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using Common;
using DevicesModule.ViewModels;
using FiresecAPI.Models;
using System.Linq;

namespace DevicesModule.Views
{
    public partial class DevicesMenuView : UserControl
    {
        public DevicesMenuView()
        {
            InitializeComponent();
            Current = this;
            AcceptKeyboard = true;
        }

        static DevicesMenuView()
        {
            EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDownStatic), true);
        }

        public static DevicesMenuView Current { get; private set; }
        public bool AcceptKeyboard { get; set; }

        static void OnKeyDownStatic(object sender, KeyEventArgs e)
        {
            Current.OnKeyDown(sender, e);
        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (AcceptKeyboard != true)
                return;

            if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
                PressButton(copyButton);

            if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
                PressButton(pasteButton);

            if (e.Key == Key.X && Keyboard.Modifiers == ModifierKeys.Control)
                PressButton(cutButton);

            if (e.Key == Key.Delete)
                PressButton(removeButton);

            if (e.Key == Key.Insert)
                PressButton(addButton);

            if (e.Key == Key.P && Keyboard.Modifiers == ModifierKeys.Control)
                PressButton(showPropertiesButton);

            if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control)
                PressButton(autoDetectButton);

            if (e.Key == Key.R && Keyboard.Modifiers == ModifierKeys.Control)
                PressButton(readDeviceButton, false);

            if (e.Key == Key.R && Keyboard.Modifiers == ModifierKeys.Alt)
                PressButton(usbReadDeviceButton, true);

            if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control)
            {
                PressButton(writeDeviceButton, false);
            }

            if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Alt)
            {
                PressButton(usbWriteDeviceButton, true);
            }

            if (e.Key == Key.W && ((int)Keyboard.Modifiers == ((int)ModifierKeys.Control + (int)ModifierKeys.Shift)))
                PressButton(writeAllDeviceButton);

            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                PressButton(setPasswordButton, false);
            }

            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Alt)
            {
                PressButton(usbSetPasswordButton, true);
            }

            if (e.Key == Key.U && Keyboard.Modifiers == ModifierKeys.Control)
            {
                PressButton(updateSoftButton, false);
            }

            if (e.Key == Key.U && Keyboard.Modifiers == ModifierKeys.Alt)
            {
                PressButton(usbUpdateSoftButton, true);
            }
            //
            if (e.Key == Key.Right)
            {
                if (DevicesViewModel.Current == null || DevicesViewModel.Current.SelectedDevice == null)
                    return;

                if (DevicesViewModel.Current.SelectedDevice.HasChildren && !DevicesViewModel.Current.SelectedDevice.IsExpanded) 
                    DevicesViewModel.Current.SelectedDevice.IsExpanded = true;
            }

            if (e.Key == Key.Left)
            {
                if (DevicesViewModel.Current == null || DevicesViewModel.Current.SelectedDevice == null)
                    return;

                if (DevicesViewModel.Current.SelectedDevice.HasChildren && DevicesViewModel.Current.SelectedDevice.IsExpanded)
                    DevicesViewModel.Current.SelectedDevice.IsExpanded = false;
            }
        }

        void PressButton(Button button)
        {
            if (button.Command.CanExecute(null))
                button.Command.Execute(null);
        }

        void PressButton(MenuItem menuItem, bool? commandParameter = null)
        {
            if (menuItem.Command.CanExecute(commandParameter))
                menuItem.Command.Execute(commandParameter);
        }
    }
}