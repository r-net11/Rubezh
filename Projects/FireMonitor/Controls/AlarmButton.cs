using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Controls
{
    public class AlarmButton : ToggleButton
    {
        public static readonly DependencyProperty CornerRadiusProperty =
            Border.CornerRadiusProperty.AddOwner(typeof(AlarmButton));

        public static readonly DependencyProperty OuterBorderBrushProperty =
            DependencyProperty.Register("OuterBorderBrush", typeof(Brush), typeof(AlarmButton));

        public static readonly DependencyProperty OuterBorderThicknessProperty =
            DependencyProperty.Register("OuterBorderThickness", typeof(Thickness), typeof(AlarmButton));

        public static readonly DependencyProperty InnerBorderBrushProperty =
            DependencyProperty.Register("InnerBorderBrush", typeof(Brush), typeof(AlarmButton));

        public static readonly DependencyProperty InnerBorderThicknessProperty =
            DependencyProperty.Register("InnerBorderThickness", typeof(Thickness), typeof(AlarmButton));

        public static readonly DependencyProperty GlowColorProperty =
            DependencyProperty.Register("GlowColor", typeof(SolidColorBrush), typeof(AlarmButton));

        public static readonly DependencyProperty HighlightMarginProperty =
            DependencyProperty.Register("HighlightMargin", typeof(Thickness), typeof(AlarmButton));

        public static readonly DependencyProperty HighlightBrightnessProperty =
            DependencyProperty.Register("HighlightBrightness", typeof(byte), typeof(AlarmButton));

        public static readonly DependencyProperty IsGlowingProperty =
            DependencyProperty.Register("IsGlowing", typeof(bool), typeof(AlarmButton));

        public static readonly DependencyProperty PathToIconProperty =
            DependencyProperty.Register("PathToIcon", typeof(string), typeof(AlarmButton));

        static AlarmButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AlarmButton), new FrameworkPropertyMetadata(typeof(AlarmButton)));
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius) GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public Brush OuterBorderBrush
        {
            get { return (Brush) GetValue(OuterBorderBrushProperty); }
            set { SetValue(OuterBorderBrushProperty, value); }
        }

        public Thickness OuterBorderThickness
        {
            get { return (Thickness) GetValue(OuterBorderThicknessProperty); }
            set { SetValue(OuterBorderThicknessProperty, value); }
        }

        public Brush InnerBorderBrush
        {
            get { return (Brush) GetValue(InnerBorderBrushProperty); }
            set { SetValue(InnerBorderBrushProperty, value); }
        }

        public Thickness InnerBorderThickness
        {
            get { return (Thickness) GetValue(InnerBorderThicknessProperty); }
            set { SetValue(InnerBorderThicknessProperty, value); }
        }

        public Brush GlowColor
        {
            get { return (SolidColorBrush) GetValue(GlowColorProperty); }
            set { SetValue(GlowColorProperty, value); }
        }

        public Thickness HighlightMargin
        {
            get { return (Thickness) GetValue(HighlightMarginProperty); }
            set { SetValue(HighlightMarginProperty, value); }
        }

        public byte HighlightBrightness
        {
            get { return (byte) GetValue(HighlightBrightnessProperty); }
            set { SetValue(HighlightBrightnessProperty, value); }
        }

        public bool IsGlowing
        {
            get { return (bool) GetValue(IsGlowingProperty); }
            set { SetValue(IsGlowingProperty, value); }
        }

        public string PathToIcon
        {
            get { return (string) GetValue(PathToIconProperty); }
            set { SetValue(PathToIconProperty, value); }
        }
    }
}