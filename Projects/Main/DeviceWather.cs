#define FAKE_COM_SERVER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Common;

namespace Main
{
    public class ComDeviceWather
    {
        ComServerEmulator.ComServerEmulatorViewModel emulatorViewModel;
        string message;

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

        void CreateComServerEmulator()
        {
            ComServerEmulator.ComServerEmulatorView emulatorView = new ComServerEmulator.ComServerEmulatorView();
            emulatorViewModel = new ComServerEmulator.ComServerEmulatorViewModel();
            emulatorView.DataContext = emulatorViewModel;
            emulatorView.Show();
        }

        void GetdeviceState()
        {
#if FAKE_COM_SERVER
            message = emulatorViewModel.StateMessage;
            ComServer.CoreState.config coreState = emulatorViewModel.GetCoreState();
#else
            message = ComServer.NativeComServer.GetCoreState();
            ComServer.CoreState.config coreState = ComServer.ComServer.GetCoreState();
#endif

            if (AssadDeviceManager.Devices == null)
            {
                return;
            }

            foreach (AssadDevice assadDevice in AssadDeviceManager.Devices)
            {
                assadDevice.State.StateId = 7;
            }

            if (coreState.dev != null)
            {
                foreach (ComServer.CoreState.devType innerDevice in coreState.dev)
                {
                    ComDevice comDevice = ComDeviceManager.Devices.Find(x => x.PlaceInTree == innerDevice.name);
                    string ParentAddress = comDevice.ParentAddress;
                    if (ParentAddress == "0")
                    {
                        ParentAddress = null;
                    }
                    AssadDevice assadDevice = AssadDeviceManager.Devices.First
                        (x => ((x.Address == comDevice.Address) && (x.ParentAddress == ParentAddress)));

                    List<ComState> oldComStates = new List<ComState>();
                    foreach (ComState comState in comDevice.States)
                    {
                        ComState oldComState = new ComState();
                        oldComState.Id = comState.Id;
                        oldComState.Name = comState.Name;
                        oldComState.IsActive = comState.IsActive;
                        //if (comState.IsActive)
                        //{
                        //    ;
                        //}
                        oldComState.Priority = comState.Priority;
                        oldComStates.Add(oldComState);

                        comState.IsActive = false;
                    }
                    foreach (ComServer.CoreState.stateType innerState in innerDevice.state)
                    {
                        comDevice.States.First(x => x.Id == innerState.id).IsActive = true;
                    }

                    List<ComState> comStates = comDevice.States.FindAll(x => x.IsActive);
                    if (comStates != null || comStates.Count > 0)
                    {
                        int maxPriority = comStates.Min(x => x.Priority);

                        assadDevice.State.StateId = maxPriority;

                        comDevice.MaxPriority = maxPriority;
                    }

                    foreach (ComServer.CoreState.stateType innerState in innerDevice.state)
                    {
                        ComState newValue = comDevice.States.First(x => x.Id == innerState.id);
                        ComState oldValue = oldComStates.First(x => x.Id == innerState.id);
                        //if ((oldValue.IsActive == false) && (newValue.IsActive == true))
                        {
                            assadDevice.FireEvent(newValue.Name);
                        }
                    }
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
