using System;
using System.Windows;
using System.Windows.Media;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;

namespace PlansModule.Primitives
{
	/// <summary>
	/// Painter for a Primitive.
	/// </summary>
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

		public Rect Bounds
		{
			get { return this.primitivePainter.Bounds; }
		}

		public void Invalidate()
		{
			this.primitivePainter.Invalidate();
		}

		public void Draw(DrawingContext drawingContext)
		{
			this.primitivePainter.Draw(drawingContext);
		}

		public void Transform()
		{
			this.primitivePainter.Transform();
		}

		public bool HitTest(Point point)
		{
			return this.primitivePainter.HitTest(point);
		}

		public object GetToolTip(string title)
		{
			return this.primitivePainter.GetToolTip(title);
		}

		#region Properties

		#endregion

		#region Fields
		private readonly IPainter primitivePainter = null;

		#endregion
	}
}
