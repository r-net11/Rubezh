using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;
using Microsoft.Win32;
using System.Windows.Forms.Integration;
using Microsoft.VisualStudio.OLE.Interop;
using ServiceVisualizer;
using System.Threading;

namespace ActiveX
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Editor(typeof(System.Drawing.Design.UITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [PropertyTab(typeof(Form))]
    [Guid("B1B2A50B-AE0F-456B-949F-707B27BD74C4")]
    public partial class CActiveX : UserControl, ISpecifyPropertyPages
    {
        public CActiveX()
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
        
        private void ActiveX_Load(object sender, EventArgs e)
        {
            // Create the ElementHost control for hosting the
            // WPF UserControl.
            ElementHost host = new ElementHost();
            host.Dock = DockStyle.Fill;

            // Create the WPF UserControl.
            activeX = new RubezhDevices.RubezhDevice();

            // Assign the WPF UserControl to the ElementHost control's
            // Child property.
            host.Child = activeX;

            // Add the ElementHost control to the form's
            // collection of child controls.
            this.Controls.Add(host);
        }

        RubezhDevices.RubezhDevice activeX;

        public static CActiveX Current;
        public void GetPages(Microsoft.VisualStudio.OLE.Interop.CAUUID[] pPages)
        {
            Current = this;
            RubezhDevices.DeviceManager deviceManager = new RubezhDevices.DeviceManager();
            RubezhDevices.Device device = new RubezhDevices.Device();

            WindowManager.Show();
            DriverId =  WindowManager.DriverId;
            SetDriverId(DriverId);
        }
        public string DriverId { get; set; }

        public void SetDriverId(string driverId)
        {
            activeX.DriverId = driverId;
        }
    }
}
