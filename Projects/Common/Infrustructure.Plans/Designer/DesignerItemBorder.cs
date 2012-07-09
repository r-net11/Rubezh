using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows;

namespace Infrustructure.Plans.Designer
{
	public class DesignerItemBorder : Control
	{
		public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register("Thickness", typeof(double), typeof(DesignerItemBorder), new FrameworkPropertyMetadata(1.0));
		public virtual double Thickness
		{
			get { return (double)GetValue(ThicknessProperty); }
			set { SetValue(ThicknessProperty, value); }
		}

	}
}
