using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace InstructionsModule.Views
{
    public partial class InstructionsView : UserControl
    {
        public InstructionsView()
        {
            InitializeComponent();
        }

        public static InputBindingCollection GetInputBindings(UserControl element)
        {
            return (InputBindingCollection)element.GetValue(InputBindingsProperty);
        }

        public static void SetInputBindings(
          UserControl element, InputBindingCollection value)
        {
            element.SetValue(InputBindingsProperty, value);
        }

        public static readonly DependencyProperty InputBindingsProperty =
            DependencyProperty.RegisterAttached(
            "InputBindings",
            typeof(InputBindingCollection),
            typeof(InstructionsView), new FrameworkPropertyMetadata());
    }
}
