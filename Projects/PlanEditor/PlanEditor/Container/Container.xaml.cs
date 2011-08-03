using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Windows.Controls.Primitives;


namespace PlanEditor
{
     
    public partial class Container : System.Windows.Controls.UserControl
    {   
        CustomPanel  Panel = null;
        //PECanvas MyCanvas = new PECanvas();
        //PECanvas MyCanvas = null;

        UnDoRedo UnDoObject = null;    
     
        #region Constructor

        public Container()
        {
            InitializeComponent();
            Panel = new CustomPanel();           
            SetPanel();
            //MyCanvas = new PECanvas();
            //SetCanvas();
            
            InitializeEvent();
        }

        void ConfigureMapView_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                MessageBox.Show("ok");
            }
            if (e.Key == Key.Delete)
            {
                Panel.delete();
                //MessageBox.Show()
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z)
            {
                UnDoObject.Undo(1);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Y )
            {
                UnDoObject.Redo(1);
            }            
        }  

        #endregion

        private void SetPanel()
        {
         
            UnDoObject = new UnDoRedo();
            Panel.UnDoObject = UnDoObject;
            UnDoObject.Container = Panel;
            vbPanel.Child = Panel;
            UnDoObject.EnableDisableUndoRedoFeature += new EventHandler(UnDoObject_EnableDisableUndoRedoFeature);
          
        }

        void UnDoObject_EnableDisableUndoRedoFeature(object sender, EventArgs e)
        {
            if (UnDoObject.IsUndoPossible())
            {
                btnUndo.IsEnabled = true;
            }
            else
            {
                btnUndo.IsEnabled = false;
            }

            if (UnDoObject.IsRedoPossible())
            {
                btnRedo.IsEnabled = true;
            }

            else
            {
                btnRedo.IsEnabled = false;
            }
             
        }       

        void Panel_EndDrawing(object sender, EventArgs e)
        {
            ToggleButton(false);
        }

        private void ToggleButton(bool IsCheck)
        {
            EnabDisabToolbar(true);
            btnLine.IsChecked = IsCheck;
            btnRectangle.IsChecked = IsCheck;
            btnOval.IsChecked = IsCheck;        
            
        }
        private void EnabDisabToolbar(bool IsEnable)
        {
            //MainToolBar.IsEnabled = IsEnable;           
        }
        
        #region delete

        void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Panel.delete();
        }

        #endregion
 
        #region Toolbar Events

        void deleteRows(DataTable dt)
        {            
            foreach (DataRowView drv in dt.DefaultView)
            {                
                drv.Delete();                
            }         
        }
           
        void btnOval_Click(object sender, RoutedEventArgs e)
        {
            Panel.DrawEllipseApbMode();
            btnOval.IsChecked = false;
        }

        void btnRectangle_Click(object sender, RoutedEventArgs e)
        {
            Panel.DrawLineApbMode();
            btnRectangle.IsChecked = false;
        }

        void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            
            UnDoObject.Redo(1);
            Panel.RemoveResizGrip();
            
        }
        void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            UnDoObject.Undo(1);
            Panel.RemoveResizGrip();
            
        } 

        #endregion
     
        void Shape_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton(false);
            
            EnabDisabToolbar(false);
            ((ToggleButton)sender).IsChecked = true;
            switch (((ToggleButton)sender).Name)
            {

                case "btnLine":
                    Panel.DrawLineApbMode();
                    break;

                case "btnRectangle":
                    Panel.DrawRectangleApbMode();
                    break;

                case "btnOval":
                    Panel.DrawEllipseApbMode();
                    break;             

            }
        }
        
        private void InitializeEvent()
        {
            
            #region Register Event            
            btnLine.Click += new RoutedEventHandler(Shape_Click);
            btnRectangle.Click += new RoutedEventHandler(Shape_Click);
            btnOval.Click += new RoutedEventHandler(Shape_Click); 
            btnDelete.Click += new RoutedEventHandler(btnDelete_Click);
            btnUndo.Click += new RoutedEventHandler(btnUndo_Click);
            btnRedo.Click += new RoutedEventHandler(btnRedo_Click);        
            if (Panel!=null) Panel.CompleteDraw += new EventHandler(Panel_EndDrawing);
            //if (MyCanvas != null) MyCanvas.CompleteDraw += new EventHandler(Panel_EndDrawing);      
            this.KeyDown += new System.Windows.Input.KeyEventHandler(ConfigureMapView_KeyDown);
           
            #endregion
        }
     
    }
}

