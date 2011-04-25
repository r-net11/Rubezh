using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;
using System.IO;

namespace RubezhAX
{
    [ProgId("RubezhAX.UC")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public partial class UCRubezh : UserControl, Microsoft.VisualStudio.OLE.Interop.ISpecifyPropertyPages
    {

        public static void DebugMessage(string mess)
        {
            string cur;
            if (Current == null) cur = "";
            cur = Current.DeviceName;

            FileStream fs=new FileStream("D:\\TEMP\\RubezhAX\\AXP_Logger.TXT", FileMode.Append, FileAccess.Write);
            fs.Close();
            
            StreamWriter sw=new StreamWriter("D:\\TEMP\\RubezhAX\\AXP_Logger.TXT", true, Encoding.Default);
            sw.WriteLine(DateTime.Now.ToShortTimeString() + " " + cur + " "  + mess);
            sw.Close();

        }

        public static UCRubezh Current;

        [ComVisible(true)]
        public bool DisableWrite { get; set; }


        //private string deviceName;
        //public string DeviceName
        //{
        //    set { if (!DisableWrite) deviceName = value; }
        //    get { return deviceName;}
        //}

        [ComVisible(true)]
        public string DeviceName
        {
            set;
            get;
        }

        [ComVisible(true)]
        public string DriverIdX { get; set; }

        [ComVisible(false)]
        public string PathX { get; set; }

        //public string 

        [ComVisible(true)]
        public string AddressX { get; set; }

        private int intStatus;

        [ComVisible(true)]
        public int IntStatus
        {
            get { return intStatus; }
            set 
            {
                Color color = SystemColors.ButtonFace;
                intStatus = value;
                if( intStatus >4 ) intStatus = 4;
           switch (intStatus)
            {
                case 0: color = SystemColors.ButtonFace; break;
                case 1: color = Color.Red; break;
                case 2: color = Color.Yellow; break;
                case 3: color = Color.Green; break;
                case 4: color = Color.Lime; break;
                default: color = SystemColors.ButtonFace; break;
            }
            this.BackColor = color;            
            strStatus = intStatus.ToString();
            
            }
        }


        private string strStatus;

        [ComVisible(true)]
        public string StrStatus
        { get { return strStatus; } }


        public UCRubezh()
        {
            intStatus = 0;
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


#region ISpecifyPropertyPages Members

public void  GetPages(Microsoft.VisualStudio.OLE.Interop.CAUUID[] pPages)
{
// 	throw new NotImplementedException();
    Current = this;
    AXPropertyPage axpp = new AXPropertyPage();



    DebugMessage("<-- " + DateTime.Now.ToShortDateString() + " ->");    
    DebugMessage("Запуск ProperyPage");    
    ViewModel viewModel = new ViewModel( this);
    viewModel.form = axpp;
    try
    {
        if (viewModel.goMethodAPI() == false)
        {
            DebugMessage("Ошибка запуска goMethodAPI()");
            MessageBox.Show(" Ошибка запуска goMethodAPI()");
            return;
        }
    }
    catch (Exception e)
    {
        DebugMessage(e.Message);
        MessageBox.Show(e.Message);
        return;    
    
    }
    
    axpp.DataContext = viewModel;
    
    axpp.ShowDialog();
    DebugMessage("Завершение работы ProperyPage");
}

#endregion


private void UCRubezh_Load(object sender, EventArgs e)
{
    DeviceNameLabel.Text = DeviceName;
    AddressLabel.Text = AddressX;
}


    } // end class declaration
}
