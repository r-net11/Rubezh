using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.OLE.Interop;
using System.Runtime.InteropServices;
using CurrentDeviceModule.ViewModels;
using Microsoft.Win32;
using System.Reflection;
using CurrentDeviceModule.Views;
using FiresecClient;
using System.Windows;
using System.Windows.Resources;

namespace ActiveX
{
    //[ClassInterface(ClassInterfaceType.None)]
    //[PropertyTab(typeof(System.Windows.Forms.Form))]
    [Guid("03B7A288-90FB-47D2-8B2A-2CEADDD13367")]
    [ComVisible(true)]
    public partial class ActiveX : UserControl, ISpecifyPropertyPages
    {
        public ActiveX()
        {
            InitializeComponent();
        }

        CurrentDeviceViewModel _currentDeviceViewModel;
        CurrentDeviceView _currentDeviceView;

        public Guid DeviceId { get; set; }

        private void ActiveXDeviceControl_Load(object sender, EventArgs e)
        {
            StartFiresecClient();
            InitializeCurrentDevice();
        }

        private void InitializeCurrentDevice()
        {
            _currentDeviceViewModel = new CurrentDeviceViewModel();
            _currentDeviceView = new CurrentDeviceView();
            _currentDeviceView.DataContext = _currentDeviceViewModel;

            var app = new System.Windows.Application();
            Uri uri = new Uri("pack://application:,,,/Controls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;component/Themes/DataGridStyle.xaml");
            var resources = System.Windows.Application.LoadComponent(uri) as System.Windows.ResourceDictionary;

            app.Resources.MergedDictionaries.Add(resources);
            
            //_currentDeviceView.Resources.Source = uri;
            //StreamResourceInfo sri = System.Windows.Application.GetResourceStream(uri);
            //ResourceDictionary resources = (ResourceDictionary)ResourceHelper.BamlReader(sri.Stream);
            //ResourceDictionary rd = new ResourceDictionary() { Source = new System.Uri("pack://application:,,,/Controls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;component/Themes/DataGridStyle.xaml") };
            //_currentDeviceView.Resources.MergedDictionaries.Add(resources);
            elementHost.Child = _currentDeviceView;

            if (DeviceId != Guid.Empty)
            {
                _currentDeviceViewModel.Inicialize(DeviceId);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            elementHost.Width = Width;
            elementHost.Height = Height;
        }

        private void StartFiresecClient()
        {
            FiresecManager.Connect("adm", "");
        }

        public void GetPages(Microsoft.VisualStudio.OLE.Interop.CAUUID[] pPages)
        {
            _currentDeviceViewModel.SelectDevice();
            DeviceId = _currentDeviceViewModel.DeviceId;
        }

        #region ActiveX Control Registration
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

        #endregion


    }


}
