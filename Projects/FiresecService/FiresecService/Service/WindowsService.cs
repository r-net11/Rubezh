using System.ServiceProcess;

namespace FiresecService.Service
{
    public class WindowsService : ServiceBase
    {
        public WindowsService()
        {

        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            System.Diagnostics.Debugger.Launch();
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
            FiresecManager.ConnectFiresecCOMServer("adm", "");
            FiresecServiceManager.Open();
        }

        protected override void OnStop()
        {
            base.OnStop();
            FiresecServiceManager.Close();
        }
    }
}
