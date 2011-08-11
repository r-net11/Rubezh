using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace PlansModule.ViewModels
{
    public class TreeViewNew : TreeView
    {
        /*
         *         protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
                {
                    base.OnMouseLeftButtonDown(e);
         * */
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

        }
        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
        }
        protected override void OnLostKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            MessageBox.Show("test");
        }
    }
}
