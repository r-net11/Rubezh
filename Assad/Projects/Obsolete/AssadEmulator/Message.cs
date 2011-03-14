using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Common;
using System.IO;

namespace AssadEmulator
{
    class Message : INotifyPropertyChanged
    {
        public Message(string name)
        {
            SendCommand = new RelayCommand(OnSend, x => SocketServer.Connected);

            Name = name;
            Text = LoadFromFile(name);
        }

        public string LoadFromFile(string path)
        {
            StreamReader reader = new StreamReader(@"Emulator\" + path + ".txt", Encoding.UTF8);
            string message = reader.ReadToEnd();
            reader.Close();
            return message;
        }

        public RelayCommand SendCommand { get; private set; }
        public void OnSend(object parameter)
        {
            SocketServer.Send(Text);
        }

        string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        string text;
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                OnPropertyChanged("Text");
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
