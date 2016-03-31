using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKDModule.PassCard.Designer
{
	public class PrimitivePainter : IPainter
	{
		/// <summary>
		/// Initializes a new Instance of current Class.
		/// </summary>
		/// <param name="primitivePainter">Painter used for Primitives.</param>
		public PrimitivePainter(IPainter primitivePainter)
		{
			if (primitivePainter == null)
				throw new ArgumentNullException("primitivePainter");
			this.primitivePainter = primitivePainter;
		}

		public void ResetElement(ElementBase element)
		{
			this.primitivePainter.ResetElement(element);
		}

		public System.Windows.Rect Bounds
		{
			get { return this.primitivePainter.Bounds; }
		}

		public void Invalidate()
		{
			this.primitivePainter.Invalidate();
		}

		public void Draw(System.Windows.Media.DrawingContext drawingContext)
		{
			this.primitivePainter.Draw(drawingContext);
		}

		public void Transform()
		{
			this.primitivePainter.Transform();
		}

		public bool HitTest(System.Windows.Point point)
		{
			return this.primitivePainter.HitTest(point);
		}

		public object GetToolTip(string title)
		{
			return this.ToolTip;
		}

		#region Properties

		public BaseViewModel ToolTip { get; set; }

		#endregion

		#region Fields
		private readonly IPainter primitivePainter = null;

		#endregion
	}
}
