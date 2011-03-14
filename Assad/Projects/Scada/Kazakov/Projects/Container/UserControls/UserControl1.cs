using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;
using Microsoft.Win32;
using System.Windows.Forms.Integration;

namespace AControls
{
    // АПИ: 'ИП 212-64'
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Editor(typeof(System.Drawing.Design.UITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [PropertyTab(typeof(Form))]
    [Guid("F90A0BF8-AC9D-4642-9171-DDE70204B168")]
    public partial class Control1 : UserControl
    {
        private Panel panel;
        private Timer timer1;
        bool tick = false;

        public Control1()
        {
            InitializeComponent();
        }

        Color backColor1 = Color.White;
        Color backColor2 = Color.Black;
        Color backColor3 = Color.Black;
        Color backColor4 = Color.Black;
        Color PenColor = Color.Black;

        ///	<summary>
        ///	Свойство Stait  - определяеят множество всех состояний индикатора
        ///	</summary>
        int state;
        [ComVisible(true)]
        public int State
        {
            get { return state; }
            set
            {
                state = value;
                switch (state)
                {
                    // Базовый рисунок
                    case 0:
                        timer1.Enabled = false;
                        PenColor = Color.Black;
                        panel.BackColor = Color.White;
                        break;
                    // Обнаружена неисправность
                    case 1:
                        timer1.Enabled = true;
                        backColor1 = Color.Yellow;
                        backColor2 = Color.Transparent;
                        backColor3 = Color.Black;
                        backColor4 = Color.Transparent;
                        break;
                    // Обход устройств
                    case 2:
                        timer1.Enabled = false;
                        PenColor = Color.Aqua;
                        panel.BackColor = Color.Transparent;
                        break;
                    // Состояние - норма
                    case 3:
                        timer1.Enabled = false;
                        PenColor = Color.Black;
                        panel.BackColor = Color.Green;
                        break;
                    // Состояние - норма(*)
                    case 4:
                        timer1.Enabled = true;
                        backColor1 = Color.Green;
                        backColor2 = Color.Aqua;
                        backColor3 = Color.Black;
                        backColor4 = Color.Black;
                        break;
                    // Состояние - внимание
                    case 5:
                        timer1.Enabled = true;
                        backColor1 = Color.Orange;
                        backColor2 = Color.Transparent;
                        backColor3 = Color.Black;
                        backColor4 = Color.Transparent;
                        break;
                    // Состояние неизвестно
                    case 6:
                        timer1.Enabled = true;
                        backColor1 = Color.Gray;
                        backColor2 = Color.Transparent;
                        backColor3 = Color.Black;
                        backColor4 = Color.Transparent;
                        break;
                    // Состояние тревоги
                    case 7:
                        timer1.Enabled = true;
                        backColor1 = Color.Red;
                        backColor2 = Color.Transparent;
                        backColor3 = Color.Black;
                        backColor4 = Color.Transparent;
                        break;
                    // Требуется обслуживание
                    case 8:
                        timer1.Enabled = true;
                        backColor1 = Color.Brown;
                        backColor2 = Color.Transparent;
                        backColor3 = Color.Black;
                        backColor4 = Color.Transparent;
                        break;
                    // Исключение
                    default:
                        timer1.Enabled = true;
                        backColor1 = Color.White;
                        backColor2 = Color.Black;
                        backColor3 = Color.Black;
                        backColor4 = Color.White;
                        break;
                }

                this.Invalidate();
            }
        }

        ///	<summary>
        ///	Метод  OnSizeChanged переопределён с целью правильного масштабирования индикатора
        ///	</summary>

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);


            int MinSize = Math.Min(this.Size.Width, this.Size.Height);
            /*
            if (panel.Width >= panel.Parent.Width)
            {
                MinSize = Math.Min(this.Size.Width, this.Size.Height);
            }
            else
            {
                MinSize = Math.Max(this.Size.Width, this.Size.Height);
            }
            */
            panel.Width = MinSize;
            panel.Height = MinSize;
            //panel.Parent.Width = MinSize;
            //panel.Parent.Height = MinSize;
            //panel.Parent.Invalidate();
            //panel.Parent.Refresh();
            //panel.Parent.MaximumSize = new System.Drawing.Size(500, 500);

            //panel.Invalidate();
            panel.Refresh();
        }

        ///	<summary>
        ///	Метод  timer1_Tick осуществляет мигание индикатора
        ///	</summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            tick = !tick;
            if (tick)
            {
                panel.BackColor = backColor1;
                PenColor = backColor3;
            }
            else
            {
                panel.BackColor = backColor2;
                PenColor = backColor4;
            }
        }

        ///	<summary>
        ///	Метод  panel_Paint рисует индикатор датчика
        ///	</summary>
        private void panel_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(PenColor, ((float)((float)1 / (float)18)) * panel.Width);
            PointF[] points =
             {
                 new PointF(((float)((float)11/(float)18))*panel.Width, ((float)((float)3/(float)18))*panel.Height),
                 new PointF(((float)((float)11/(float)18))*panel.Width*9/10, ((float)((float)3/(float)18))*panel.Height),
                 new PointF(((float)((float)8/(float)18))*panel.Width*9/10, ((float)((float)8/(float)18))*panel.Height*9/10),
                 new PointF(((float)((float)11/(float)18))*panel.Width*9/10, ((float)((float)10/(float)18))*panel.Height*9/10),
                 new PointF(((float)((float)8/(float)18))*panel.Width*9/10, ((float)((float)15/(float)18))*panel.Height),
                 new PointF(((float)((float)8/(float)18))*panel.Width, ((float)((float)15/(float)18))*panel.Height),
                 new PointF(((float)((float)11/(float)18))*panel.Width, ((float)((float)10/(float)18))*panel.Height),
                 new PointF(((float)((float)8/(float)18))*panel.Width, ((float)((float)8/(float)18))*panel.Height)
             };

            //Draw lines to screen.
            e.Graphics.DrawLines(pen, points);
        }

        ///	<summary>
        ///	Register the class as a	control	and	set	it's CodeBase entry
        ///	</summary>
        ///	<param name="key">The registry key of the control</param>
        [ComRegisterFunction()]
        public static void RegisterClass(string key)
        {
            // Strip off HKEY_CLASSES_ROOT\ from the passed key as I don't need it
            StringBuilder sb = new StringBuilder(key);
            sb.Replace(@"HKEY_CLASSES_ROOT\", "");

            // Open the CLSID\{guid} key for write access
            RegistryKey k = Registry.ClassesRoot.OpenSubKey(sb.ToString(), true);

            // And create	the	'Control' key -	this allows	it to show up in
            // the ActiveX control container
            RegistryKey ctrl = k.CreateSubKey("Control");
            ctrl.Close();

            // Next create the CodeBase entry	- needed if	not	string named and GACced.
            RegistryKey inprocServer32 = k.OpenSubKey("InprocServer32", true);
            inprocServer32.SetValue("CodeBase", Assembly.GetExecutingAssembly().CodeBase);
            inprocServer32.Close();

            // Finally close the main	key
            k.Close();
        }

        ///	<summary>
        ///	Called to unregister the control
        ///	</summary>
        ///	<param name="key">Tke registry key</param>
        [ComUnregisterFunction()]
        public static void UnregisterClass(string key)
        {
            StringBuilder sb = new StringBuilder(key);
            sb.Replace(@"HKEY_CLASSES_ROOT\", "");

            // Open	HKCR\CLSID\{guid} for write	access
            RegistryKey k = Registry.ClassesRoot.OpenSubKey(sb.ToString(), true);

            // Delete the 'Control'	key, but don't throw an	exception if it	does not exist
            k.DeleteSubKey("Control", false);

            // Next	open up	InprocServer32
            RegistryKey inprocServer32 = k.OpenSubKey("InprocServer32", true);

            // And delete the CodeBase key,	again not throwing if missing
            k.DeleteSubKey("CodeBase", false);

            // Finally close the main key
            k.Close();
        }
        
        private void Control1_Load(object sender, EventArgs e)
        {
            // Create the ElementHost control for hosting the
            // WPF UserControl.
            ElementHost host = new ElementHost();
            host.Dock = DockStyle.Fill;

            // Create the WPF UserControl.
            HostingWpfUserControlInWf.UserControl1 uc =
                new HostingWpfUserControlInWf.UserControl1();

            // Assign the WPF UserControl to the ElementHost control's
            // Child property.
            host.Child = uc;

            // Add the ElementHost control to the form's
            // collection of child controls.
            this.panel.Controls.Add(host);

        }
        
    }
}