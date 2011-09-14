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
            var dialog = new OpenFileDialog();
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
                var pc = Process.GetCurrentProcess();
                string directoryPath = pc.MainModule.FileName.Substring(0, pc.MainModule.FileName.LastIndexOf(@"\"));
                var di = new DirectoryInfo(directoryPath + "\\Logs");
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
            var reader = new StreamReader(fileName);
            var serializer = new XmlSerializer(typeof(LogCollection));
            LogCollection logCollection = (LogCollection) serializer.Deserialize(reader);
            reader.Close();

            Logs = new ObservableCollection<LogEntry>(logCollection.logEnties);
        }

        public ObservableCollection<LogEntry> Logs { get; private set; }

        LogEntry _selectedEntry;
        public LogEntry SelectedEntry
        {
            get { return _selectedEntry; }
            set
            {
                _selectedEntry = value;
                OnPropertyChanged("SelectedEntry");

                var xmlDocument = new XmlDocument();
                try
                {
                    xmlDocument.LoadXml(_selectedEntry.Message);
                }
                catch (XmlException)
                {
                }
                SelectedXml = xmlDocument;
            }
        }

        XmlDocument _selectedXml;
        public XmlDocument SelectedXml
        {
            get { return _selectedXml; }
            set
            {
                _selectedXml = value;
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