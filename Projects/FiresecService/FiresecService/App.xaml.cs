using System;
using System.Windows;
using FiresecService;
using System.Diagnostics;

namespace FiresecServiceRunner
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        { 
            base.OnStartup(e);
            
#if ! DEBUG
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
#endif
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
        }
    }
}