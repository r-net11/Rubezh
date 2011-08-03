using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Input;
using System.Windows;



namespace PlanEditor.Basic
{
    public abstract class PEShape : UIElement, ICloneable
    {
        
        // true - фигуру двигать нельзя
        protected bool Fixed;
        protected Point ptCurrent;
        public bool active;
        public bool resize;
        
        #region Z-последовательность
        public int Layer
        {
            get
            {
                return Layer;
            }
            set
            {
                if (!Fixed) this.Layer = value;
            }
        }

        #endregion

        public PEShape()
        {
            Fixed=false;
            resize = false;
        }
        public object Clone() 
        {
            return this.MemberwiseClone();
        }
        public virtual UIElement GetShape() { return this;}
        public virtual void MouseMove(object sender, MouseEventArgs e) { }
    }
}
