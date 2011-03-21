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
using Microsoft.VisualStudio.OLE.Interop;
using System.IO;

namespace UserControls
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Editor(typeof(System.Drawing.Design.UITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [PropertyTab(typeof(Form))]
    [Guid("8D7E002B-D236-4320-8DD5-D1E6640B2B7F")]
    public partial class UserControl5 : UserControl, ISpecifyPropertyPages
    {
        public UserControl5()
        {
            InitializeComponent();
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

        private void UserControl5_Load(object sender, EventArgs e)
        {
            // Create the ElementHost control for hosting the
            // WPF UserControl.
            ElementHost host = new ElementHost();
            host.Dock = DockStyle.Fill;

            // Create the WPF UserControl.
            WpfApplication1.Window1 uc =
                new WpfApplication1.Window1();

            // Assign the WPF UserControl to the ElementHost control's
            // Child property.
            host.Child = uc;

            // Add the ElementHost control to the form's
            // collection of child controls.
            this.Controls.Add(host);
        }

        public static UserControl5 Current;


        public void GetPages(Microsoft.VisualStudio.OLE.Interop.CAUUID[] pPages)
        {
            // 	throw new NotImplementedException();
            Current = this;
            RubezhAX.AXPropertyPage axpp = new RubezhAX.AXPropertyPage();
            StreamWriter stream = null;
            try
            {
                stream = File.CreateText("C:\\TEMP\\RubezhAX\\AXP_Logger.TXT");
            }
            catch (Exception e)
            {
                MessageBox.Show(" Ошибка");
            }
            if (stream != null) stream.WriteLine("Запуск ProperyPage");
            //    Window1 view = new Window1();
            RubezhAX.ViewModel viewModel = new RubezhAX.ViewModel(this);
            if (stream != null) stream.WriteLine("Создана  viewModel");
            viewModel.form = axpp;
            if (stream != null) stream.WriteLine("Присвоены указатели");

            if (stream != null) stream.WriteLine("Запуск goMethod");
            try
            {
                if (viewModel.goMethodAPI() == false)
                    MessageBox.Show(" Ошибка запуска goMethod()");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            axpp.DataContext = viewModel;

            axpp.ShowDialog();
            stream.WriteLine("{Завершение работы ProperyPage");
            stream.Close();
        }
    }      
}
