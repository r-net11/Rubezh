using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using PlanEditor;

namespace PlanEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        //PERectangle PERect = new PERectangle(5);
        //PECanvas MyCanvas = new PECanvas();
        Container objectContainer = null;

        public MainWindow()
        {

            InitializeComponent();
            
            bdrMainArea.Child = null;
            if (objectContainer == null)
            {
                objectContainer = new Container();

            }
            bdrMainArea.Child = objectContainer;
            
            /*

            this.Content = MyCanvas.GetCanvas();
            Canvas.SetTop(MyCanvas.GetCanvas(), 0);
            Canvas.SetLeft(MyCanvas.GetCanvas(), 0);
            MyCanvas.Height = 500;
            MyCanvas.Width = 500;
            Polygon poly = new Polygon();
            poly.Fill = Brushes.Black;

            PELine line = new PELine(new Point(15, 15), new Point(50, 50));
            Canvas.SetTop(line.GetShape(), 15);
            Canvas.SetLeft(line.GetShape(), 15); 
            PELine line2 = new PELine(new Point(15, 150), new Point(50, 50));
            Canvas.SetTop(line2.GetShape(), 15);
            Canvas.SetLeft(line2.GetShape(), 150); 
            PELine line6 = new PELine(new Point(50,100), new Point(50, 150));
            Canvas.SetTop(line6.GetShape(), 50);
            Canvas.SetLeft(line6.GetShape(), 100); 
            PELine line4 = new PELine(new Point(300, 150), new Point(300, 10));
            Canvas.SetTop(line4.GetShape(), 300);
            Canvas.SetLeft(line4.GetShape(), 150); 
            PELine line3 = new PELine(new Point(150, 150), new Point(250, 150));
            Canvas.SetTop(line3.GetShape(), 150);
            Canvas.SetLeft(line3.GetShape(), 150); 
            PELine line5 = new PELine(new Point(260, 160), new Point(160, 160));
            Canvas.SetTop(line5.GetShape(), 260);
            Canvas.SetLeft(line5.GetShape(), 160); 

            PERectangle rect = new PERectangle();
            //rect1.Height = rect1.Width = 32;
            //rect1.Fill = Brushes.Blue;

            Canvas.SetTop(rect.GetShape(), 8);
            Canvas.SetLeft(rect.GetShape(), 8);

            Canvas.SetTop(poly, 80);
            Canvas.SetLeft(poly, 80);
            

            PEEllipse ellipse = new PEEllipse();

            Canvas.SetTop(ellipse.GetShape(), 200);
            Canvas.SetLeft(ellipse.GetShape(), 200);


            MyCanvas.AddShape(rect);
            //MyCanvas.AddShape(tb);
            //MyCanvas.AddShape(line); 
            MyCanvas.AddShape(line2);
            MyCanvas.AddShape(ellipse);
            //MyCanvas.AddShape(line3);
            //MyCanvas.AddShape(line4);
            //MyCanvas.AddShape(line5);
            //MyCanvas.AddShape(line6);
           // MyCanvas.AddShape(poly);
            */
            //this.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(window1_PreviewKeyDown);
           // myCanvas.Children.Add(MyCanvas.GetCanvas());
            //myStackPanel.
            //myStackPanel.Children.Add(MyCanvas.GetCanvas());
            
        }

    }

    
}
