using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using DevicesModule.ViewModels;
using DiagnosticsModule.Views;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Ionic.Zip;

namespace DiagnosticsModule.ViewModels
{
    public class DiagnosticsViewModel : ViewPartViewModel
    {
        public DiagnosticsViewModel()
        {
            ShowDriversCommand = new RelayCommand(OnShowDrivers);
            ShowXDriversCommand = new RelayCommand(OnShowXDrivers);
            ShowTreeCommand = new RelayCommand(OnShowTree);
            SecurityTestCommand = new RelayCommand(OnSecurityTest);
            Test1Command = new RelayCommand(OnTest1);
            Test2Command = new RelayCommand(OnTest2);
            Test3Command = new RelayCommand(OnTest3);
            Test4Command = new RelayCommand(OnTest4);
            Test5Command = new RelayCommand(OnTest5);
            Test6Command = new RelayCommand(OnTest6);
			Test7Command = new RelayCommand(OnTest7);
			Test8Command = new RelayCommand(OnTest8);
            Test9Command = new RelayCommand(OnTest9);
            BalloonTestCommand = new RelayCommand(OnBalloonTest);
        }

        public void StopThreads()
        {
            IsThreadStoping = true;
        }
        bool IsThreadStoping = false;

        string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        public RelayCommand ShowDriversCommand { get; private set; }
        void OnShowDrivers()
        {
            var driversView = new DriversView();
            driversView.ShowDialog();
        }

        public RelayCommand ShowXDriversCommand { get; private set; }
        void OnShowXDrivers()
        {
            var driversView = new XDriversView();
            driversView.ShowDialog();
        }

        public RelayCommand SecurityTestCommand { get; private set; }
        void OnSecurityTest()
        {
        }

        public RelayCommand ShowTreeCommand { get; private set; }
        void OnShowTree()
        {
            var devicesTreeViewModel = new DevicesTreeViewModel();
            DialogService.ShowModalWindow(devicesTreeViewModel);
        }

        int counter = 0;
        public RelayCommand Test1Command { get; private set; }
        void OnTest1()
        {
            while (true)
            {
                WriteAllDeviceConfigurationHelper.Run(false);
                Trace.WriteLine("WriteAllDeviceConfigurationHelper Count=" + counter.ToString() + " " + DateTime.Now.ToString());
                counter++;
            }
        }

        public RelayCommand Test2Command { get; private set; }
        void OnTest2()
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if (IsThreadStoping)
                        break;

                    FiresecManager.FiresecDriver.SetNewConfig(FiresecManager.FiresecConfiguration.DeviceConfiguration);
                    //FiresecManager.FiresecService.NotifyClientsOnConfigurationChanged();
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                    Trace.WriteLine("SetNewConfig Count=" + counter.ToString() + " " + DateTime.Now.ToString());
                    counter++;
                }
            }));
            thread.IsBackground = true;
            thread.Start();
        }

        public RelayCommand Test3Command { get; private set; }
        void OnTest3()
        {
        }

        public RelayCommand Test4Command { get; private set; }
        void OnTest4()
        {
        }

        public RelayCommand Test5Command { get; private set; }
        void OnTest5()
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if (IsThreadStoping)
                        break;

                    var journalRecords = new List<JournalRecord>();
                    var journalRecord = new JournalRecord()
                    {
                        DeviceTime = DateTime.Now,
                        SystemTime = DateTime.Now,
                        Description = "TestEvent"
                    };
                    journalRecords.Add(journalRecord);
                    FiresecManager.FiresecService.AddJournalRecords(journalRecords);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    Trace.WriteLine("SetNewConfig Count=" + counter.ToString() + " " + DateTime.Now.ToString());
                    counter++;
                }
            }));
            thread.IsBackground = true;
            thread.Start();
        }

        public RelayCommand Test6Command { get; private set; }
        void OnTest6()
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                int count = 0;
                while (true)
                {
					//if (Firesec.NativeFiresecClient.TasksCount > 10)
					//    continue;
                    Thread.Sleep(TimeSpan.FromMilliseconds(1000));

                    //var safeFiresecService = new SafeFiresecService("net.pipe://127.0.0.1/FiresecService/");
                    //safeFiresecService.Connect(new ClientCredentials() { ClientType = ClientType.Administrator, ClientUID = Guid.NewGuid(), UserName = "adm", Password = "" }, true);
                    //safeFiresecService.StartShortPoll(false);
                    //safeFiresecService.ShortPoll();
                    //safeFiresecService.Dispose();
                    //Trace.WriteLine("Count = " + count++.ToString());

                    FiresecManager.FiresecDriver.AddUserMessage("Test Message " + count++.ToString());
                    if (count % 1000 == 0)
                    {
                        Trace.WriteLine("Count = " + count.ToString());
                    }
                }
            }));
            thread.IsBackground = true;
            thread.Start();
        }

		public RelayCommand Test8Command { get; private set; }
		void OnTest8()
		{
			FiresecManager.FiresecDriver.AddUserMessage("Single Test Message");
		}

        public RelayCommand Test7Command { get; private set; }
		void OnTest7()
		{
            var thread = new Thread(new ThreadStart(() =>
            {
                int count = 0;
                while (true)
                {
                    if (IsThreadStoping)
                        break;

					FiresecManager.FiresecService.Test("Hello " + count++.ToString());//.ShortPoll(FiresecServiceFactory.UID);
                    Thread.Sleep(1000);
                }
            }));
            thread.IsBackground = true;
            thread.Start();
		}

        public RelayCommand Test9Command { get; private set; }
        void OnTest9()
        {
            FiresecManager.DeviceLibraryConfiguration = null;
            FiresecManager.DeviceLibraryConfiguration = GetConfig(FiresecManager.DeviceLibraryConfiguration, "DeviceLibraryConfiguration.xml");
            ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
        }

        T GetConfig<T>(T configuration, string fileName)
            where T : VersionedConfiguration, new()
        {
            var stream = FiresecManager.FiresecService.GetConfig();
            using (Stream file = File.Create("Configuration\\config.fscp"))
            {
                CopyStream(stream, file);
                file.Close();
                var unzip = ZipFile.Read("Configuration\\config.fscp", new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
                var xmlstream = new MemoryStream();
                var entry = unzip[fileName];
                if (entry != null)
                {
                    entry.Extract(xmlstream);
                    xmlstream.Position = 0;
                    configuration = ZipSerializeHelper.DeSerialize<T>(xmlstream);
                }
                return configuration;
            }
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        public RelayCommand BalloonTestCommand { get; private set; }
        void OnBalloonTest()
        {
            BalloonHelper.Show("Предупреждение", "Это текст предупреждения");
        }
    }
}