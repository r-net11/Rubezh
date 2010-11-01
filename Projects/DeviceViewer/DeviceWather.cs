#define FAKE_COM_SERVER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms.Integration;
using Common;

namespace DeviceViewer
{
    public class DeviceWather
    {
        public void Start()
        {
#if FAKE_COM_SERVER
            CreateComServerEmulator();
#endif

            Thread thread = new Thread(new ThreadStart(Do));
            thread.Start();
        }

        void Do()
        {
            while (true)
            {
                GetdeviceState();
                OnPropertyChanged();
                Thread.Sleep(1000);
            }
        }

        ComServerEmulator.ComServerEmulatorViewModel viewModel;

        void CreateComServerEmulator()
        {
            ComServerEmulator.ComServerEmulatorView window1 = new ComServerEmulator.ComServerEmulatorView();
            viewModel = new ComServerEmulator.ComServerEmulatorViewModel();
            window1.DataContext = viewModel;
            ElementHost.EnableModelessKeyboardInterop(window1);
            window1.Show();
        }

        string message;

        void GetdeviceState()
        {
#if FAKE_COM_SERVER
            message = viewModel.StateMessage;
            ComServer.CoreState.config coreState = viewModel.GetCoreState();
#else
            message = ComServer.NativeComServer.GetCoreState();
            ComServer.CoreState.config coreState = ComServer.ComServer.GetCoreState();
#endif

            if (coreState.dev != null)
                foreach (ComServer.CoreState.devType innerDevice in coreState.dev)
                {
                    ComDevice device = ComDeviceManager.Devices.Find(x => x.PlaceInTree == innerDevice.name);
                    foreach (ComServer.CoreState.stateType innerState in innerDevice.state)
                    {
                        device.States.First(x => x.Id == innerState.id).IsActive = true;
                    }
                }
        }

        public void OnPropertyChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(message);
        }
        public event Action<string> PropertyChanged;
    }
}
