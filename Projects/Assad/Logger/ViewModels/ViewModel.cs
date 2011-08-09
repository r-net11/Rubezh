using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Infrastructure.Common;
using Microsoft.Win32;

namespace Logger
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            OpenFileCommand = new RelayCommand(OpenFile);
            LoadLastFile();
        }

        public RelayCommand OpenFileCommand { get; private set; }
        public void OpenFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "log files|*.xml";
            bool? result = dialog.ShowDialog();
            if ((result.HasValue) && (result.Value == true))
            {
                string fileName = dialog.FileName;
                LoadFromFile(fileName);
            }
        }

        void LoadLastFile()
        {
            try
            {
                Process pc = Process.GetCurrentProcess();
                string directoryPath = pc.MainModule.FileName.Substring(0, pc.MainModule.FileName.LastIndexOf(@"\"));
                DirectoryInfo di = new DirectoryInfo(directoryPath + "\\Logs");
                FileInfo[] files = di.GetFiles("*.xml");
                DateTime dateTime = files.Max(x => x.CreationTime);
                FileInfo lastFile = files.First(a => a.CreationTime == dateTime);
                LoadFromFile(lastFile.FullName);
            }
            catch
            {
            }
        }

        void LoadFromFile(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            XmlSerializer serializer = new XmlSerializer(typeof(LogCollection));
            LogCollection logCollection = (LogCollection) serializer.Deserialize(reader);
            reader.Close();

            Logs = new ObservableCollection<LogEntry>(logCollection.logEnties);
        }

        ObservableCollection<LogEntry> logs;
        public ObservableCollection<LogEntry> Logs
        {
            get { return logs; }
            set
            {
                logs = value;
                OnPropertyChanged("Logs");
            }
        }

        LogEntry selectedEntry;
        public LogEntry SelectedEntry
        {
            get { return selectedEntry; }
            set
            {
                selectedEntry = value;
                OnPropertyChanged("SelectedEntry");

                XmlDocument xmlDocument = new XmlDocument();
                try
                {
                    xmlDocument.LoadXml(selectedEntry.Message);
                }
                catch (XmlException)
                {
                }
                SelectedXml = xmlDocument;
            }
        }

        XmlDocument selectedXml;
        public XmlDocument SelectedXml
        {
            get { return selectedXml; }
            set
            {
                selectedXml = value;
                OnPropertyChanged("SelectedXml");
            }
        }

        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}