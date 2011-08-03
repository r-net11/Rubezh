using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using PlanEditor.Basic;
using System.Windows.Controls;
using System.Windows;

namespace PlanEditor
{
    class ListObjects : IEnumerable
    {
        public List<Object> Objects;
        public bool activeGroup;
        public ListObjects()
        {
            Objects = new List<Object>();
            activeGroup = false;
        }
        public void Add(Object obj)
        {
            Objects.Add(obj);
        }
        public IEnumerator GetEnumerator()
        {
            return Objects.GetEnumerator();
        }
        public void SetDeactiveLine(int _code, Canvas canvas)
        {
            
            foreach (Object obj in Objects)
            {
                
                Type t = typeof(PlanEditor.PELine);
                if (obj.GetType() == t)
                {

                    PELine line = (PELine)obj;
                    int i = line.GetShape().GetHashCode();

                    if (line.active)
                    {
                        line.SetDeactive(canvas);
                    }

                }
            }
        }
        public void DragMove(int _code, Canvas canvas)
        {
            foreach (Object obj in Objects)
            {
                
                Type t = typeof(PlanEditor.PELine);
                if (obj.GetType() == t)
                {
                    PELine line = (PELine)obj;
                    if (line.active)
                    {
                        line.Move(canvas);
                    }
                }
            }
        }
        public void DragFinished(int _code, Canvas canvas, double top, double left)
        {
            foreach (Object obj in Objects)
            {
                
                Type t = typeof(PlanEditor.PELine);
                if (obj.GetType() == t)
                {
                    PELine line = (PELine)obj;
                    if (line.active)
                    {
                        line.MoveFinished(canvas, top, left);
                    }
                }
            }
        }
        public void SetDeactiveAll(Canvas canvas)
        {
            foreach (Object obj in Objects)
            {
                Type tLine = typeof(PlanEditor.PELine);
                Type tRect = typeof(PlanEditor.PERectangle);
                Type tEllipse = typeof(PlanEditor.PEEllipse);
                if (obj.GetType() == tLine)
                {
                    PELine line = (PELine)obj;
                    if (line.active)
                    {
                        line.SetDeactive(canvas);
                    }
                }
                if (obj.GetType() == tRect)
                {
                    PERectangle rect = (PERectangle)obj;
                    if (rect.active)
                    {
                        rect.SetDeactive(canvas);
                    }
                }
                if (obj.GetType() == tEllipse)
                {
                    PEEllipse ellipse = (PEEllipse)obj;
                    if (ellipse.active)
                    {
                        ellipse.SetDeactive(canvas);
                    }
                }
            }
        }
        public void SetActiveLineToResize(bool resize)
        {
            foreach (Object obj in Objects)
            {
                Type t = typeof(PlanEditor.PELine);
                if (obj.GetType() == t)
                {
                    PELine line = (PELine)obj;
                    int i = line.GetShape().GetHashCode();
                    if (line.active)
                    {
                        line.resize = resize;
                    }

                }
            }
        }

        public void SetActiveShape(object sender, Canvas canvas)
        {
            if (sender is System.Windows.Shapes.Line)
            {
                SetActiveLine(sender.GetHashCode(), canvas);
            }
            if (sender is System.Windows.Shapes.Rectangle)
            {
                SetActiveRectangle(sender.GetHashCode(), canvas);
            }
            if (sender is System.Windows.Shapes.Ellipse)
            {
                SetActiveEllipse(sender.GetHashCode(), canvas);
            }
        }
        public void SetActiveEllipse(int _code, Canvas canvas)
        {
            if (!activeGroup) SetDeactiveAll(canvas);
            foreach (Object obj in Objects)
            {
                Type t = typeof(PlanEditor.PEEllipse);
                if (obj.GetType() == t)
                {
                    PEEllipse ellipse= (PEEllipse)obj;
                    int i = ellipse.GetShape().GetHashCode();
                    if (i == _code && !ellipse.active)
                    {
                        ellipse.SetActive(canvas);
                    }

                }
            }
        }
        public void SetActiveRectangle(int _code, Canvas canvas)
        {
            if (!activeGroup) SetDeactiveAll(canvas);
            foreach (Object obj in Objects)
            {
                Type t = typeof(PlanEditor.PERectangle);
                if (obj.GetType() == t)
                {

                    PERectangle rect = (PERectangle)obj;
                    int i = rect.GetShape().GetHashCode();
                    if (i == _code && !rect.active)
                    {
                        rect.SetActive(canvas);
                    }

                }
            }
        }
            
        public void SetActiveLine(int _code, Canvas canvas)
        {
            
            if (!activeGroup)SetDeactiveAll(canvas);
            foreach (Object obj in Objects)
            {
                Type t = typeof(PlanEditor.PELine);
                if (obj.GetType()==t)
                {
                    
                    PELine line = (PELine)obj;
                    int i = line.GetShape().GetHashCode();
                    if (i == _code && !line.active)
                    {
                        line.SetActive(canvas);
                    }
                   
                }
            }
        }
    }

    class ListShapes :IEnumerable
    {
        public List<PEShape> Shapes;
        private int Width;
        private int Height;
        public int GetMaxLayer()
        {
            return Shapes.Count;
        }

        public PEShape GetShape(int _layer)
        {
            return Shapes[_layer];
        }

        public bool ChangeLayerUp(PEShape shape)
        {
            if (shape.Layer< Shapes.Count)
            {
                PEShape tmp = Shapes[shape.Layer];
                Shapes[shape.Layer] = Shapes[shape.Layer + 1];
                Shapes[shape.Layer + 1] = Shapes[shape.Layer];
                return true;
            }
            else return false;
        }

        public bool ChangeLayerDown(PEShape shape)
        {
            return false;
        }

        public ListShapes(PEShape _shape)
        {
            Shapes = new List<PEShape>();
            Shapes.Add(_shape);
        }

        public ListShapes()
        {
            Shapes = new List<PEShape>();
            this.Width = 500;
            this.Height = 500;
        }
        public ListShapes(int width, int height)
        {
            Shapes = new List<PEShape>();
            this.Width = width;
            this.Height = height;
        }

        public void AddShape(PEShape _shape)
        {
            if (Shapes==null) Shapes = new List<PEShape>();
            Shapes.Add(_shape);
        }

        public IEnumerator GetEnumerator()
        {
            return Shapes.GetEnumerator();
        }
    }

}
